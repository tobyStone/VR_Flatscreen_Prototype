using UnityEngine;

public class SunVisualAligner : MonoBehaviour
{
    [SerializeField] private Transform targetCamera;
    [SerializeField] private Light sunLight;
    [SerializeField] private float distanceFromCamera = 500f;
    [SerializeField] private float visualScale = 40f;

    private void LateUpdate()
    {
        if (targetCamera == null || sunLight == null)
        {
            return;
        }

        Vector3 sunDirection = -sunLight.transform.forward;

        transform.position = targetCamera.position + sunDirection * distanceFromCamera;
        transform.localScale = Vector3.one * visualScale;
    }
}