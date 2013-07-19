using UnityEngine;
using System.Collections;

[AddComponentMenu("2D Toolkit/GUI/tk2dButtonMouseOver")]
/// <summary>
/// Simple gui button
/// </summary>
public class tk2dButtonMouseOver : tk2dButtonBase 
{
	// Button Up = normal state
	// Button Down = held down
	// Button Pressed = after it is pressed and activated
	
	/// <summary>
	/// The button down sprite. This is resolved by name from the sprite collection of the sprite component.
	/// </summary>
	public string buttonDownSprite = "button_over";
	/// <summary>
	/// The button up sprite. This is resolved by name from the sprite collection of the sprite component.
	/// </summary>
	public string buttonUpSprite = "button_up";
	/// <summary>
	/// The button pressed sprite. This is resolved by name from the sprite collection of the sprite component.
	/// </summary>
	
	[HideInInspector]
	public int buttonDownSpriteId = -1, buttonUpSpriteId = -1;

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

	tk2dBaseSprite sprite;

	/// <summary>
	/// How much to scale the sprite when the button is in the down state
	/// </summary>
	public float targetScale = 0.95f;
	/// <summary>
	/// The length of time the scale operation takes
	/// </summary>
	public float scaleTime = 0.05f;
	
	protected override void Start ()
	{
		base.Start ();
		UpdateSpriteIds();
	}

	/// <summary>
	/// Call this when the sprite names have changed
	/// </summary>
	public void UpdateSpriteIds()
	{
		if(sprite == null)
			sprite = GetComponent<tk2dBaseSprite>();
		
		buttonDownSpriteId 		= (buttonDownSprite.Length > 0)?sprite.GetSpriteIdByName(buttonDownSprite):-1;
		buttonUpSpriteId 		= (buttonUpSprite.Length > 0)?sprite.GetSpriteIdByName(buttonUpSprite):-1;
	}
	
	IEnumerator coScale(Vector3 defaultScale, float startScale, float endScale)
    {
		float t0 = Time.realtimeSinceStartup;
		
		Vector3 scale = defaultScale;
		float s = 0.0f;
		while (s < scaleTime)
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
		
		Vector3 defaultScale = transform.localScale;
		
		// Button has been pressed for the first time, cursor/finger is still on it
		if (targetScale != 1.0f)
		{
			// Only do this when the scale is actually enabled, to save one frame of latency when not needed
			yield return StartCoroutine( coScale(defaultScale, 1.0f, targetScale) );
		}
		PlaySound(buttonDownSound);
		if (buttonDownSpriteId != -1)
			sprite.spriteId = buttonDownSpriteId;
		
		base.OnButtonDownEvent();
		
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
					yield return StartCoroutine( coScale(defaultScale, targetScale, 1.0f) );
				}
				PlaySound(buttonUpSound);
				if (buttonUpSpriteId != -1)
					sprite.spriteId = buttonUpSpriteId;
				
				base.OnButtonUpEvent();

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
				if (buttonDownSpriteId != -1)
					sprite.spriteId =  buttonDownSpriteId;
				
				base.OnButtonDownEvent();

				buttonPressed = true;
			}
			
			if (buttonPressed)
			{
				base.OnButtonAutoFireEvent();
			}
			
			yield return 0;
		}
		
		if (buttonPressed)
		{
			if (targetScale != 1.0f)
			{
				// Handle case when cursor was in bounds when the button was released / finger lifted
				yield return StartCoroutine( coScale(defaultScale, targetScale, 1.0f) );
			}
			AudioManager.Instance.playButtonClicked();
			PlaySound(buttonPressedSound);
			if (buttonDownSpriteId != -1)
				sprite.spriteId = buttonDownSpriteId;
				
			base.OnButtonUpEvent();
			base.OnButtonPressedEvent();
			
			if (buttonDownSpriteId != -1)
				sprite.spriteId = buttonDownSpriteId;// buttonUpSpriteId;
			
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
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
		
		if (collider.Raycast(ray, out hitInfo, 1.0e8f))
        {
			if (!Physics.Raycast(ray, hitInfo.distance - 0.01f))
			{
				if (!buttonDown && Input.GetMouseButtonDown(0))
				{
					StartCoroutine(coHandleButtonPress());
				}
				else
					ShowMouseOver();
			}
		}
		else
			RemoveMouseOver();
		
		/*
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
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	        RaycastHit hitInfo;
			
			if (collider.Raycast(ray, out hitInfo, 1.0e8f))
            {
				if (!Physics.Raycast(ray, hitInfo.distance - 0.01f))
					ShowMouseOver();
			}
			else
			{
				RemoveMouseOver();
			}
		}
		*/
	}
	
	public void ShowMouseOver()
	{
		sprite.spriteId = buttonDownSpriteId;
	}
	
	public void RemoveMouseOver()
	{
		sprite.spriteId = buttonUpSpriteId;
	}
}
