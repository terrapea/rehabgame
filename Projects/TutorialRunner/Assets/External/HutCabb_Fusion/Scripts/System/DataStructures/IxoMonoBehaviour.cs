using UnityEngine;
using System.Collections;

public class IxoMonoBehaviour : MonoBehaviour {

	private GameObject cachedGameObject;
	new public GameObject gameObject {
		get { 
			if(cachedGameObject == null)
				cachedGameObject = gameObject;
			return cachedGameObject;
		}
	}
	
	private Transform cachedTransform;
	new public Transform transform {
		get { 
			if(cachedTransform == null)
				cachedTransform = transform;
			return cachedTransform;
		}
	}
}
