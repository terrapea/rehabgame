using UnityEngine;
using System.Collections;
using System;

public class SimpleMoveScript : MonoBehaviour {
	
	public float speed = 10;
	public float rotated_speed = Mathf.PI/18;
	
	public delegate void OnBasicEventHandler();
	public event OnBasicEventHandler OnCallibrateBtnPressed;
	
	void Awake()
	{
		OnCallibrateBtnPressed = I2rSkeletonController.Instance.CalibrateAndResetSensor;
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
		float dt = Time.deltaTime;
		float horizAxis = Input.GetAxis("Fire1");
		bool left  = Input.GetKey(KeyCode.D);
		bool right = Input.GetKey(KeyCode.A);
		Vector3 oldPos = this.transform.position;
				
		if(left)
		{
			oldPos.x += speed*dt;
			this.transform.position = oldPos;
		}else if( right )
		{
			oldPos.x -= speed*dt;
			this.transform.position = oldPos;
		}
		
		transform.RotateAround(new Vector3(0,1,0),rotated_speed*dt);
		
		
		bool space = Input.GetKey(KeyCode.Space);
		if(space)
		{
			OnCallibrateBtnPressed();
		}
	}
}
