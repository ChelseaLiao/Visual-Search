using UnityEngine;
using ViveSR.anipal.Eye;

public class EyeGazeDetect : MonoBehaviour
{
    private const int LengthOfRay = 10; // Define the length of the Ray projected from the eye. Adjust this value based on your needs.
    private LineRenderer gazeRayRenderer;

    //public Transform CameraTransform; // Assign the VR Camera (headset) Transform here in the Inspector

    private void Start()
    {
        // If you want to visualize the eye gaze as a ray in your scene, you can use LineRenderer.
        gazeRayRenderer = gameObject.AddComponent<LineRenderer>();
        gazeRayRenderer.startWidth = 0.01f;
        gazeRayRenderer.endWidth = 0.01f;
    }

    private void Update()
    {
        if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING) return;

        Vector3 gazeOrigin;
        Vector3 gazeDirection;

        bool eyeDataValid = SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out gazeOrigin, out gazeDirection);

        if (eyeDataValid)
        {
            // Adjust the gaze origin and direction according to the headset's position and rotation
            gazeOrigin = Camera.main.transform.position + Camera.main.transform.rotation * gazeOrigin;
            gazeDirection = Camera.main.transform.rotation * gazeDirection;

            Ray gazeRay = new Ray(gazeOrigin, gazeDirection);
            RaycastHit hitInfo;

            if (Physics.Raycast(gazeRay, out hitInfo, LengthOfRay))
            {
                GameObject targetObject = hitInfo.transform.gameObject;
                Debug.Log("Looking at: " + targetObject.name);

                // Optional: visualize the gaze ray
                gazeRayRenderer.SetPosition(0, gazeRay.origin);
                gazeRayRenderer.SetPosition(1, hitInfo.point);
            }
            else
            {
                // Optional: visualize the gaze ray
                gazeRayRenderer.SetPosition(0, gazeRay.origin);
                gazeRayRenderer.SetPosition(1, gazeRay.origin + gazeRay.direction * LengthOfRay);
            }
        }
    }
}
