using System;
using System.Collections.Generic;
using UnityEngine;

namespace JustSave
{
    [Serializable]
    public class JSDictionary<T> : JSSerializable where T : JSSerializable
    {
        Dictionary<string, T> savedValues;

        /// <summary>
        /// create a new empty serializable dictionary
        /// </summary>
        public JSDictionary()
        {
            savedValues = new Dictionary<string, T>();
        }

        /// <summary>
        /// creates a new serializable dictionary
        /// </summary>
        /// <param name="startDict">a predefined dictionary to include. The JSDictionary will include a deep copy of it.</param>
        public JSDictionary(Dictionary<string, T> startDict)
        {
            //creating a deep copy
            savedValues = new Dictionary<string, T>(startDict);
        }

        /// <summary>
        /// the method trys to add the value to the dictionary. If the keys wasn't already set, adds it and returns true.
        /// If the key was already set, overwrites it with the value and returns false.
        /// </summary>
        /// <param name="key">the key to save the value</param>
        /// <param name="value">the value to be saved</param>
        /// <returns></returns>
        public bool AddOrReplaceValueByKey(string key, T value)
        {
            if (savedValues.ContainsKey(key))
            {
                savedValues[key] = value;
                return false;
            }
            else
            {
                savedValues.Add(key, value);
                return true;
            }
        }

        /// <summary>
        /// removes a value from the JSDictionary. True if a value was removed, else returns false.
        /// </summary>
        /// <param name="key">the key to search for.</param>
        /// <returns>Returns true if a value was removed, else returns false.</returns>
        public bool RemoveValueByKey(string key)
        {
            if (savedValues.ContainsKey(key))
            {
                savedValues.Remove(key);
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// tries to get the value associated with a key. If no value was found, returns null
        /// </summary>
        /// <param name="key">the key associated with the wanted value</param>
        /// <returns>value of type T or null, if the key is no associated with any value</returns>
        public T GetValueByKey(string key)
        {
            if (savedValues.ContainsKey(key))
            {
                return savedValues[key];
            }
            else
            {
                Debug.LogError("key " + key + " not found");
                return null;
            }
        }

        /// <summary>
        /// tries to get the value associated with a key. If no value was found, creates a new Dictionary as value
        /// </summary>
        /// <param name="key">the key associated with the wanted value</param>
        /// <returns>value of type T or null, if the key is no associated with any value</returns>
        public T GetOrCreateValueByKey(string key)
        {
            if (savedValues.ContainsKey(key))
            {
                return savedValues[key];
            }
            else
            {
                savedValues.Add(key, new JSDictionary<JSSerializable>() as T);
                return GetValueByKey(key);
            }
        }

        /// <summary>
        /// Creates the dictionary as a string representation
        /// </summary>
        /// <returns>a string representation of the dictionary</returns>
        public override string ToString()
        {
            string result = "";
            foreach (String key in savedValues.Keys)
            {
                try
                {
                    result = result + key + ": " + GetValueByKey(key).ToString() + "; ";
                }
                catch (Exception)
                {
                    Debug.LogError("An error occured while getting value to key: " + key);
                    throw;
                }
            }
            return result;
        }
    }

}
