using UnityEngine;

public class CanvasFollower : MonoBehaviour
{
    public Transform cameraTransform;
    public float distanceFromCamera = 2.0f; // Adjust to set the Canvas distance from the camera

    void Start()
    {
        // If no camera is assigned, try to find the main camera
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        if (cameraTransform == null) return;

        // Position the Canvas in front of the camera
        transform.position = cameraTransform.position + cameraTransform.forward * distanceFromCamera;

        // Rotate the Canvas to face the camera
        transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
    }
}
