using UnityEngine;

public enum GravityType { Attract, Repel }

public class GravitationalBody : MonoBehaviour
{
    public float mass = 10f;
    public float radius = 15f;
    public GravityType gravityType = GravityType.Attract;

    void OnEnable()  => GravitySystem.Instance?.Register(this);
    void OnDisable() => GravitySystem.Instance?.Unregister(this);
}
