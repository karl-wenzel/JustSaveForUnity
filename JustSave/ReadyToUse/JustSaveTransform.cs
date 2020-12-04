using UnityEngine;
using JustSave;

/// <summary>
/// this is a ready-to-use component using the JustSave package. Add to a object with a JustSaveSceneId or a JustSaveRuntimeId to track the position, rotation and scale across saves
/// </summary>
[RequireComponent(typeof(UnityEngine.Transform))]
public class JustSaveTransform : Savable
{
    [Header("JustSaveTransform Settings")]
    [Tooltip("Set this to true, if you have rigidbodies on this Object. It will reset the velocity of this rigidbodies when loading." +
        " If you want to save the velocity, use the JustSaveRigidbody component and set this to false.")]
    public bool ResetRigidbody = true;
    [Autosaved]
    [HideInInspector]
    public Vector3 Position;
    [Autosaved]
    [HideInInspector]
    public Quaternion Rotation;
    [Autosaved]
    [HideInInspector]
    public Vector3 Scale;

    public override void JSOnSave()
    {
        base.JSOnSave();
        Position = transform.position;
        Rotation = transform.rotation;
        Scale = transform.localScale;
    }

    public override void JSOnLoad()
    {
        base.JSOnLoad();
        transform.position = Position;
        transform.rotation = Rotation;
        transform.localScale = Scale;
        if (ResetRigidbody)
        {
            Rigidbody[] RbOnThis = gameObject.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody Rb in RbOnThis)
            {
                Rb.velocity = Vector3.zero;
                Rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
