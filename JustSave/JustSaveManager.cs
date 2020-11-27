using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JustSave
{
    /// <summary>
    /// This is the main class of the JustSave package.
    /// </summary>
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

        public void Save()
        {
            Debug.Log("Saving...");
        }

        public void Load()
        {
            Debug.Log("Loading...");
        }
    }
}
