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
        /// <returns>True, if no binding for this key existed and it was added. False, if a value was replaced. Use this to determine if you are overwriting values</returns>
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
                if (Dbug.Is(DebugMode.ERROR))
                {
                    Debug.LogError("key " + key + " not found");
                    Debug.LogError("Available keys would be: ");

                    foreach (string debug_key in savedValues.Keys)
                    {
                        Debug.LogError(debug_key);
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Returning the content of the JSDictionary in an array of the form of a Tuple with a string as the first value and the corresponding JSSerializable as the second value.
        /// </summary>
        /// <returns>The Tuple[]-representation of the JSDictionary</returns>
        public Tuple<string, JSSerializable>[] GetValuePairs() {
            Tuple<string, JSSerializable>[] result = new Tuple<string, JSSerializable>[savedValues.Keys.Count];
            int i = 0;
            foreach (string key in savedValues.Keys)
            {
                result[i] = new Tuple<string, JSSerializable>(key, GetValueByKey(key));
                i++;
            }
            return result;
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
            result = result + "Count: " + savedValues.Keys.Count + " ";

            foreach (String key in savedValues.Keys)
            {
                try
                {
                    result = result + key + ": " + GetValueByKey(key).ToString() + "; ";
                }
                catch (Exception)
                {
                    if (Dbug.Is(DebugMode.WARN)) Debug.LogError("An error occured while getting value to key: " + key + ". Could Therefore not print JSDictionary to console.");
                    throw;
                }
            }
            return result;
        }

        public string ToShortString() {
            string result = "";
            result = result + " " + (savedValues.Keys.Count>6 ? savedValues.Count+":" : "");

            foreach (String key in savedValues.Keys)
            {
                try
                {
                    result = result + (key.Length <= 20 ? key.Split('_')[0].Replace("JustSave","") : key.Substring(0, 20).Split('_')[0]).Replace("JustSave", "") + ((GetValueByKey(key) is JSDictionary<JSSerializable>) ? "(" + (GetValueByKey(key) as JSDictionary<JSSerializable>).ToShortString()+") " : "") + " ";
                }
                catch (Exception)
                {
                    if (Dbug.Is(DebugMode.WARN)) Debug.LogError("An error occured while getting value to key: " + key + ". Could Therefore not print JSDictionary to console.");
                    throw;
                }
            }
            return result;
        }
    }

}
