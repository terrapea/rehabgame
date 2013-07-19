using UnityEngine;
using System.Collections;

[AddComponentMenu("2D Toolkit/GUI/tk2dButtonMouseOverScaleUp")]
/// <summary>
/// Simple gui button
/// </summary>
public class tk2dButtonMouseOverScaleUp : tk2dButtonBase
{
	public float overScale = 1.03f;
	/// <summary>
	/// How much to scale the sprite when the button is in the down state
	/// </summary>
	public float targetScale = 0.97f;
	/// <summary>
	/// The length of time the scale operation takes
	/// </summary>
	public float scaleTime = 0.05f;
	
	Vector3 defaultScale;
	void Awake()
	{
		defaultScale = transform.localScale;
	}

	/*
	// Use this for initialization
	protected override void Start () 
	{
		if (viewCamera == null)
		{
			// Find a camera parent 
            Transform node = transform;
            while (node && node.camera == null)
            {
                node = node.parent;
            }
            if (node && node.camera != null) 
			{
				viewCamera = node.camera;
			}
			
			// See if a tk2dCamera exists
			if (viewCamera == null && tk2dCamera.inst)
			{
				viewCamera = tk2dCamera.inst.mainCamera;
			}
			
			// ...otherwise, use the main camera
			if (viewCamera == null)
			{
				viewCamera = Camera.main;
			}
		}
		
		if (collider == null)
		{
			BoxCollider newCollider = gameObject.AddComponent<BoxCollider>();
			Vector3 colliderExtents = newCollider.extents;
			colliderExtents.z = 0.2f;
			newCollider.extents = colliderExtents;
		}
		
		if ((buttonDownSound != null || buttonPressedSound != null || buttonUpSound != null) &&
			audio == null)
		{
			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.playOnAwake = false;
		}
	}*/
	
	IEnumerator coScale(Vector3 defaultScale, float startScale, float endScale)
    {
		iTween.Stop(gameObject,"sca");
		
		transform.localScale = defaultScale * startScale;
		
		float t0 = Time.realtimeSinceStartup;
		
		Vector3 scale = defaultScale;
		float s = 0.0f;
#if UNITY_4_0
			while (gameObject.activeSelf && s < scaleTime)
#else
			while (gameObject.active && s < scaleTime)
#endif
		{
			float t = Mathf.Clamp01(s / scaleTime);
			float scl = Mathf.Lerp(startScale, endScale, t);
			scale = defaultScale * scl;
			transform.localScale = scale;
			
			yield return 0;
			s = (Time.realtimeSinceStartup - t0);
		}
		
		transform.localScale = defaultScale * endScale;
    }
	
	protected override IEnumerator coHandleButtonPress()
	{
		buttonDown = true; // inhibit processing in Update()
		bool buttonPressed = true; // the button is currently being pressed
		
		// Button has been pressed for the first time, cursor/finger is still on it
		if (targetScale != 1.0f)
		{
			// Only do this when the scale is actually enabled, to save one frame of latency when not needed
			yield return StartCoroutine( coScale(defaultScale, 1.0f, targetScale) );
		}
		PlaySound(buttonDownSound);
		
		OnButtonDownEvent();
		
		while (Input.GetMouseButton(0))
		{
            Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
			bool colliderHit = collider.Raycast(ray, out hitInfo, 1.0e8f);
            if (buttonPressed && !colliderHit)
			{
				if (targetScale != 1.0f)
				{
					// Finger is still on screen / button is still down, but the cursor has left the bounds of the button
					//yield return StartCoroutine( coScale(defaultScale, targetScale, 1.0f) );
				}
				PlaySound(buttonUpSound);
				OnButtonUpEvent();

				buttonPressed = false;
			}
			else if (!buttonPressed & colliderHit)
			{
				if (targetScale != 1.0f)
				{
					// Cursor had left the bounds before, but now has come back in
					yield return StartCoroutine( coScale(defaultScale, 1.0f, targetScale) );
				}
				PlaySound(buttonDownSound);
				
				OnButtonDownEvent();

				buttonPressed = true;
			}
			
			if (buttonPressed)
			{
				OnButtonAutoFireEvent();
			}
			
			yield return 0;
		}
		
		if (buttonPressed)
		{
			if (targetScale != 1.0f)
			{
				// Handle case when cursor was in bounds when the button was released / finger lifted
				yield return StartCoroutine( coScale(defaultScale, targetScale, overScale) );
			}
			PlaySound(buttonPressedSound);
			/*
			if (targetObject)
			{
				targetObject.SendMessage(messageName,messageParameter);
			}
			 */
			OnButtonUpEvent();
			OnButtonPressedEvent();
			AudioManager.Instance.playButtonClicked();
			
			// Button may have been deactivated in ButtonPressed / Up event
			// Don't wait in that case
#if UNITY_4_0
			if(gameObject.activeSelf)
#else
			if (gameObject.active)
#endif
			{
				yield return StartCoroutine(LocalWaitForSeconds(pressedWaitTime));
			}
			
		}
		
		buttonDown = false;
	}
	
	// Update is called once per frame
	protected override void Update ()
	{
		if(viewCamera == null)
			return;
		
		if (!buttonDown && Input.GetMouseButtonDown(0))
        {
            Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (collider.Raycast(ray, out hitInfo, 1.0e8f))
            {
				if (!Physics.Raycast(ray, hitInfo.distance - 0.01f))
					StartCoroutine(coHandleButtonPress());
            }
        }
		else
		{
			Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
	        RaycastHit hitInfo = new RaycastHit();
			
			if (collider.Raycast(ray, out hitInfo, 1.0e8f))
            {
				if (!Physics.Raycast(ray, hitInfo.distance - 0.01f) && !isMouseOver)
					StartCoroutine(ShowMouseOver());
			}
			else
			{
				if(isMouseOver)
					StartCoroutine(RemoveMouseOver());
			}
		}
	}
	
	bool isMouseOver = false;
	private IEnumerator ShowMouseOver()
	{
		isMouseOver = true;
		yield return StartCoroutine( coScale(defaultScale, 1f, overScale) );
	}
	
	private IEnumerator RemoveMouseOver()
	{
		if(buttonDown)
			yield return StartCoroutine( coScale(defaultScale, targetScale, 1.0f ) );
		else
			yield return StartCoroutine( coScale(defaultScale, overScale, 1.0f ) );
		isMouseOver = false;
	}
}
