using UnityEngine;
using System.Collections;

[AddComponentMenu("2D Toolkit/GUI/tk2dButtonBase")]
/// <summary>
/// Simple gui button
/// </summary>
public class tk2dButtonBase : MonoBehaviour 
{
	/// <summary>
	/// The camera this button is meant to be viewed from.
	/// Set this explicitly for best performance.\n
	/// The system will automatically traverse up the hierarchy to find a camera if this is not set.\n
	/// If nothing is found, it will fall back to the active <see cref="tk2dCamera"/>.\n
	/// Failing that, it will use Camera.main.
	/// </summary>
	public Camera viewCamera;
	
	/// <summary>
	/// Audio clip to play when the button transitions from up to down state. Requires an AudioSource component to be attached to work.
	/// </summary>
	public AudioClip buttonDownSound = null;
	/// <summary>
	/// Audio clip to play when the button transitions from down to up state. Requires an AudioSource component to be attached to work.
	/// </summary>
	public AudioClip buttonUpSound = null;
	/// <summary>
	/// Audio clip to play when the button is pressed. Requires an AudioSource component to be attached to work.
	/// </summary>
	public AudioClip buttonPressedSound = null;

	// Delegates
	/// <summary>
	/// Button event handler delegate.
	/// </summary>
	public delegate void ButtonHandlerDelegate(tk2dButtonBase source);
	
	// Messaging
	/*
	/// <summary>
	/// Target object to send the message to. The event methods below are significantly more efficient.
	/// </summary>
	public GameObject targetObject = null;
	/// <summary>
	/// The message to send to the object. This should be the name of the method which needs to be called.
	/// </summary>
    public string messageName = "";
	public string messageParameter = "";
	*/
	/// <summary>
	/// Occurs when button is pressed (tapped, and finger lifted while inside the button)
	/// </summary>
	public event ButtonHandlerDelegate ButtonPressedEvent;
	/// <summary>
	/// Occurs every frame for as long as the button is held down.
	/// </summary>
	public event ButtonHandlerDelegate ButtonAutoFireEvent;
	/// <summary>
	/// Occurs when button transition from up to down state
	/// </summary>
	public event ButtonHandlerDelegate ButtonDownEvent;
	/// <summary>
	/// Occurs when button transitions from down to up state
	/// </summary>
	public event ButtonHandlerDelegate ButtonUpEvent;
	
	public float pressedWaitTime = 0.1f;
	
	protected bool buttonDown = false;
	
	private tk2dBaseSprite baseSprite;
	public tk2dBaseSprite Sprite
	{
		get{
			if(baseSprite == null)
				baseSprite = GetComponent<tk2dBaseSprite>();
			return baseSprite;
		}
	}
	
	protected virtual void OnLevelWasLoaded()
	{
		if (viewCamera == null)
		{
			/*
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
			*/
			// ...otherwise, use the main camera
			//if (viewCamera == null)
			{
				viewCamera = Camera.main;
			}
		}
	}
	
	// Use this for initialization
	protected virtual void Start () 
	{
		if (viewCamera == null)
		{
			/*
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
			*/
			// ...otherwise, use the main camera
			//if (viewCamera == null)
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
	}
	
	// Modify this to suit your audio solution
	// In our case, we have a global audio manager to play one shot sounds and pool them
	protected void PlaySound(AudioClip source)
	{
		if (audio && source)
		{
			audio.PlayOneShot(source);
		}
	}
	
	protected IEnumerator LocalWaitForSeconds(float seconds)
	{
		float t0 = Time.realtimeSinceStartup;
		float s = 0.0f;
		while (s < seconds)
		{
			yield return 0;
			s = (Time.realtimeSinceStartup - t0);
		}
	}
	
	protected virtual IEnumerator coHandleButtonPress()
	{
		buttonDown = true; // inhibit processing in Update()
		bool buttonPressed = true; // the button is currently being pressed
		
		PlaySound(buttonDownSound);
		
		if (ButtonDownEvent != null)
			ButtonDownEvent(this);
		
		while (Input.GetMouseButton(0))
		{
            Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
			bool colliderHit = collider.Raycast(ray, out hitInfo, 1.0e8f);
            if (buttonPressed && !colliderHit)
			{
				PlaySound(buttonUpSound);
				
				if (ButtonUpEvent != null)
					ButtonUpEvent(this);

				buttonPressed = false;
			}
			else if (!buttonPressed & colliderHit)
			{
				PlaySound(buttonDownSound);
				
				if (ButtonDownEvent != null)
					ButtonDownEvent(this);

				buttonPressed = true;
			}
			
			if (buttonPressed && ButtonAutoFireEvent != null)
			{
				ButtonAutoFireEvent(this);
			}
			
			yield return 0;
		}
		
		if (buttonPressed)
		{
			PlaySound(buttonPressedSound);
			/*
			if (targetObject)
			{
				targetObject.SendMessage(messageName,messageParameter);
			}
			 */
			if (ButtonUpEvent != null)
				ButtonUpEvent(this);
			
			if (ButtonPressedEvent != null)
				ButtonPressedEvent(this);
			
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
	protected virtual void Update ()
	{
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
	}
	
	protected void OnButtonPressedEvent()
	{
		if(ButtonPressedEvent != null)
			ButtonPressedEvent(this);
	}
	
	/// <summary>
	/// Occurs every frame for as long as the button is held down.
	/// </summary>
	protected void OnButtonAutoFireEvent()
	{
		if(ButtonAutoFireEvent != null)
			ButtonAutoFireEvent(this);
	}
	
	/// <summary>
	/// Occurs when button transition from up to down state
	/// </summary>
	protected void OnButtonDownEvent()
	{
		if(ButtonDownEvent != null)
			ButtonDownEvent(this);
	}
	
	/// <summary>
	/// Occurs when button transitions from down to up state
	/// </summary>
	protected void OnButtonUpEvent()
	{
		if(ButtonUpEvent != null)
			ButtonUpEvent(this);
	}
}
