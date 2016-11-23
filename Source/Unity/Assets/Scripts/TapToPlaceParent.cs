using UnityEngine;

public class TapToPlaceParent : MonoBehaviour
{
    public static bool Placing;
    void Start()
    {
        Placing = true;
        SpatialMapping.Instance.DrawVisualMeshes = true;
    }
    // Called by GazeGestureManager when the user performs a Select gesture
    void OnSelect()
    {
        // If the user is in placing mode, display the spatial mapping mesh.
        if (Placing)
        {
            Placing = !Placing;
            SpatialMapping.Instance.DrawVisualMeshes = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // If the user is in placing mode,
        // update the placement to match the user's gaze.

        if (Placing)
        {
            // Do a raycast into the world that will only hit the Spatial Mapping mesh.
            var headPosition = Camera.main.transform.position;
            var gazeDirection = Camera.main.transform.forward;

            RaycastHit hitInfo;
            if (Physics.Raycast(headPosition, gazeDirection, out hitInfo,
                30.0f, SpatialMapping.PhysicsRaycastMask))
            {
                // Move this object's parent object to
                // where the raycast hit the Spatial Mapping mesh.
                this.transform.position = hitInfo.point - new Vector3(0.5f,0f,0.5f);
            }
        }
    }
}