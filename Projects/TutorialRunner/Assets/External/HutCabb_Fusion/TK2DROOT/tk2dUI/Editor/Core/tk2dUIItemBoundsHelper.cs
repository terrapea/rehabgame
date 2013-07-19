using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public static class tk2dUIItemBoundsHelper {
    static readonly Vector3[] boxExtents = new Vector3[] {
        new Vector3(-1, -1, -1),
        new Vector3( 1, -1, -1),
        new Vector3(-1,  1, -1),
        new Vector3( 1,  1, -1),
        new Vector3(-1, -1,  1),
        new Vector3( 1, -1,  1),
        new Vector3(-1,  1,  1),
        new Vector3( 1,  1,  1),
    };

    static void GetRendererBoundsInChildren(Matrix4x4 rootWorldToLocal, Vector3[] minMax, HashSet<Transform> ignoreItems, Transform t) {
        if (!ignoreItems.Contains(t)) {
            MeshFilter mf = t.GetComponent<MeshFilter>();
            if (mf != null && mf.sharedMesh != null) {
                Bounds b = mf.sharedMesh.bounds;
                Matrix4x4 relativeMatrix = rootWorldToLocal * t.localToWorldMatrix;
                for (int j = 0; j < 8; ++j) {
                    Vector3 localPoint = b.center + Vector3.Scale(b.extents, boxExtents[j]);
                    Vector3 pointRelativeToRoot = relativeMatrix.MultiplyPoint(localPoint);
                    minMax[0] = Vector3.Min(minMax[0], pointRelativeToRoot);
                    minMax[1] = Vector3.Max(minMax[1], pointRelativeToRoot);
                }
            }
            for (int i = 0; i < t.GetChildCount(); ++i) {
                Transform child = t.GetChild(i);

                if (child.collider) {
                    continue;
                }

                GetRendererBoundsInChildren(rootWorldToLocal, minMax, ignoreItems, child);
            }
        }
    }

    static Bounds GetRendererBoundsInChildren(Transform root, HashSet<Transform> ignoreItems, Transform transform) {
        Vector3 vector3Min = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        Vector3 vector3Max = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3[] minMax = new Vector3[] {
            vector3Max,
            vector3Min
        };

        GetRendererBoundsInChildren( root.worldToLocalMatrix, minMax, ignoreItems, transform );

        Bounds b = new Bounds();
        if (minMax[0] != vector3Max && minMax[1] != vector3Min) {
            b.SetMinMax(minMax[0], minMax[1]);
        }
        return b;
    }

    public static void FixColliderBounds( tk2dUIItem item ) {
        HashSet<Transform> ignoreItems = new HashSet<Transform>( item.editorIgnoreBounds );
        Transform root = item.transform;
        Collider collider = item.collider;
        Bounds b = GetRendererBoundsInChildren(root, ignoreItems, root);

        foreach (Transform t in item.editorExtraBounds) {
            if (t != null) {
                Bounds b2 = GetRendererBoundsInChildren( root, ignoreItems, t );
                if (b2.size != Vector3.zero) {
                    b.Encapsulate(b2);
                }
            }
        }

        BoxCollider boxCollider = collider as BoxCollider;
        if (boxCollider != null) {
            Undo.RegisterUndo(boxCollider, "Fit Collider");
            b.extents = new Vector3(b.extents.x, b.extents.y, boxCollider.extents.z);
            b.center = new Vector3(b.center.x, b.center.y, boxCollider.center.z);
            boxCollider.extents = b.extents;
            boxCollider.center = b.center;
        }

        SphereCollider sphereCollider = collider as SphereCollider;
        if (sphereCollider != null) {
            Undo.RegisterUndo(sphereCollider, "Fit Collider");
            sphereCollider.center = new Vector3(b.center.x, b.center.y, 0);
            sphereCollider.radius = Mathf.Max( b.extents.x, b.extents.y );
        }
    }

    // Menu helper to fix selected item bounds.
    const string fixSelectedItemBoundsMenuEntry = tk2dMenu.root + "UI/Fix Selected Item Bounds";

    [MenuItem(fixSelectedItemBoundsMenuEntry, true)]
    public static bool ValidateFixSelectedItemBounds() {
        return (Selection.gameObjects != null && Selection.gameObjects.Length != 0);
    }

    [MenuItem(fixSelectedItemBoundsMenuEntry)]
    public static void DoFixSelectedItemBounds() {
    	if (!ValidateFixSelectedItemBounds()) {
    		return;
    	}

    	Undo.RegisterSceneUndo("Fix Selected Item Bounds");

    	HashSet<tk2dUIItem> items = new HashSet<tk2dUIItem>();
    	foreach (GameObject go in Selection.gameObjects) {
    		tk2dUIItem[] itemsToAdd = go.GetComponentsInChildren<tk2dUIItem>();
    		foreach (tk2dUIItem itemToAdd in itemsToAdd) {
    			items.Add( itemToAdd );
    		}
    	}

    	foreach (tk2dUIItem item in items) {
            FixColliderBounds(item);
    	}
    }
}


