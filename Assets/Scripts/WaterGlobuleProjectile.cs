using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WaterGlobuleProjectile : MonoBehaviour
{
    [Header("Gravity")]
    [SerializeField] private float lunarGravity = 1.62f;

    [Header("Lifetime")]
    [SerializeField] private float lifetime = 6f;

    [Header("Impact")]
    [SerializeField] private bool destroyOnImpact = true;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        rb.AddForce(Vector3.down * lunarGravity, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (destroyOnImpact)
        {
            Destroy(gameObject);
        }
    }
}