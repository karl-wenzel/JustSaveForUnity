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
        //constructor
        private JustSaveManager() {
            //default values for path, filename and fileending
            path = Application.persistentDataPath + "/";
            fileName = "default";
            fileEnding = ".savefile";
        }

        public static readonly JustSaveManager Instance = new JustSaveManager();

        string path;
        string fileName;
        string fileEnding;

        /// <summary>
        /// call this method to save everything. Automatically assembles a save file and saves it.
        /// </summary>
        public void Save()
        {
            Debug.Log("Saving...");
        }

        /// <summary>
        /// call this method to load everything.
        /// </summary>
        public void Load()
        {
            Debug.Log("Loading...");
        }
    }

}
