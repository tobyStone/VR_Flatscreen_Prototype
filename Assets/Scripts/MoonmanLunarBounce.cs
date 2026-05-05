using UnityEngine;
using UnityEngine.AI;

public class MoonmanLunarBounce : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform visualModel;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;

    [Header("Moon Bounce")]
    [SerializeField] private float bounceHeight = 0.35f;
    [SerializeField] private float bounceSpeed = 2.2f;
    [SerializeField] private float minimumMoveSpeed = 0.1f;

    [Header("Animation Speed")]
    [SerializeField] private float idleAnimationSpeed = 1f;
    [SerializeField] private float runAnimationSpeed = 0.55f;

    private Vector3 originalLocalPosition;
    private float bounceTimer;

    private void Awake()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (visualModel == null && animator != null)
        {
            visualModel = animator.transform;
        }

        if (visualModel != null)
        {
            originalLocalPosition = visualModel.localPosition;
        }
    }

    private void Update()
    {
        if (agent == null || animator == null || visualModel == null)
        {
            return;
        }

        bool isMoving = agent.velocity.magnitude > minimumMoveSpeed;

        if (isMoving)
        {
            bounceTimer += Time.deltaTime * bounceSpeed;

            float bounce = Mathf.Abs(Mathf.Sin(bounceTimer)) * bounceHeight;

            visualModel.localPosition = originalLocalPosition + new Vector3(0f, bounce, 0f);

            animator.speed = runAnimationSpeed;
        }
        else
        {
            bounceTimer = 0f;

            visualModel.localPosition = Vector3.Lerp(
                visualModel.localPosition,
                originalLocalPosition,
                Time.deltaTime * 8f
            );

            animator.speed = idleAnimationSpeed;
        }
    }
}