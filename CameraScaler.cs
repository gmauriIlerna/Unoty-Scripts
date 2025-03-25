using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    public float baseOrthographicSize = 5f; // Adjust this based on your design
    public float targetAspect = 16f / 9f; // Adjust based on your reference aspect ratio

    void Start()
    {
        Camera cam = Camera.main;
        float currentAspect = (float)Screen.width / Screen.height;
        cam.orthographicSize = baseOrthographicSize * (targetAspect / currentAspect);
    }
}
