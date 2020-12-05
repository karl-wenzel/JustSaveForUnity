using UnityEngine;
using System;
using JustSave;
using System.Collections;

[RequireComponent(typeof(JustSaveTransform))]
public class JustSaveNiceDespawn : Savable
{
    public float DespawnTime;
    Vector3 StartScale;
    public float EndScaleFactor = 0.1f;
    float StartDespawnTime;
    bool m_Pooled;
    bool DespawnAnimationRunning;

    public override void JSOnNeeded()
    {
        base.JSOnNeeded();
        if (DespawnAnimationRunning == false)
        {
            m_Pooled = false;
            StartDespawnTime = Time.time;
            StartCoroutine(DespawnAnimation());
        }
    }

    public override void JSOnPooled()
    {
        base.JSOnPooled();
        StartScale = transform.localScale;
    }

    public override void JSOnDespawned()
    {
        base.JSOnDespawned();
        m_Pooled = true;
    }

    public IEnumerator DespawnAnimation() {
        DespawnAnimationRunning = true;
        while (!m_Pooled) {
            transform.localScale = (1f - (1f - EndScaleFactor) * ((Time.time - StartDespawnTime) / DespawnTime)) * StartScale;
            yield return null;
        }
        yield return new WaitForSeconds(0f);
        DespawnAnimationRunning = false;
    }
}
