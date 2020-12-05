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
        [Tooltip("Set this to true, if you want to receive Debug messages every time this object is called by the JustSave-System. Note, that your Saving System should also at least be in DebugMode.INFO")]
        public bool Debug = false;

        public virtual void JSOnSave() {
            if (Dbug.Is(DebugMode.INFO) && Debug) print("Object " + name + " has been saved.");
        }

        public virtual void JSOnSpawned() {
            if (Dbug.Is(DebugMode.INFO) && Debug) print("Object " + name + " has been spawend at " + transform.position + ".");
        }

        public virtual void JSOnDespawned()
        {
            if (Dbug.Is(DebugMode.INFO) && Debug) print("Object " + name + " has been despawend. Returning to pool.");
        }

        public virtual void JSOnLoad() {
            if (Dbug.Is(DebugMode.INFO) && Debug) print("Object " + name + " has been loaded.");
        }

        public virtual void JSOnPooled() {
            if (Dbug.Is(DebugMode.INFO) && Debug) print("Object " + name + " has been pooled.");
        }

        public virtual void JSOnNeeded() {
            if (Dbug.Is(DebugMode.INFO) && Debug) print("Calling " + name + " to despawn itself.");
        }
    }

}
