
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JustSave
{



    /// <summary>
    /// This is the main class of the JustSave package.
    /// </summary>
    /// <example>An example how to get a reference to the Manager: 
    /// <code>
    /// using JustSave;
    /// 
    /// public class ExampleClass {
    ///     public void GetReference () {
    ///         JustSaveManager myJustSaveManager = JustSaveManager.Instance;
    ///     }
    /// }
    /// </code>
    /// </example>
    public sealed class JustSaveManager
    {
        public static readonly JustSaveManager Instance = new JustSaveManager();

        FileManager myFileManager = new FileManager();
        SaveAssembler mySaveAssembler = new SaveAssembler();
        SaveInterpreter mySaveInterpreter = new SaveInterpreter();

        //the object pooling manager can be acessed from elsewhere
        ObjectPoolingManager myObjectPoolingManager = new ObjectPoolingManager();

        public ObjectPoolingManager GetObjectPoolingManager()
        {
            return myObjectPoolingManager;
        }

        public void ToggleDebugMode(DebugMode Mode)
        {
            Dbug.GlobalLevel = Mode;
        }

        //constructor
        private JustSaveManager()
        {
            //default values for path, filename and fileending
            ResetSavePath();
            fileName = "default";
            fileEnding = ".savefile";
        }

        public void SetFileName(string newFileName)
        {
            if (newFileName != "" && newFileName != null)
            {
                fileName = newFileName;
                if (Dbug.Is(DebugMode.INFO)) Debug.Log("Filename set to " + newFileName + ". Complete path is now: " + path + fileName + fileEnding);
            }
            else if (Dbug.Is(DebugMode.ERROR)) Debug.LogError("Filename can not be empty or null.");
        }

        public void SetFileEnding(string newFileEnding)
        {
            if (newFileEnding != null)
            {
                fileEnding = newFileEnding;
                if (Dbug.Is(DebugMode.INFO)) Debug.Log("Fileending set to " + newFileEnding + ". Complete path is now: " + path + fileName + fileEnding);
            }
            else if (Dbug.Is(DebugMode.ERROR)) Debug.LogError("Fileending can not be null.");
        }

        public void SetFilePath(string newFilePath)
        {
            if (newFilePath != null && newFilePath != "")
            {
                path = newFilePath;
                if (Dbug.Is(DebugMode.INFO)) Debug.Log("Filepath set to " + newFilePath + ". Complete path is now: " + path + fileName + fileEnding);
            }
            else if (Dbug.Is(DebugMode.ERROR)) Debug.LogError("Filepath can not be null or empty.");
        }

        public void ResetSavePath() {
            path = Application.persistentDataPath + System.IO.Path.DirectorySeparatorChar;
            if (Dbug.Is(DebugMode.INFO)) Debug.Log("Reset savepath. Complete path is now: " + path + fileName + fileEnding);
        }

        string path;
        string fileName;
        string fileEnding;

        /// <summary>
        /// Spawning an object with object pooling. If all pools with the given Id are empty, returns null.
        /// </summary>
        /// <param name="PrefabId">The Id of the prefab to be spawned. Id should match the Id of some object pool.</param>
        /// <param name="Position">The position at which the prefab shall be spawned</param>
        /// <returns>The Gameobject, that was spawned or null, if no Gameobject could be spawned</returns>
        public GameObject Spawn(string PrefabId, Vector3 Position)
        {
            return myObjectPoolingManager.Spawn(PrefabId, Position);
        }

        /// <summary>
        /// Creating a new Object Pool from script. Returns null, if no pool was created.
        /// </summary>
        /// <param name="PrefabToSpawn">The Prefab to spawn. Must have a JustSaveRuntimeId-component</param>
        /// <param name="PrefabId">A prefab id. Should be unique among your other object-pooling-ids</param>
        /// <param name="BasePoolSize">The base size to which the pool should be filled</param>
        /// <param name="Mode">How the pool should operate if you want to spawn something and the pool is empty.</param>
        /// <param name="UseForceSpawning">When set to true, the pool will automatically despawn old objects when no more objects are available</param>
        /// <returns></returns>
        public ObjectPool CreateObjectPool(GameObject PrefabToSpawn, string PrefabId, int BasePoolSize, PoolingMode Mode, int NotifyToDespawn)
        {
            if (BasePoolSize < 0 || PrefabId == null || PrefabId == "") return null;
            GameObject newPool = new GameObject("ObjectPoolFor" + PrefabToSpawn, typeof(ObjectPool));
            ObjectPool newPoolComponent = newPool.GetComponent<ObjectPool>();
            newPoolComponent.SpawnPrefabId = PrefabId;
            JustSaveRuntimeId SavablePrefabToSpawn = PrefabToSpawn.GetComponent<JustSaveRuntimeId>();
            if (SavablePrefabToSpawn == null) return null;
            newPoolComponent.SpawnPrefab = SavablePrefabToSpawn;
            newPoolComponent.BasePoolSize = BasePoolSize;
            newPoolComponent.SetPoolingMode(Mode);
            newPoolComponent.NotifyToDespawn = NotifyToDespawn <= BasePoolSize ? NotifyToDespawn : BasePoolSize;
            myObjectPoolingManager.RegisterObjectPool(PrefabId, newPoolComponent);
            return newPoolComponent;
        }

        /// <summary>
        /// call this method to save everything. Automatically assembles a save file and saves it.
        /// </summary>
        public void Save()
        {
            if (Dbug.Is(DebugMode.INFO)) Debug.Log("Saving...");
            Save newSave = mySaveAssembler.GetCurrentSave();
            if (myFileManager.SaveFile(newSave, path + fileName + fileEnding) && Dbug.Is(DebugMode.INFO)) Debug.Log("File saved successfully");
        }

        /// <summary>
        /// call this to get a Save-file representing the current gamestate, without really saving anything to the hard drive
        /// </summary>
        /// <returns>A save-representation of the current state</returns>
        public Save GetSaveFromCurrentState()
        {
            return mySaveAssembler.GetCurrentSave();
        }

        /// <summary>
        /// interpret a given save and apply it to the application
        /// </summary>
        /// <param name="source">the save to interpret</param>
        public void InterpretThisSave(Save source)
        {
            mySaveInterpreter.InterpretSave(source);
        }

        /// <summary>
        /// call this method to load everything.
        /// </summary>
        public void Load()
        {
            if (Dbug.Is(DebugMode.INFO)) Debug.Log("Loading...");
            Save loadedSave = myFileManager.LoadFile(path + fileName + fileEnding);
            if (Dbug.Is(DebugMode.DEBUG))
            {
                Debug.Log("Save to load: " + loadedSave.ToString());
                Debug.Log("Shortform: " + loadedSave.ToShortString());
            }
            mySaveInterpreter.InterpretSave(loadedSave);
            if (Dbug.Is(DebugMode.INFO)) Debug.Log("File loaded.");

        }
    }

}
