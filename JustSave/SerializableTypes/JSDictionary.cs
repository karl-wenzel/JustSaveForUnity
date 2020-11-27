using System;
using System.Collections.Generic;
using UnityEngine;

namespace JustSave
{
    [Serializable]
    public class JSDictionary<T> where T : JSSerializable
    {
        Dictionary<string, T> savedValues;

        public JSDictionary()
        {
            savedValues = new Dictionary<string, T>();
        }

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
        public bool AddOrReplaceIntegerByKey(string key, T value)
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

        public bool RemoveIntegerByKey(string key)
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
    }

}
