using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JustSave;
using System.Reflection;

namespace JustSave
{


    public class SaveAssembler
    {
        /// <summary>
        /// assembles a new JustSave.Save from the current application and returns this Save.
        /// </summary>
        /// <returns>the assembled Save</returns>
        public Save GetCurrentSave()
        {
            Save newSave = new Save();

            //getting references to spawning and autosaves
            JSDictionary<JSSerializable> Runtime = newSave.Runtime;
            JSDictionary<JSSerializable> Static = newSave.Static;

            JustSaveId[] JSManagedObjects = Object.FindObjectsOfType<JustSaveId>();

            bool IsRuntime;

            foreach (JustSaveId IdObj in JSManagedObjects)
            {
                //its either a JustSaveRuntime Id
                if (IdObj is JustSaveRuntimeId)
                {
                    IsRuntime = true;
                }
                //or a SceneId
                else
                {
                    IsRuntime = false;
                }
                Debug.Log("Getting Autosave Fields in " + IdObj.gameObject.name);
                GameObject Search = IdObj.gameObject;
                Component[] Components = Search.GetComponentsInChildren<Component>();
                List<System.Attribute> Attributes = new List<System.Attribute>();

                Debug.Log("Checking " + Components.Length + " Components in " + IdObj.gameObject.name);
                //getting the attributes
                foreach (Component m_Comp in Components)
                {
                    FieldInfo[] FieldInfos = m_Comp.GetType().GetFields();

                    foreach (FieldInfo Field in FieldInfos)
                    {
                        if (System.Attribute.IsDefined(Field, typeof(Autosaved)))
                        {
                            Debug.Log("Found Savable Field: " + Field.Name + " in Component " + m_Comp.GetType().Name);
                            Debug.Log("Field Has Type: " + Field.FieldType);
                            System.Type AutosaveFieldType = Field.FieldType;

                            System.Type[] SerializableTypes = { typeof(System.Int32) };

                            if (AutosaveFieldType == typeof(int))
                            {
                                SaveValue(newSave, IsRuntime, IdObj.GetSaveIdentifier(), m_Comp.GetType().Name, Field.Name, 
                                    new JSInt((int)Field.GetValue(m_Comp as object)));
                            }
                            else if (AutosaveFieldType == typeof(float))
                            {
                                SaveValue(newSave, IsRuntime, IdObj.GetSaveIdentifier(), m_Comp.GetType().Name, Field.Name, 
                                    JSFloat.GetJSFloat((float)Field.GetValue(m_Comp as object)));
                            }
                            else if (AutosaveFieldType == typeof(Vector3))
                            {
                                SaveValue(newSave, IsRuntime, IdObj.GetSaveIdentifier(), m_Comp.GetType().Name, Field.Name, 
                                    JSTriple.GetJSTriple((Vector3)Field.GetValue(m_Comp as object)));
                            }
                            else if (AutosaveFieldType == typeof(Vector2))
                            {
                                SaveValue(newSave, IsRuntime, IdObj.GetSaveIdentifier(), m_Comp.GetType().Name, Field.Name, 
                                    JSNTuple.GetFromVector2((Vector2)Field.GetValue(m_Comp as object)));
                            }
                            else if (AutosaveFieldType == typeof(Vector4))
                            {
                                SaveValue(newSave, IsRuntime, IdObj.GetSaveIdentifier(), m_Comp.GetType().Name, Field.Name, 
                                    JSNTuple.GetFromVector4((Vector4)Field.GetValue(m_Comp as object)));
                            }
                            else if (AutosaveFieldType == typeof(Quaternion))
                            {
                                SaveValue(newSave, IsRuntime, IdObj.GetSaveIdentifier(), m_Comp.GetType().Name, Field.Name, 
                                    JSNTuple.GetFromQuaternion((Quaternion)Field.GetValue(m_Comp as object)));
                            }
                            else if (AutosaveFieldType == typeof(string))
                            {
                                SaveValue(newSave, IsRuntime, IdObj.GetSaveIdentifier(), m_Comp.GetType().Name, Field.Name, 
                                    JSBasic.GetFromObject(Field.GetValue(m_Comp as object)));
                            }
                            else if (AutosaveFieldType is System.Runtime.Serialization.ISerializable)
                            {
                                SaveValue(newSave, IsRuntime, IdObj.GetSaveIdentifier(), m_Comp.GetType().Name, Field.Name, 
                                    JSBasic.GetFromObject(Field.GetValue(m_Comp as object)));
                            }
                        }
                    }
                }
            }
            Debug.Log(newSave.ToString());
            return newSave;
        }

        /// <summary>
        /// manipulate the given savefile by setting the value at the given path
        /// </summary>
        /// <param name="save">The save to manipulate</param>
        /// <param name="Runtime">True if the following Id is runtime, false if it is a scene id</param>
        /// <param name="ObjectIdentifier">The Id of the object to find</param>
        /// <param name="ComponentIdentifier">The Identifier of the Component from which this field will be saved</param>
        /// <param name="FieldName">The Name of the Field from which the value was extracted</param>
        /// <param name="Value">The Value to save. Must derive from JSSerializable</param>
        public void SaveValue(Save save, bool Runtime, string ObjectIdentifier, string ComponentIdentifier, string FieldName, JSSerializable Value)
        {
            JSDictionary<JSSerializable> ToSave = Runtime ? save.Runtime : save.Static;

            //following the path through the Savefile, creating additional Entries if necessary, then adding or replacing the value to the field name
            ((JSDictionary<JSSerializable>)(((JSDictionary<JSSerializable>)(ToSave.GetOrCreateValueByKey(ObjectIdentifier)))
                .GetOrCreateValueByKey(ComponentIdentifier))).AddOrReplaceValueByKey(FieldName, Value);
        }
    }


}
