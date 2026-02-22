using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    public PlayerConfig config;

    private Vector3 velocity;
    private float currentScale;
    private bool isDead;

    void Start()
    {
        Debug.Assert(config != null, "PlayerController: PlayerConfig is not assigned!");
        currentScale = config.baseScale;
        transform.localScale = Vector3.one * currentScale;
        ApplyVisuals();
    }

    void ApplyVisuals()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (config.mesh != null && mf != null) mf.mesh = config.mesh;
        if (config.material != null && mr != null) mr.material = config.material;
    }

    void Update()
    {
        if (isDead) return;
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.CurrentState != GameState.Playing) return;

        Vector3 inputDir = GetInputDirection();
        float speed = Mathf.Max(config.minSpeed, config.baseSpeed - (currentScale - config.baseScale) * config.speedScalePenalty);
        velocity = Vector3.Lerp(velocity, inputDir * speed, config.inertiaSmoothing * Time.deltaTime);

        if (GravitationalWaveEvent.Instance != null)
        {
            float force = GravitationalWaveEvent.Instance.GetForce();
            if (force > 0f)
            {
                Vector3 toCenter = (Vector3.zero - transform.position).normalized;
                velocity += toCenter * force * Time.deltaTime;
            }
        }

        transform.position += velocity * Time.deltaTime;
    }

    Vector3 GetInputDirection()
    {
        if (IsPointerOverUI()) return Vector3.zero;

        Vector3 wasd = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        if (wasd.sqrMagnitude > 0.01f) return wasd.normalized;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return Vector3.zero;
            return GetDirectionToScreenPoint(touch.position);
        }

        if (Input.GetMouseButton(0))
            return GetDirectionToScreenPoint(Input.mousePosition);

        return Vector3.zero;
    }

    Vector3 GetDirectionToScreenPoint(Vector3 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        Plane plane = new Plane(Vector3.up, transform.position);
        if (plane.Raycast(ray, out float dist))
        {
            Vector3 worldPoint = ray.GetPoint(dist);
            Vector3 dir = worldPoint - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.25f) return dir.normalized;
        }
        return Vector3.zero;
    }

    bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;
        if (EventSystem.current.IsPointerOverGameObject()) return true;
        if (Input.touchCount > 0)
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        return false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        WorldObject worldObj = other.GetComponent<WorldObject>();
        if (worldObj == null) return;

        float objScale = worldObj.GetScale();

        if (objScale < currentScale * 0.9f)
        {
            ScoreManager.Instance.AddScore(worldObj.GetPoints(), other.transform.position);

            if (AbsorptionEffect.Instance != null)
            {
                Color? objColor = null;
                MeshRenderer mr = other.GetComponent<MeshRenderer>();
                if (mr != null && mr.sharedMaterial != null)
                    objColor = mr.sharedMaterial.color;
                AbsorptionEffect.Instance.Play(other.transform.position, objScale, objColor);
            }

            if (objScale > currentScale * 0.5f && CameraShake.Instance != null)
                CameraShake.Instance.Shake(0.12f, 0.15f, 8);

            AudioManager.Instance?.PlayAbsorptionSFX();

            Grow(worldObj.GetGrowthAmount());
            worldObj.GetAbsorbed(transform.position);
        }
        else if (objScale > currentScale * 1.1f)
        {
            Die();
        }
    }

    void Grow(float amount)
    {
        currentScale += amount;
        transform.DOKill();
        transform.DOScale(Vector3.one * currentScale, 0.35f).SetEase(Ease.OutBack);

        AudioManager.Instance?.PlayGrowSFX();

        if (EvolutionManager.Instance != null)
            EvolutionManager.Instance.OnPlayerScaleChanged(currentScale);
    }

    void Die()
    {
        isDead = true;

        if (CameraShake.Instance != null)
            CameraShake.Instance.Shake(0.5f, 0.6f, 14);

        AudioManager.Instance?.PlayDeathSFX();

        transform.DOKill();
        transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InBack)
            .OnComplete(() => GameManager.Instance.GameOver());
    }

    public float GetCurrentScale() => currentScale;
    public float GetVelocityMagnitude() => velocity.magnitude;
    public void ForceKill() { if (!isDead) Die(); }
}
