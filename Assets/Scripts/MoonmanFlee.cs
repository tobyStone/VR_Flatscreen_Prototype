using UnityEngine;
using UnityEngine.AI;

public class MoonmanFlee : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private MoonmanHealth moonmanHealth;

    [Header("Distances")]
    [SerializeField] private float triggerDistance = 4f;
    [SerializeField] private float stopRunningDistance = 7f;
    [SerializeField] private float fleeDistance = 5f;
    [SerializeField] private float minimumDistanceFromPlayer = 1.2f;

    [Header("Timing")]
    [SerializeField] private float repathInterval = 0.35f;

    [Header("Water Reaction")]
    [SerializeField] private float waterHitRunDuration = 4f;
    private float forcedRunTimer;

    [Header("Escape Search")]
    [SerializeField] private int randomFallbackAttempts = 12;
    [SerializeField] private float sideEscapeAngle = 70f;

    [Header("Water / Electrics Reaction")]
    [SerializeField] private ParticleSystem electricsSmoke;
    [SerializeField] private ParticleSystem electricsSparks;
    [SerializeField] private Light electricFlashLight;
    [SerializeField] private float flashDuration = 0.12f;
    [SerializeField] private float flashIntensity = 4f;


    private Coroutine flashRoutine;

    private float nextRepathTime;

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
        if (moonmanHealth == null)
        {
            moonmanHealth = GetComponent<MoonmanHealth>();
        }
    }

    private void Update()
    {
        if (player == null || agent == null || animator == null)
        {
            return;
        }

        if (moonmanHealth != null && moonmanHealth.IsDefeated)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (forcedRunTimer > 0f)
        {
            forcedRunTimer -= Time.deltaTime;
            RunAwayFromPlayer();

            bool forcedIsMoving = agent.velocity.magnitude > 0.1f;
            animator.SetBool("IsRunning", forcedIsMoving || agent.hasPath);

            return;
        }

        if (distanceToPlayer < triggerDistance)
        {
            RunAwayFromPlayer();
        }
        else if (distanceToPlayer > stopRunningDistance)
        {
            StopRunning();
        }

        bool isMoving = agent.velocity.magnitude > 0.1f;
        animator.SetBool("IsRunning", isMoving);
    }
    private void RunAwayFromPlayer()
    {
        if (Time.time < nextRepathTime)
        {
            return;
        }

        Vector3 away = transform.position - player.position;
        away.y = 0f;

        if (away.sqrMagnitude < 0.001f)
        {
            away = transform.forward;
        }

        away.Normalize();

        Vector3 chosenDestination;

        if (TryFindEscapeDestination(away, out chosenDestination))
        {
            agent.SetDestination(chosenDestination);
        }

        nextRepathTime = Time.time + repathInterval;
    }

    public void ReactToElectricsWaterHit(Vector3 hitPosition)
    {
        forcedRunTimer = waterHitRunDuration;
        nextRepathTime = 0f;

        if (moonmanHealth != null && moonmanHealth.IsDefeated)
        {
            return;
        }

        if (animator != null)
        {
            animator.SetTrigger("WaterHit");
            animator.SetBool("IsRunning", true);
        }

        PlayElectricalEffect(hitPosition);

        Debug.Log("Moonman electrics hit by water");

        RunAwayFromPlayer();
    }


    private void PlayElectricalEffect(Vector3 hitPosition)
    {
        Vector3 effectPosition = hitPosition + Vector3.up * 0.35f;

        if (electricsSparks != null)
        {
            electricsSparks.transform.position = effectPosition;
            electricsSparks.Play();
        }

        if (electricsSmoke != null)
        {
            electricsSmoke.transform.position = effectPosition;
            electricsSmoke.Play();
        }

        if (electricFlashLight != null)
        {
            electricFlashLight.transform.position = effectPosition;

            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
            }

            flashRoutine = StartCoroutine(FlashLightRoutine());
        }
    }

    private System.Collections.IEnumerator FlashLightRoutine()
    {
        float[] flickers = { 1f, 0.2f, 0.9f, 0.1f, 0.8f, 0f };

        for (int i = 0; i < flickers.Length; i++)
        {
            electricFlashLight.intensity = flashIntensity * flickers[i];
            yield return new WaitForSeconds(flashDuration / flickers.Length);
        }

        electricFlashLight.intensity = 0f;
        flashRoutine = null;
    }

    private bool TryFindEscapeDestination(Vector3 awayDirection, out Vector3 destination)
    {
        float[] angles =
        {
            0f,
            35f,
            -35f,
            sideEscapeAngle,
            -sideEscapeAngle,
            110f,
            -110f,
            160f,
            -160f
        };

        for (int i = 0; i < angles.Length; i++)
        {
            Vector3 direction = Quaternion.Euler(0f, angles[i], 0f) * awayDirection;
            Vector3 wantedPosition = transform.position + direction * fleeDistance;

            if (IsValidEscapePoint(wantedPosition, out destination))
            {
                return true;
            }
        }

        for (int i = 0; i < randomFallbackAttempts; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized * fleeDistance;
            Vector3 randomDirection = new Vector3(randomCircle.x, 0f, randomCircle.y);
            Vector3 wantedPosition = transform.position + randomDirection;

            if (IsValidEscapePoint(wantedPosition, out destination))
            {
                return true;
            }
        }

        destination = transform.position;
        return false;
    }

    private bool IsValidEscapePoint(Vector3 wantedPosition, out Vector3 destination)
    {
        destination = transform.position;

        NavMeshHit hit;
        if (!NavMesh.SamplePosition(wantedPosition, out hit, fleeDistance, NavMesh.AllAreas))
        {
            return false;
        }

        float distanceFromPlayer = Vector3.Distance(hit.position, player.position);
        if (distanceFromPlayer < minimumDistanceFromPlayer)
        {
            return false;
        }

        NavMeshPath path = new NavMeshPath();
        if (!agent.CalculatePath(hit.position, path))
        {
            return false;
        }

        if (path.status != NavMeshPathStatus.PathComplete)
        {
            return false;
        }

        destination = hit.position;
        return true;
    }

    private void StopRunning()
    {
        if (agent.hasPath)
        {
            agent.ResetPath();
        }

        animator.SetBool("IsRunning", false);
    }
}