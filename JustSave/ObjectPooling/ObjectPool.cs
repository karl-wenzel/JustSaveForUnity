using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JustSave
{
    /// <summary>
    /// An ObjectPool, which can be added to any gameobject. The pooled objects will be stored as inactive child objects of this gameobject
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        [Header("Type Settings")]
        [Tooltip("The Prefab, which the ObjectPool should spawn. Must have a JustSaveRuntimeId-Component. To work optimal, the Prefab should also have a component deriving from Savable.")]
        public JustSaveRuntimeId SpawnPrefab;
        [Tooltip("The Id with which the prefab is referred to by the Spawning system. Must be unique for a type of prefab.")]
        public string SpawnPrefabId;

        [Space]
        [Header("Pool Settings")]
        public int BasePoolSize;
        [Tooltip("If ForceSpawning is enabled, the pool will despawn the oldest spawned objects if it has no despawned objects anymore")]
        public bool ForceSpawning = false;

        int CurrentlyActiveObjects = 0;

        public int NotifyToDespawn = 0;

        /// <summary>
        /// Sets the NotifyToDespawn-value
        /// </summary>
        /// <param name="NotifyToDespawn">when an object in the pool is *n* objects away from beeing reused, it will be notified</param>
        public void SetNotifyToDespawn(int NotifyToDespawn)
        {
            if (GetPoolSize() < NotifyToDespawn)
            {
                if (Dbug.Is(DebugMode.ERROR)) Debug.LogError("Cant Set NotifyToDespawn to " + NotifyToDespawn + ", because the pool only has " + GetPoolSize() + " members.");
            }
            else
            {
                this.NotifyToDespawn = NotifyToDespawn;
            }
        }

        /// <summary>
        /// Returns the number of currently spawned objects.
        /// </summary>
        /// <returns>number of currently spawned objects</returns>
        public int GetCurrentlyActiveObjects() {
            return CurrentlyActiveObjects;
        }

        /// <summary>
        /// class to store information about objects in the pool and outside the pool
        /// </summary>
        public class PoolObjectInformation
        {
            public readonly GameObject ObjectReference;
            bool Spawned;
            public List<ISavable> SavablesInObject;

            public void SetSpawned(bool Spawned)
            {
                this.Spawned = Spawned;
            }

            public bool IsSpawned()
            {
                return Spawned;
            }

            public PoolObjectInformation(GameObject objectReference, bool spawned)
            {
                ObjectReference = objectReference;
                Spawned = spawned;

                SavablesInObject = new List<ISavable>();
                //precalculating Savables on Start, so we dont have to search for them everytime we instantiate the prefab
                foreach (Component m_comp in objectReference.GetComponentsInChildren<Component>())
                {
                    if (m_comp is ISavable)
                    {
                        SavablesInObject.Add(m_comp as ISavable);
                    }
                }
            }
        }

        // main datastructure for storing information about the pool. The object at the start will be spawned next
        public List<PoolObjectInformation> Pool = new List<PoolObjectInformation>();

        private void FillPool()
        {
            for (int i = Pool.Count; i < BasePoolSize; i++)
            {
                Pool.Add(new PoolObjectInformation(Instantiate(SpawnPrefab.gameObject, transform), false));
                Pool[i].ObjectReference.SetActive(false);
                Pool[i].ObjectReference.GetComponent<JustSaveRuntimeId>().SetUp(Pool[i], this, SpawnPrefabId);

                foreach (ISavable mySavable in Pool[i].SavablesInObject)
                {
                    mySavable.JSOnPooled();
                }
            }
        }

        private void Start()
        {
            FillPool();

            //registers this pool, incase the user created it in the unity editor. If the pool was created at runtime, the Register function will just not register it a second time.
            JustSaveManager.Instance.GetObjectPoolingManager().RegisterObjectPool(SpawnPrefabId, this);
        }



        /// <summary>
        /// Despawn a Savable Object by its PoolIndex. Returns false, if no corresponding entry could be found.
        /// </summary>
        /// <param name="PoolIndex">The Pool Index of the object</param>
        /// <returns>True if the Object was successfully despawned</returns>
        public bool Despawn(int PoolIndex)
        {
            if (GetPoolSize() > PoolIndex)
            {
                Despawn(Pool[PoolIndex]);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Despawns a Savable Object by given PoolObjectInformation
        /// </summary>
        /// <param name="PoolObject">The poolentry to use for despawning</param>
        public void Despawn(PoolObjectInformation PoolObject)
        {
            if (Dbug.Is(DebugMode.EXTENSIVE)) Debug.Log("Despawning Object " + PoolObject.ObjectReference.GetComponent<JustSaveRuntimeId>().GetSaveIdentifier());

            PoolObject.ObjectReference.SetActive(false);
            PoolObject.SetSpawned(false);
            PoolObject.ObjectReference.transform.position = Vector3.zero;

            CurrentlyActiveObjects--;
        }

        public int GetPoolSize()
        {
            return Pool.Count;
        }

        public GameObject Spawn(Vector3 Position)
        {

            //Force Spawning will always find an target
            if (ForceSpawning)
            {
                return Spawn(Pool[0], Pool[0].ObjectReference, Pool[0].SavablesInObject, Position);
            }
            else
            {
                foreach (PoolObjectInformation thisPoolObjectInformation in Pool)
                {
                    if (!thisPoolObjectInformation.IsSpawned())
                    {
                        thisPoolObjectInformation.SetSpawned(true);

                        return Spawn(thisPoolObjectInformation, thisPoolObjectInformation.ObjectReference, thisPoolObjectInformation.SavablesInObject, Position);
                    }
                }
            }
            return null;
        }

        GameObject Spawn(PoolObjectInformation PoolEntry, GameObject PrefabToSpawn, List<ISavable> SavablesInObject, Vector3 Position)
        {
            if (Dbug.Is(DebugMode.EXTENSIVE)) Debug.Log("Spawning Object " + PoolEntry.ObjectReference.GetComponent<JustSaveRuntimeId>().GetSaveIdentifier());

            PrefabToSpawn.GetComponent<JustSaveRuntimeId>().Spawn();
            PrefabToSpawn.transform.position = Position;
            PrefabToSpawn.SetActive(true);

            Pool.Add(PoolEntry);
            Pool.RemoveAt(0);

            //calling JSOnSpawned() on every Savable in the Prefab
            foreach (Savable mySavable in SavablesInObject)
            {
                mySavable.JSOnSpawned();
            }

            if (NotifyToDespawn > 0 && GetPoolSize() - CurrentlyActiveObjects <= NotifyToDespawn)
            {
                foreach (ISavable ISavableComponent in Pool[NotifyToDespawn].SavablesInObject)
                {
                    ISavableComponent.JSOnNeeded();
                }
            }

            CurrentlyActiveObjects++;
            return PrefabToSpawn;
        }
    }

}
