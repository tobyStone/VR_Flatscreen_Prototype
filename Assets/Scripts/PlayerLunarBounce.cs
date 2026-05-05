using UnityEngine;

public class PlayerLunarBounce : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform visualTarget;

    [Header("Movement Detection")]
    [SerializeField] private float minimumMoveSpeed = 0.1f;

    [Header("Moon Bounce")]
    [SerializeField] private float bounceHeight = 0.37f;
    [SerializeField] private float bounceSpeed = 1.7f;
    [SerializeField] private float returnSpeed = 8f;

    private Vector3 originalLocalPosition;
    private Vector3 previousPosition;
    private float bounceTimer;

    private void Start()
    {
        if (visualTarget == null)
        {
            visualTarget = transform;
        }

        originalLocalPosition = visualTarget.localPosition;
        previousPosition = transform.position;
    }

    private void Update()
    {
        Vector3 currentPosition = transform.position;

        Vector3 horizontalMovement = currentPosition - previousPosition;
        horizontalMovement.y = 0f;

        float speed = horizontalMovement.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        bool isMoving = speed > minimumMoveSpeed;

        if (isMoving)
        {
            bounceTimer += Time.deltaTime * bounceSpeed;

            float bounce = Mathf.Abs(Mathf.Sin(bounceTimer)) * bounceHeight;

            visualTarget.localPosition = originalLocalPosition + new Vector3(0f, bounce, 0f);
        }
        else
        {
            bounceTimer = 0f;

            visualTarget.localPosition = Vector3.Lerp(
                visualTarget.localPosition,
                originalLocalPosition,
                Time.deltaTime * returnSpeed
            );
        }

        previousPosition = currentPosition;
    }
}