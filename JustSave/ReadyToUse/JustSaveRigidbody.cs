using UnityEngine;
using JustSave;

/// <summary>
/// this is a ready-to-use component using the JustSave package. Add to a object with a JustSaveSceneId or a JustSaveRuntimeId to track the position, rotation and scale across saves
/// </summary>
[RequireComponent(typeof(UnityEngine.Rigidbody))]
public class JustSaveRigidbody : Savable
{

    [Autosaved]
    [HideInInspector]
    public Vector3 Velocity;
    [Autosaved]
    [HideInInspector]
    public Vector3 AngularVelocity;
    Rigidbody Rb;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
    }

    public override void JSOnSave()
    {
        base.JSOnSave();
        Velocity = Rb.velocity;
        AngularVelocity = Rb.angularVelocity;
    }

    public override void JSOnLoad()
    {
        base.JSOnLoad();
        Rb.velocity = Velocity;
        Rb.angularVelocity = AngularVelocity;
    }
}
