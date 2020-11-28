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
        public virtual void JSOnSave() {
            print("Object " + name + " has been saved.");
        }

        public virtual void JSOnSpawned() {
            print("Object " + name + " has been spawend at " + transform.position + ".");
        }

        public virtual void JSOnDespawned()
        {
            print("Object " + name + " has been despawend. Returning to pool.");
        }

        public virtual void JSOnLoad() {
            print("Object " + name + " has been loaded.");
        }

        public virtual void JSOnPooled() {
            print("Object " + name + " has been pooled.");
        }

        public void JSDespawn() {
        }
    }

}
