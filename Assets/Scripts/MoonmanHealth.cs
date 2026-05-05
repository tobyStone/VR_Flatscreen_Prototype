using UnityEngine;
using UnityEngine.AI;

public class MoonmanHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxElectricHits = 5;
    [SerializeField] private int currentElectricHits;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private MoonmanFlee moonmanFlee;
    [SerializeField] private MoonmanLunarBounce lunarBounce;
    [SerializeField] private MoonmanHealthBarUI healthBarUI;

    [Header("Animation")]
    [SerializeField] private string deathTriggerName = "MoonmanDies";

    private bool isDefeated;

    public bool IsDefeated
    {
        get { return isDefeated; }
    }

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        if (moonmanFlee == null)
        {
            moonmanFlee = GetComponent<MoonmanFlee>();
        }

        if (lunarBounce == null)
        {
            lunarBounce = GetComponent<MoonmanLunarBounce>();
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    public void RegisterElectricHit(Vector3 hitPosition)
    {
        if (isDefeated)
        {
            return;
        }

        currentElectricHits++;
        currentElectricHits = Mathf.Clamp(currentElectricHits, 0, maxElectricHits);

        UpdateUI();

        if (healthBarUI != null)
        {
            healthBarUI.Fizz();
        }

        if (currentElectricHits >= maxElectricHits)
        {
            DefeatMoonman();
        }
    }

    private void UpdateUI()
    {
        if (healthBarUI == null)
        {
            return;
        }

        float remainingHealth = 1f - ((float)currentElectricHits / maxElectricHits);
        healthBarUI.SetHealth(remainingHealth);
    }

    private void DefeatMoonman()
    {
        isDefeated = true;

        if (agent != null)
        {
            agent.ResetPath();
            agent.isStopped = true;
        }

        if (moonmanFlee != null)
        {
            moonmanFlee.enabled = false;
        }

        if (lunarBounce != null)
        {
            lunarBounce.enabled = false;
        }

        if (animator != null)
        {
            animator.SetBool("IsRunning", false);
            animator.ResetTrigger("WaterHit");
            animator.SetTrigger(deathTriggerName);
        }

        Debug.Log("Moonman defeated.");
    }
}