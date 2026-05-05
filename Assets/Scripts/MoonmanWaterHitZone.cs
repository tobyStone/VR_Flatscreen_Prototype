using UnityEngine;

public class MoonmanWaterHitZone : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MoonmanFlee moonmanFlee;
    [SerializeField] private MoonmanHealth moonmanHealth;

    [Header("Settings")]
    [SerializeField] private float hitCooldown = 0.35f;
    [SerializeField] private bool destroyWaterGlobuleOnHit = true;

    private float nextAllowedHitTime;

    private void Awake()
    {
        if (moonmanFlee == null)
        {
            moonmanFlee = GetComponentInParent<MoonmanFlee>();
        }

        if (moonmanHealth == null)
        {
            moonmanHealth = GetComponentInParent<MoonmanHealth>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time < nextAllowedHitTime)
        {
            return;
        }

        WaterGlobuleProjectile waterGlobule = other.GetComponentInParent<WaterGlobuleProjectile>();

        if (waterGlobule == null)
        {
            return;
        }

        nextAllowedHitTime = Time.time + hitCooldown;

        if (moonmanHealth != null)
        {
            moonmanHealth.RegisterElectricHit(transform.position);
        }
        else
        {
            Debug.LogWarning("MoonmanHealth is missing on ElectricsHitZone.");
        }

        if (moonmanHealth == null || !moonmanHealth.IsDefeated)
        {
            if (moonmanFlee != null)
            {
                moonmanFlee.ReactToElectricsWaterHit(transform.position);
            }
        }

        if (destroyWaterGlobuleOnHit)
        {
            Destroy(waterGlobule.gameObject);
        }
    }
}