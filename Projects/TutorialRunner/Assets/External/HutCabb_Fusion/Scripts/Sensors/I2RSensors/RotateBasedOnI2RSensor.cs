using UnityEngine;
using System.Collections;

public class RotateBasedOnI2RSensor : MonoBehaviour {
	
	public int sensorIndex = 0;
	Quaternion initialRotation;
	
	void Awake()
	{
		initialRotation = Quaternion.Inverse(transform.root.rotation) * transform.rotation;
		print(initialRotation.eulerAngles);
	}
	
	// Update is called once per frame
	void Update () {
		
		Vector3 mag = I2rSensor.Instance.Fusions[sensorIndex].Magnet;
		transform.LookAt(mag);
		
		if(mag.x > 100f || mag.x < -100f
			|| mag.y > 100f || mag.y < -100f
			|| mag.z > 100f || mag.z < -100f)
			return;
		
		FileConsole.WriteLine(mag.x + "\t" + mag.y + "\t" + mag.z);
		
		//transform.rotation =  I2rSensor.Instance.GetQuaternion(sensorIndex);
		/*
		Quaternion q = I2rSensor.Instance.GetQuaternion(sensorIndex);
		q *= Quaternion.Euler(90f,0f,0f);
		q *= Quaternion.Euler(0f,180f,0f);
		Vector3 euler = q.eulerAngles;*/
		//euler.z *= -1;
		
		//Quaternion q = I2rSensor.Instance.GetQuaternion(sensorIndex);
		
		/*
		Vector3 euler = q.eulerAngles;
		Vector3 right = q * Vector3.right;
		Vector3 forward = q * Vector3.forward;
		Vector3 up = q * Vector3.up;
		float forwardAngle = Mathf.Atan2(forward.z,forward.y);
		*/
		//transform.rotation = transform.root.rotation * Quaternion.Euler(-90f,0f,0f) * q * initialRotation;
		//transform.rotation = (transform.root.rotation * Quaternion.Euler(90f,0f,0f)) * q * initialRotation;// * Quaternion.Euler(0f,-90f,0f);
		//transform.rotation = q;
	}
}
