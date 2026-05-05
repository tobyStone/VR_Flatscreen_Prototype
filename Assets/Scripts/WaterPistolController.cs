using UnityEngine;
using UnityEngine.InputSystem;

public class WaterPistolController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform weaponModel;
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Transform muzzlePoint;

    [Header("Resting / Droop Pose")]
    [SerializeField] private Vector3 restPositionOffset = new Vector3(0f, -0.08f, 0f);
    [SerializeField] private Vector3 restRotationOffset = new Vector3(10f, 0f, 0f);

    [Header("Firing / Aim Pose")]
    [SerializeField] private Vector3 firePositionOffset = new Vector3(0f, 0f, 0f);
    [SerializeField] private Vector3 fireRotationOffset = new Vector3(0f, 0f, 0f);
    [SerializeField] private float firePoseHoldTime = 0.15f;

    [Header("Recoil")]
    [SerializeField] private float recoilBackDistance = 0.08f;
    [SerializeField] private float recoilUpAngle = 5f;
    [SerializeField] private float recoilReturnSpeed = 12f;

    [Header("Movement Bounce")]
    [SerializeField] private float movementBounceHeight = 0.035f;
    [SerializeField] private float movementBounceSide = 0.02f;
    [SerializeField] private float movementBounceSpeed = 5f;
    [SerializeField] private float minimumMoveSpeed = 0.05f;

    [Header("Smoothing")]
    [SerializeField] private float poseReturnSpeed = 8f;

    [Header("Continuous Fire")]
    [SerializeField] private float fireInterval = 0.08f;
    private float nextFireTime;

    [Header("Water Globules")]
    [SerializeField] private GameObject waterGlobulePrefab;
    [SerializeField] private float globuleLaunchSpeed = 12f;
    [SerializeField] private int globulesPerShot = 3;
    [SerializeField] private float spreadAngle = 4f;



    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;

    private Vector3 previousPlayerPosition;
    private float recoilAmount;
    private float firePoseTimer;
    private float bounceTimer;

    private void Start()
    {
        if (weaponModel == null)
        {
            weaponModel = transform;
        }

        if (playerRoot == null)
        {
            playerRoot = transform.root;
        }

        originalLocalPosition = weaponModel.localPosition;
        originalLocalRotation = weaponModel.localRotation;

        if (playerRoot != null)
        {
            previousPlayerPosition = playerRoot.position;
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.isPressed)
        {
            if (Time.time >= nextFireTime)
            {
                Fire();
                nextFireTime = Time.time + fireInterval;
            }
        }

        UpdateTimers();
        ApplyWeaponMotion();
    }
    private void UpdateTimers()
    {
        recoilAmount = Mathf.MoveTowards(recoilAmount, 0f, recoilReturnSpeed * Time.deltaTime);
        firePoseTimer = Mathf.MoveTowards(firePoseTimer, 0f, Time.deltaTime);
    }

    private void ApplyWeaponMotion()
    {
        if (weaponModel == null)
        {
            return;
        }

        bool isInFirePose = firePoseTimer > 0f;

        Vector3 targetPosePositionOffset = isInFirePose ? firePositionOffset : restPositionOffset;
        Vector3 targetPoseRotationOffset = isInFirePose ? fireRotationOffset : restRotationOffset;

        Vector3 bounceOffset = GetMovementBounceOffset();

        Vector3 recoilOffset = new Vector3(0f, 0f, -recoilBackDistance * recoilAmount);
        Vector3 recoilRotationOffset = new Vector3(-recoilUpAngle * recoilAmount, 0f, 0f);

        Vector3 finalPosition =
            originalLocalPosition +
            targetPosePositionOffset +
            bounceOffset +
            recoilOffset;

        Quaternion finalRotation =
            originalLocalRotation *
            Quaternion.Euler(targetPoseRotationOffset + recoilRotationOffset);

        weaponModel.localPosition = Vector3.Lerp(
            weaponModel.localPosition,
            finalPosition,
            Time.deltaTime * poseReturnSpeed
        );

        weaponModel.localRotation = Quaternion.Slerp(
            weaponModel.localRotation,
            finalRotation,
            Time.deltaTime * poseReturnSpeed
        );
    }

    private Vector3 GetMovementBounceOffset()
    {
        if (playerRoot == null)
        {
            return Vector3.zero;
        }

        Vector3 movement = playerRoot.position - previousPlayerPosition;
        movement.y = 0f;

        float speed = movement.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        previousPlayerPosition = playerRoot.position;

        if (speed < minimumMoveSpeed)
        {
            bounceTimer = 0f;
            return Vector3.zero;
        }

        bounceTimer += Time.deltaTime * movementBounceSpeed;

        float verticalBounce = Mathf.Abs(Mathf.Sin(bounceTimer)) * movementBounceHeight;
        float sideBounce = Mathf.Sin(bounceTimer * 0.5f) * movementBounceSide;

        return new Vector3(sideBounce, verticalBounce, 0f);
    }

    public void Fire()
    {
        recoilAmount = 1f;
        firePoseTimer = firePoseHoldTime;

        ShootWaterGlobules();

        if (muzzlePoint != null)
        {
            Debug.Log("Water pistol fired from: " + muzzlePoint.position);
        }
    }

    private void ShootWaterGlobules()
    {
        if (waterGlobulePrefab == null || muzzlePoint == null)
        {
            return;
        }

        for (int i = 0; i < globulesPerShot; i++)
        {
            GameObject globule = Instantiate(
                waterGlobulePrefab,
                muzzlePoint.position,
                muzzlePoint.rotation
            );

            Rigidbody rb = globule.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 spread = new Vector3(
                    Random.Range(-spreadAngle, spreadAngle),
                    Random.Range(-spreadAngle, spreadAngle),
                    0f
                );

                Vector3 direction = Quaternion.Euler(spread) * muzzlePoint.forward;

                rb.linearVelocity = direction * globuleLaunchSpeed;
            }
        }
    }
}