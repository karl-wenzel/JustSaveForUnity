using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JustSave;
using System.Reflection;
using System;

namespace JustSave
{


    public class SaveAssembler
    {
        
        bool DebugThis;

        public SaveAssembler(bool DebugThis) {
            this.DebugThis = DebugThis;
        }

        /// <summary>
        /// assembles a new JustSave.Save from the current application and returns this Save.
        /// </summary>
        /// <returns>the assembled Save</returns>
        /// 
        public Save GetCurrentSave()
        {
            Save newSave = new Save();

            int OverwriteCounter = 0;

            //getting references to spawning and autosaves
            JSDictionary<JSSerializable> Runtime = newSave.Runtime;
            JSDictionary<JSSerializable> Static = newSave.Static;

            JustSaveId[] JSManagedObjects = UnityEngine.Object.FindObjectsOfType<JustSaveId>();

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
                GameObject Search = IdObj.gameObject;
                Component[] Components = Search.GetComponentsInChildren<Component>();
                List<Attribute> Attributes = new List<Attribute>();

                //getting the attributes
                foreach (Component m_Comp in Components)
                {
                    //calling JSOnSave on every class implementing the ISavable interface
                    if (m_Comp is ISavable) {
                        ((ISavable)m_Comp).JSOnSave();
                    }

                    FieldInfo[] FieldInfos = m_Comp.GetType().GetFields();

                    foreach (FieldInfo Field in FieldInfos)
                    {
                        if (Attribute.IsDefined(Field, typeof(Autosaved)))
                        {
                            Type AutosaveFieldType = Field.FieldType;

                            // already serializable Types
                            if (AutosaveFieldType.IsSerializable)
                            {
                                if (!(SaveValue(newSave, IsRuntime, IdObj.GetSaveIdentifier(), m_Comp.GetType().Name, Field.Name,
                                    JSBasic.GetFromObject(Field.GetValue(m_Comp))))) OverwriteCounter++;
                            }
                            // support for unitys vector-types
                            else if (AutosaveFieldType == typeof(Vector2))
                            {
                                if (!(SaveValue(newSave, IsRuntime, IdObj.GetSaveIdentifier(), m_Comp.GetType().Name, Field.Name,
                                    JSNTuple.GetFromVector2((Vector2)Field.GetValue(m_Comp))))) OverwriteCounter++;
                            }
                            else if (AutosaveFieldType == typeof(Vector3))
                            {
                                if (!(SaveValue(newSave, IsRuntime, IdObj.GetSaveIdentifier(), m_Comp.GetType().Name, Field.Name,
                                    JSNTuple.GetFromVector3((Vector3)Field.GetValue(m_Comp))))) OverwriteCounter++;
                            }
                            else if (AutosaveFieldType == typeof(Vector4))
                            {
                                if (!(SaveValue(newSave, IsRuntime, IdObj.GetSaveIdentifier(), m_Comp.GetType().Name, Field.Name,
                                    JSNTuple.GetFromVector4((Vector4)Field.GetValue(m_Comp))))) OverwriteCounter++;
                            }
                            else if (AutosaveFieldType == typeof(Quaternion))
                            {
                                if (!(SaveValue(newSave, IsRuntime, IdObj.GetSaveIdentifier(), m_Comp.GetType().Name, Field.Name,
                                    JSNTuple.GetFromQuaternion((Quaternion)Field.GetValue(m_Comp))))) OverwriteCounter++;
                            }
                            // no support
                            else {
                                if (DebugThis) Debug.LogWarning("Field " + Field.Name + " of Type " + AutosaveFieldType.Name + " is not serializable and will be skipped.");
                            }
                        }
                    }
                }
            }
            if (DebugThis) {
                Debug.Log("______Assembled Save______");
                Debug.Log(newSave.ToString());
                Debug.Log("__________________________");
                Debug.Log("______Short Form Save______");
                Debug.Log(newSave.ToShortString());
                Debug.Log("__________________________");
                if (OverwriteCounter == 0) Debug.Log("Overwritten: " + OverwriteCounter + " Fields. If this number is greater than 0, you should look into this.");
                else Debug.LogWarning("Overwritten: " + OverwriteCounter + " Fields. If this number is greater than 0, you should look into this.");
            }
            
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
        public bool SaveValue(Save save, bool Runtime, string ObjectIdentifier, string ComponentIdentifier, string FieldName, JSSerializable Value)
        {
            JSDictionary<JSSerializable> ToSave = Runtime ? save.Runtime : save.Static;

            //following the path through the Savefile, creating additional Entries if necessary, then adding or replacing the value to the field name
            return ((JSDictionary<JSSerializable>)(((JSDictionary<JSSerializable>)(ToSave.GetOrCreateValueByKey(ObjectIdentifier)))
                .GetOrCreateValueByKey(ComponentIdentifier))).AddOrReplaceValueByKey(FieldName, Value);
        }
    }


}
