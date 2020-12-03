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
        public bool InterpretSave(Save source)
        {
            //getting references to spawning and autosaves
            JSDictionary<JSSerializable> Runtime = source.Runtime;
            JSDictionary<JSSerializable> Static = source.Static;

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

            //loading scene objects
            foreach (JustSaveSceneId IdObj in JSManagedSceneObjects)
            {
                Debug.Log("Getting Autosave Fields in " + IdObj.gameObject.name);
                GameObject Search = IdObj.gameObject;
                Component[] Components = Search.GetComponentsInChildren<Component>();
                List<Attribute> Attributes = new List<Attribute>();

                Debug.Log("Checking " + Components.Length + " Components in " + IdObj.gameObject.name);

                //getting the attributes
                foreach (Component m_Comp in Components)
                {
                    FieldInfo[] FieldInfos = m_Comp.GetType().GetFields();

                    foreach (FieldInfo Field in FieldInfos)
                    {
                        if (Attribute.IsDefined(Field, typeof(Autosaved)))
                        {
                            Debug.Log("Found Loadable Field: " + Field.Name + " in Component " + m_Comp.GetType().Name);
                            Debug.Log("Field Has Type: " + Field.FieldType);
                            Type AutosaveFieldType = Field.FieldType;

                            if (AutosaveFieldType.IsSerializable)
                            {
                                object extractedValue = ((JSBasic)GetValue(source, false, IdObj.GetSaveIdentifier(), m_Comp.GetType().Name, Field.Name)).GetValue();
                                Field.SetValue(m_Comp, extractedValue);
                            }
                            // support for unitys vector-types
                            else if (AutosaveFieldType == typeof(Vector2))
                            {
                                object extractedValue = ((JSNTuple)GetValue(source, false, IdObj.GetSaveIdentifier(), m_Comp.GetType().Name, Field.Name)).GetVector2();
                                Field.SetValue(m_Comp, extractedValue);
                            }
                            else if (AutosaveFieldType == typeof(Vector3))
                            {
                                object extractedValue = ((JSNTuple)GetValue(source, false, IdObj.GetSaveIdentifier(), m_Comp.GetType().Name, Field.Name)).GetVector3();
                                Field.SetValue(m_Comp, extractedValue);
                            }
                            else if (AutosaveFieldType == typeof(Vector4))
                            {
                                object extractedValue = ((JSNTuple)GetValue(source, false, IdObj.GetSaveIdentifier(), m_Comp.GetType().Name, Field.Name)).GetVector4();
                                Field.SetValue(m_Comp, extractedValue);
                            }
                        }
                    }
                }
            }
            return true;
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

            return result;
        }
    }

}
