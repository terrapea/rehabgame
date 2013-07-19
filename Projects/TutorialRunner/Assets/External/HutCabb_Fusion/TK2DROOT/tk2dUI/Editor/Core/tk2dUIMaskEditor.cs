using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(tk2dUIMask))]
public class tk2dUIMaskEditor : Editor {
	public override void OnInspectorGUI() {
		tk2dUIMask mask = (tk2dUIMask)target;

		DrawDefaultInspector();
		if (GUI.changed) {
			mask.Build();
		}
	}
}
