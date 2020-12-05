using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JustSave;
using System.Reflection;
using System;

namespace JustSave
{
    /// <summary>
    /// The SaveInterpreter is responsible for taking a Save and applying it to the Scene
    /// </summary>
    public class SaveInterpreter
    {

        /// <summary>
        /// Interprets a given save and applies the data to the Application
        /// </summary>
        /// <param name="source">The save to interpret</param>
        /// <returns>True if the method reached the end of the calculation without fatal errors</returns>
        public bool InterpretSave(Save source)
        {
            List<int> arrayList = new List<int>();
            //getting references to spawning and autosaves
            JSDictionary<JSSerializable> Runtime = source.Runtime;
            JSDictionary<JSSerializable> Static = source.Static;

            //use this dictionary to remember once scanned classes for later use
            Dictionary<Type, FieldInfo[]> RememberedFields = new Dictionary<Type, FieldInfo[]>();

            //preparing references and despawning runtime objects
            JustSaveId[] JSManagedObjects = UnityEngine.Object.FindObjectsOfType<JustSaveId>();
            List<JustSaveSceneId> JSManagedSceneObjects = new List<JustSaveSceneId>();
            foreach (JustSaveId JustSaveManagedObject in JSManagedObjects)
            {
                if (JustSaveManagedObject is JustSaveRuntimeId)
                {
                    ((JustSaveRuntimeId)JustSaveManagedObject).Despawn(); //despawns every runtime object
                }
                else
                {
                    JSManagedSceneObjects.Add((JustSaveSceneId)JustSaveManagedObject); //saves every scene object reference
                }
            }
            if (Dbug.Is(DebugMode.DEBUG)) Debug.Log("Save Interpreter despawned old runtime objects");
            Debug.DebugBreak();

            //loading scene objects
            foreach (JustSaveSceneId IdObj in JSManagedSceneObjects)
            {
                SyncObject(source, false, IdObj, RememberedFields);
            }

            if (Dbug.Is(DebugMode.DEBUG)) Debug.Log("Save Interpreter synced scene objects");


            //iterating through spawned objects
            string[] idInfo = new string[2];
            foreach (Tuple<String, JSSerializable> RuntimeObjectInfo in Runtime.GetValuePairs())
            {
                idInfo = RuntimeObjectInfo.Item1.Split('_');
                GameObject spawnedPrefab = JustSaveManager.Instance.Spawn(idInfo[0], Vector3.zero);
                JustSaveRuntimeId spawnedIdObj = spawnedPrefab.GetComponent<JustSaveRuntimeId>();
                spawnedIdObj.SetId(Guid.Parse(idInfo[1]));
                SyncObject(source, true, spawnedPrefab.GetComponent<JustSaveRuntimeId>(), RememberedFields);
            }


            if (Dbug.Is(DebugMode.DEBUG))
            {
                Debug.Log("Save Interpreter spawned and synced runtime objects");
                Debug.Log("Save Interpreter finished loading Save");
            }

            return true;
        }

        public void SyncObject(Save source, bool Runtime, JustSaveId IdObj, Dictionary<Type, FieldInfo[]> RememberedFields)
        {
            GameObject Search = IdObj.gameObject;
            Component[] Components = Search.GetComponentsInChildren<Component>();
            List<Attribute> Attributes = new List<Attribute>();

            FieldInfo[] FieldInfos;

            //getting the attributes
            foreach (Component m_Comp in Components)
            {
                if (!RememberedFields.TryGetValue(m_Comp.GetType(), out FieldInfos))
                {
                    FieldInfos = m_Comp.GetType().GetFields();
                    RememberedFields.Add(m_Comp.GetType(), FieldInfos);
                }

                foreach (FieldInfo Field in FieldInfos)
                {
                    if (Attribute.IsDefined(Field, typeof(Autosaved)))
                    {
                        Type AutosaveFieldType = Field.FieldType;

                        if (AutosaveFieldType.IsSerializable)
                        {
                            object extractedValue = ((JSBasic)GetValue(source, Runtime, IdObj.GetSaveIdentifier(), m_Comp.GetType().Name, Field.Name)).GetValue();
                            Field.SetValue(m_Comp, extractedValue);
                        }
                        // support for unitys vector-types
                        else if (AutosaveFieldType == typeof(Vector2))
                        {
                            object extractedValue = ((JSNTuple)GetValue(source, Runtime, IdObj.GetSaveIdentifier(), m_Comp.GetType().Name, Field.Name)).GetVector2();
                            Field.SetValue(m_Comp, extractedValue);
                        }
                        else if (AutosaveFieldType == typeof(Vector3))
                        {
                            object extractedValue = ((JSNTuple)GetValue(source, Runtime, IdObj.GetSaveIdentifier(), m_Comp.GetType().Name, Field.Name)).GetVector3();
                            Field.SetValue(m_Comp, extractedValue);
                        }
                        else if (AutosaveFieldType == typeof(Vector4))
                        {
                            object extractedValue = ((JSNTuple)GetValue(source, Runtime, IdObj.GetSaveIdentifier(), m_Comp.GetType().Name, Field.Name)).GetVector4();
                            Field.SetValue(m_Comp, extractedValue);
                        }
                        else if (AutosaveFieldType == typeof(Quaternion))
                        {
                            object extractedValue = ((JSNTuple)GetValue(source, Runtime, IdObj.GetSaveIdentifier(), m_Comp.GetType().Name, Field.Name)).GetQuaternion();
                            Field.SetValue(m_Comp, extractedValue);
                        }
                    }
                }

                //calling JSOnLoad on every class implementing the ISavable interface
                if (m_Comp is ISavable)
                {
                    ((ISavable)m_Comp).JSOnLoad();
                }
            }
        }

        /// <summary>
        /// getting a value from the given savefile from the given path
        /// </summary>
        /// <param name="save">The save to get the value from</param>
        /// <param name="Runtime">True if the following Id is runtime, false if it is a scene id</param>
        /// <param name="ObjectIdentifier">The Id of the object to find</param>
        /// <param name="ComponentIdentifier">The Identifier of the Component from which this field will be saved</param>
        /// <param name="FieldName">The Name of the Field from which the value was extracted</param>
        /// <param name="Value">The Value to save. Must derive from JSSerializable</param>
        public JSSerializable GetValue(Save save, bool Runtime, string ObjectIdentifier, string ComponentIdentifier, string FieldName)
        {
            JSSerializable result = null;

            result = ((JSDictionary<JSSerializable>)((JSDictionary<JSSerializable>)save.GetByKey(Runtime ? "Runtime" : "Static")
                .GetValueByKey(ObjectIdentifier)).GetValueByKey(ComponentIdentifier)).GetValueByKey(FieldName);

            if (result == null)
            {
                if (Dbug.Is(DebugMode.WARN)) Debug.LogWarning("There was a problem getting a value for the Autosave field at: "
                    + ObjectIdentifier + " " + ComponentIdentifier + " " + FieldName);
            }
            return result;
        }
    }

}
