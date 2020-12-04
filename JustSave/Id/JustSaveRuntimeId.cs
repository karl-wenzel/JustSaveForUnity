﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace JustSave
{
    public class JustSaveRuntimeId : JustSaveId
    {
        Guid id;
        ObjectPool ObjectPoolReference;
        int ObjectPoolIndex;
        string PrefabId;
        bool Spawned;

        public void SetId(Guid newId) {
            id = newId;
        }

        public Guid GetId() {
            return id;
        }

        public string GetPrefabId() {
            return PrefabId;
        }

        public override string GetSaveIdentifier()
        {
            return GetPrefabId() + "_" + id.ToString();
        }

        /// <summary>
        /// called when spawning the Prefab with this RuntimeId, every time when it leaves the pool
        /// </summary>
        public void Spawn() {
            Spawned = true;
            SetId(Guid.NewGuid());
        }

        /// <summary>
        /// setting up the RuntimeId-Component (Is called on Instantiation in the pool)
        /// </summary>
        /// <param name="newIndex">Index in the object Pool</param>
        /// <param name="newPool">Reference To the object Pool</param>
        /// <param name="PrefabId">Prefab Identifier, so the saving system knows, which kind of prefab this is</param>
        public void SetUp(int newIndex, ObjectPool newPool, String PrefabId) {
            ObjectPoolIndex = newIndex;
            ObjectPoolReference = newPool;
            this.PrefabId = PrefabId;
            SetId(Guid.NewGuid());
        }

        public void Despawn() {
            ObjectPoolReference.Despawn(ObjectPoolIndex);
            Spawned = false;
        }

        public override string ToString()
        {
            return id.ToString();
        }
    }

}
