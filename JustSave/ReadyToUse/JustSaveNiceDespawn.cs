using UnityEngine;
using JustSave;
using System.Collections;

/// <summary>
/// a component, that does a nice despawn-animation. Works as long, as you dont mess with the master scale of the prefab though other methods.
/// </summary>
[RequireComponent(typeof(JustSaveRuntimeId))]
public class JustSaveNiceDespawn : Savable
{
    public float DespawnTime;
    Vector3 StartScale;
    public float EndScaleFactor = 0.1f;
    public float StartDespawnTime;
    bool m_Pooled;
    [Autosaved]
    [HideInInspector]
    public bool DespawnAnimationRunning;
    [Autosaved]
    [HideInInspector]
    public float TimeElapsed;
    JustSaveRuntimeId m_Id;

    public override void JSOnNeeded()
    {
        base.JSOnNeeded();

        if (m_Id.IsSpawned())
        {
            if (DespawnAnimationRunning == false)
            {
                m_Pooled = false;
                StartDespawnTime = Time.time;
                StartCoroutine(DespawnAnimation());
            }
        }
        else {
            if (Dbug.Is(DebugMode.DEBUG)) UnityEngine.Debug.Log("Calling JSOnNeeded on The NiceDespawn on " + gameObject.name + ", but the object is in the pool.");
        }
    }

    public override void JSOnPooled()
    {
        base.JSOnPooled();
        StartScale = transform.localScale;
        m_Id = GetComponent<JustSaveRuntimeId>();
    }

    public override void JSOnLoad()
    {
        base.JSOnLoad();
        transform.localScale = StartScale;
        if (DespawnAnimationRunning) {
            StartDespawnTime = Time.time - TimeElapsed;
            StartCoroutine(DespawnAnimation());
        }
    }

    public override void JSOnSave()
    {
        base.JSOnSave();
        if (DespawnAnimationRunning) {
            TimeElapsed = Time.time - StartDespawnTime;
        }
    }

    public override void JSOnSpawned()
    {
        base.JSOnSpawned();
        transform.localScale = StartScale;
    }

    public override void JSOnDespawned()
    {
        base.JSOnDespawned();
        m_Pooled = true;
    }

    public IEnumerator DespawnAnimation()
    {
        DespawnAnimationRunning = true;
        while (!m_Pooled && StartDespawnTime + DespawnTime >= Time.time)
        {
            transform.localScale = (1f - (1f - EndScaleFactor) * ((Time.time - StartDespawnTime) / DespawnTime)) * StartScale;
            yield return null;
        }
        m_Id.Despawn();
        DespawnAnimationRunning = false;
    }
}
