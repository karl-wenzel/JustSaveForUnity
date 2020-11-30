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

        int CurrentSpawnIndex = 0;

        /// <summary>
        /// class to store information about objects in the pool and outside the pool
        /// </summary>
        public class PoolObjectInformation
        {
            public readonly GameObject ObjectReference;
            bool Spawned;
            int LastSpawnIndex;
            public Savable[] SavablesInObject;

            public int GetLastSpawn() {
                return LastSpawnIndex;
            }

            public void SetLastSpawn(int LastSpawnIndex) {
                this.LastSpawnIndex = LastSpawnIndex;
            }

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
                LastSpawnIndex = 0;

                //precalculating Savables on Start, so we dont have to search for them everytime we instantiate the prefab
                SavablesInObject = objectReference.GetComponentsInChildren<Savable>();
            }
        }

        // main datastructure for storing information about the pool
        public List<PoolObjectInformation> Pool = new List<PoolObjectInformation>();

        private void FillPool()
        {
            for (int i = Pool.Count; i < BasePoolSize; i++)
            {
                Pool.Add(new PoolObjectInformation(Instantiate(SpawnPrefab.gameObject, transform), false));
                Pool[i].ObjectReference.SetActive(false);
                Pool[i].ObjectReference.GetComponent<JustSaveRuntimeId>().SetUp(i, this, SpawnPrefabId);

                foreach (Savable mySavable in Pool[i].SavablesInObject)
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
        public bool Despawn(int PoolIndex) {
            if (GetPoolSize() > PoolIndex) {
                Despawn(Pool[PoolIndex]);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Despawns a Savable Object by given PoolObjectInformation
        /// </summary>
        /// <param name="PoolObject">The poolentry to use for despawning</param>
        public void Despawn(PoolObjectInformation PoolObject) {
            PoolObject.ObjectReference.SetActive(false);
            PoolObject.SetSpawned(false);
        }

        public int GetPoolSize() {
            return Pool.Count;
        }

        public GameObject Spawn(Vector3 Position)
        {

            //Force Spawning will always find an target
            if (ForceSpawning)
            {
                int OldestObjectIndex = CurrentSpawnIndex;
                int At = 0;


                for (int i = 0; i < Pool.Count; i++)
                {
                    if (Pool[i].GetLastSpawn() < OldestObjectIndex)
                    {
                        OldestObjectIndex = Pool[i].GetLastSpawn();
                        At = i;
                    }
                }
                Pool[At].SetLastSpawn(CurrentSpawnIndex+1);
                CurrentSpawnIndex++;
                return Spawn(Pool[At].ObjectReference, Pool[At].SavablesInObject, Position);
            }
            else
            {
                foreach (PoolObjectInformation thisPoolObjectInformation in Pool)
                {
                    if (!thisPoolObjectInformation.IsSpawned())
                    {
                        thisPoolObjectInformation.SetSpawned(true);

                        thisPoolObjectInformation.SetLastSpawn(CurrentSpawnIndex);
                        CurrentSpawnIndex++;

                        return Spawn(thisPoolObjectInformation.ObjectReference, thisPoolObjectInformation.SavablesInObject, Position);
                    }
                }
            }
            return null;
        }

        public GameObject Spawn(GameObject PrefabToSpawn, Savable[] SavablesInObject, Vector3 Position) {
            PrefabToSpawn.GetComponent<JustSaveRuntimeId>().Spawn();
            PrefabToSpawn.SetActive(true);
            PrefabToSpawn.transform.position = Position;

            //calling JSOnSpawned() on every Savable in the Prefab
            foreach (Savable mySavable in SavablesInObject)
            {
                mySavable.JSOnSpawned();
            }

            return PrefabToSpawn;
        }
    }

}
