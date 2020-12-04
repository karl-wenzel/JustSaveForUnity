using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JustSave;

namespace JustSave
{

    /// <summary>
    /// premade implementation of ISavable. The user can decide, if he wants to overwrite the methods JSOnSave(), JSOnLoad() and JSOnSpawned()
    /// </summary>
    public abstract class Savable : MonoBehaviour, ISavable
    {
        [Header("Savable Settings:")]
        public bool Debug = false;

        public virtual void JSOnSave() {
            if (Debug) print("Object " + name + " has been saved.");
        }

        public virtual void JSOnSpawned() {
            if (Debug) print("Object " + name + " has been spawend at " + transform.position + ".");
        }

        public virtual void JSOnDespawned()
        {
            if (Debug) print("Object " + name + " has been despawend. Returning to pool.");
        }

        public virtual void JSOnLoad() {
            if (Debug) print("Object " + name + " has been loaded.");
        }

        public virtual void JSOnPooled() {
            if (Debug) print("Object " + name + " has been pooled.");
        }

        public void JSDespawn() {
            if (Debug) print("Calling " + name + " to despawn itself.");
        }
    }

}
