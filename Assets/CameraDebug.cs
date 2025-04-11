using UnityEngine;

public class CameraDebug : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        Debug.Log($"Camera setup - Size: {cam.orthographicSize}, Position: {transform.position}");
    }

    void Update()
    {
        // Print any changes during runtime
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"Camera current - Size: {cam.orthographicSize}, Position: {transform.position}");
        }
    }
}