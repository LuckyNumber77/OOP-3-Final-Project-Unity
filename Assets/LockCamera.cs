using UnityEngine;

public class LockCamera : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Camera cam;
    private float initialSize;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        cam = GetComponent<Camera>();
        initialSize = cam.orthographicSize;
    }

    void LateUpdate()
    {
        // Reset position and rotation every frame
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        cam.orthographicSize = initialSize;
    }
}