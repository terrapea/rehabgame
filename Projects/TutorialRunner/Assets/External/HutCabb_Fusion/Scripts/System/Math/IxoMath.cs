using UnityEngine;
using System.Collections;

public static class IxoMath {

	public static Quaternion FromToRotation(Quaternion a, Quaternion b) {
		return Quaternion.Inverse(a) * b;
	}
	
	public static Quaternion FlipXYAxes(Quaternion q)
	{
		Vector3 euler = q.eulerAngles;
		return Quaternion.Euler(euler.z,euler.x,-euler.x);
	}
}
