using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace JustSave
{
    /// <summary>
    /// This class is responsible for taking a savefile and saving it somewhere
    /// </summary>
    public class FileManager
    {
        /// <summary>
        /// this method takes a savefile and a path and creates or overwrites an file at this path
        /// </summary>
        /// <param name="save">the content of the save</param>
        /// <param name="path">the path to save to</param>
        /// <returns>true if the saving was sucessfull, false if an error occured</returns>
        public bool SaveFile(Save save, string path)
        {
            try
            {
                BinaryFormatter myBinaryFormatter = new BinaryFormatter();
                FileStream myFileStream = File.Create(path);
                Debug.Log("_JS: Saved to " + path);
                myBinaryFormatter.Serialize(myFileStream, save);
                myFileStream.Close();
                return true;
            }
            catch (System.Exception)
            {
                Debug.LogError("Some error occured while saving file at " + path + ". Returning false.");
                return false;
                throw;
            }
        }

        /// <summary>
        /// this method takes the path to a savefile, loads the file from this path (if it exists) and returns it, deserialized
        /// </summary>
        /// <param name="path">the path under which a savefile should be found</param>
        /// <returns>the found savefile. returns null, if no savefile was found</returns>
        public Save LoadFile(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    Debug.Log("Reading file at " + path);
                    BinaryFormatter myBinaryFormatter = new BinaryFormatter();
                    FileStream myFileStream = File.Open(path, FileMode.Open);
                    Save mySave = (Save)myBinaryFormatter.Deserialize(myFileStream);
                    myFileStream.Close();
                    return mySave;
                }
                catch (System.Exception)
                {
                    Debug.LogError("Some error occured while loading file at " + path + ". Returning null pointer.");
                    return null;
                    throw;
                }
                
            }
            else
            {
                Debug.LogError("No file found at " + path + ".");
                return null;
            }
        }
    }
}
