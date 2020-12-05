using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JustSave;

namespace JustSave
{

    /// <summary>
    /// the ObjectPoolingManager is responsible for keeping track of instantiated objects, so that they later can be saved properly
    /// </summary>
    public class ObjectPoolingManager
    {
        readonly JustSaveManager myJustSaveManager;
        List<PoolKey> ObjectPoolingList = new List<PoolKey>();

        public ObjectPoolingManager()
        {
            myJustSaveManager = JustSaveManager.Instance;
        }

        public class PoolKey
        {

            public string key;
            public ObjectPool pool;

            public PoolKey(string key, ObjectPool pool)
            {
                this.key = key;
                this.pool = pool;
            }
        }


        /// <summary>
        /// Spawning an object with object pooling. If all pools with the given Id are empty, returns false.
        /// </summary>
        /// <param name="PrefabId">The Id of the prefab to be spawned. Id should match the Id of some object pool.</param>
        /// <param name="Position">The position at which the prefab shall be spawned</param>
        /// <returns>True if an object was spawned. False if no object was spawned</returns>
        public GameObject Spawn(string PrefabId, Vector3 Position) {
            if (PrefabId == null || Position == null) {
                if (Dbug.Is(DebugMode.ERROR)) Debug.LogWarning("Spawning was called on ObjectPoolingManager with Null-Pointer arguments.");
                return null;
            }
            foreach (PoolKey myPoolKey in ObjectPoolingList)
            {
                if (myPoolKey.key.Equals(PrefabId)) {
                    return myPoolKey.pool.Spawn(Position);
                }
            }
            if (Dbug.Is(DebugMode.ERROR))
            {
                Debug.LogWarning("The ObjectPoolingManager found no ObjectPool corresponding to the id: " + PrefabId);
                Debug.LogWarning("List of all registered ObjectPools:");
                foreach (PoolKey myPoolKey in ObjectPoolingList)
                {
                    Debug.LogWarning(myPoolKey.key);
                }
            }
            return null;
        }

        public bool RegisterObjectPool(string PrefabId, ObjectPool Pool) {
            foreach (PoolKey myPoolKey in ObjectPoolingList)
            {
                if (Pool == myPoolKey.pool) return false;
            }
            ObjectPoolingList.Add(new PoolKey(PrefabId, Pool));
            return true;
        }
    }

}
