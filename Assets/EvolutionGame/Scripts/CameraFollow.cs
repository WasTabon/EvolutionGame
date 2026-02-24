using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 6f;
    public Vector3 offset = new Vector3(0f, 22f, 0f);

    public float baseHeight = 22f;
    public float heightPerScale = 3.5f;
    public float maxHeight = 80f;
    public float zoomSmoothSpeed = 3f;

    private float targetHeight;

    void LateUpdate()
    {
        if (target == null) return;

        PlayerController pc = target.GetComponent<PlayerController>();
        if (pc != null)
        {
            float desiredHeight = baseHeight + pc.GetCurrentScale() * heightPerScale;
            targetHeight = Mathf.Clamp(desiredHeight, baseHeight, maxHeight);
        }

        offset.y = Mathf.Lerp(offset.y, targetHeight, zoomSmoothSpeed * Time.deltaTime);

        Vector3 desired = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);
    }
}