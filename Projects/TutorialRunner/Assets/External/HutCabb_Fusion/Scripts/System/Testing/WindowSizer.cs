using UnityEngine;

using System.Collections;

using System.Collections.Generic;

 

[ExecuteInEditMode]

public class WindowSizer : MonoBehaviour {
 
#if UNITY_EDITOR
    private static KeyValuePair<int, int>[] resolutions = new KeyValuePair<int, int>[]{

        //new KeyValuePair<int, int>(320, 240),  // 4:3 Very Common

        //new KeyValuePair<int, int>(400, 240),  // 5:3 Samsung Intercept, Uncommon

        new KeyValuePair<int, int>(800, 480),  // 5:3 Very Common
		
		new KeyValuePair<int, int>(480, 800),  // 5:3 Very Common

        new KeyValuePair<int, int>(854, 480),  // 16:9 Motorola Droid

        new KeyValuePair<int, int>(960, 540),  // 16:9 Becoming Common on high end phones

        new KeyValuePair<int, int>(1024, 600),  // 16:9 Tablet

        new KeyValuePair<int, int>(800, 600),  // 4:3 Tablet

        new KeyValuePair<int, int>(1200, 800),  // 3:2 Tablet

        new KeyValuePair<int, int>(1200, 800),  // 3:2 Tablet

        new KeyValuePair<int, int>(1024, 768),  // 4:3 iPad

        new KeyValuePair<int, int>(960, 640)  // 3:2 iPhone

    };

    

    void OnGUI()

    {

        // Only show the overlay in the editor

        if(!Application.isEditor)
            return;

        

        GUIStyle lowerRightAligned = new GUIStyle();

        lowerRightAligned.alignment = TextAnchor.LowerRight;

        

        foreach (KeyValuePair<int, int> res in resolutions)

        {

            //GUI.Box(new Rect(resolution.x - 5, resolution.y -5, 5, 5), string.Empty);

            GUILayout.BeginArea(new Rect(res.Key - 55, res.Value - 15, 50, 15));

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.Label(res.Key + "x" + res.Value, lowerRightAligned, GUILayout.ExpandHeight(true));

            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            GUI.Box(new Rect(res.Key - 7, res.Value - 7, 5, 5), string.Empty);

        }

        // Show the current window size

        GUI.Label(new Rect(0, 0, 80, 20), Screen.width + "x" + Screen.height);

    }
#endif
}
