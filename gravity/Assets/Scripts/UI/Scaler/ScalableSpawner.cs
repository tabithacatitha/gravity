using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(SelectableObject))]
public class ScalableSpawner : MonoBehaviour, IScalable
{
    [SerializeField] float scaleMultiplier = 1f;
    [SerializeField] new ParticleSystem particleSystem;
    private ParticleSystem.VelocityOverLifetimeModule velocityModule;
    private ParticleSystem.EmissionModule emission;
    // private ParticleSystem.MainModule mainParticleSystem;
    private Vector3 savedVelocity;

    void Awake()
    {
        // mainParticleSystem = particleSystem.main
        emission = particleSystem.emission;
        velocityModule = particleSystem.velocityOverLifetime;
    }

    public void RestoreScale()
    {
        velocityModule.xMultiplier = savedVelocity.x;
        velocityModule.yMultiplier = savedVelocity.y;
        // velocityModule.zMultiplier = savedVelocity.z;
    }

    public void SaveScale()
    {
        savedVelocity.x = velocityModule.xMultiplier;
        savedVelocity.y = velocityModule.yMultiplier;
        // savedVelocity.z = velocityModule.zMultiplier;
    }

    public void ScaleTo(Vector2 mousePosition, float startingMouseDistance)
    {
        velocityModule.xMultiplier = mousePosition.x * scaleMultiplier;
        velocityModule.yMultiplier = mousePosition.y * scaleMultiplier;
    }

    public void Enable()
    {
        emission.rateOverTime = 5;
    }

    public void Disable()
    {
        emission.rateOverTime = 0;
    }

    void OnDestroy()
    {
        particleSystem.transform.parent = null;
        emission.enabled = false;
        Destroy(particleSystem.gameObject, particleSystem.main.startLifetime.constantMax);
    }
}