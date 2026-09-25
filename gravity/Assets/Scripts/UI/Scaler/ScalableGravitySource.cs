using System;
using UnityEngine;

[RequireComponent(typeof(SelectableObject))]
public class ScalableGravitySource : MonoBehaviour, IScalable
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] ParticleSystemForceField forceField;
    private Material material;
    float savedRadius;

    // need to update:
    // force field strength
    // scale of the object
    // radius on SelectableObject
    // material scale

    void Awake()
    {
        material = spriteRenderer.material;
    }

    public void RestoreScale()
    {
        ApplyScale(savedRadius);
    }

    public void SaveScale()
    {
        savedRadius = forceField.gravity.constant;
    }

    public void ScaleTo(Vector2 mousePosition, float startingMouseDistance)
    {
        float newRadius = Mathf.Max(0.1f, savedRadius - startingMouseDistance + mousePosition.magnitude); // minimum size
        ApplyScale(newRadius);

    }

    public void Enable()
    {
        forceField.gravity = new ParticleSystem.MinMaxCurve(savedRadius);
    }
    public void Disable()
    {
        savedRadius = forceField.gravity.constant;
        forceField.gravity = new ParticleSystem.MinMaxCurve(0f);
    }

    void ApplyScale(float newRadius)
    {
        forceField.gravity = new ParticleSystem.MinMaxCurve(newRadius);
        this.transform.localScale = Vector3.one * 2 * newRadius;
        GetComponent<SelectableObject>().radius = newRadius;
        material.SetFloat("_Radius", newRadius - 0.2f);
        // material.SetFloat("_OutRadius", newRadius * 0.4f);
        material.SetFloat("_InRadius", newRadius * 0.1f);
    }
}