using UnityEngine;
using System.Collections;

public class ToggleMapping : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void OnSelect()
    {
        Toggle();
    }

    public static void Toggle()
    {
        if (SpatialMapping.Instance != null)
        {
            SpatialMapping.Instance.ResetMaterialTime();
            if (WorldCursor.Instance != null && WorldCursor.Instance.GotHit)
                SpatialMapping.Instance.SetMaterialTarget(WorldCursor.Instance.HitInfo.point);
            else
                SpatialMapping.Instance.SetMaterialTarget(Camera.main.transform.position);
            SpatialMapping.Instance.DrawVisualMeshes = !SpatialMapping.Instance.DrawVisualMeshes;
        }
    }
}
