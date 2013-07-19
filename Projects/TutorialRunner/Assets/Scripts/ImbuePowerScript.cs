using UnityEngine;
using System.Collections;

public class ImbuePowerScript : MonoBehaviour {
	
	public Collider Palm = null;
	private bool isCharging = false;
	private float chargePower = 1;
	
	void OnTriggerEnter( Collider other )
	{
		if(other == Palm)
		{
			// StartPhase
			Debug.Log("START enter");
		}
	}
	
	void OnTriggerStay( Collider other )
	{
		if(other == Palm)
		{
		}
	}
	
	void OnTriggerExit( Collider other )
	{
		if(other == Palm)
		{
			Debug.Log("START exit");
		}
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}
}
