using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JustSave;

namespace JustSave
{

    public class SaveAssembler
    {
        /// <summary>
        /// assembles a new JustSave.Save from the current application and returns this Save.
        /// </summary>
        /// <returns>the assembled Save</returns>
        public Save GetCurrentSave() {
            Save newSave = new Save();

            //getting references to spawning and autosaves
            JSDictionary<JSSerializable> Runtime = newSave.Runtime;
            JSDictionary<JSSerializable> Static = newSave.Static;

            Runtime.AddOrReplaceValueByKey("lol", new JSTriple(0, 0, 0));



            JustSaveId[] JSManagedObjects = Object.FindObjectsOfType<JustSaveId>();

            List<JustSaveRuntimeId> JSRuntimeObjects = new List<JustSaveRuntimeId>();
            List<JustSaveSceneId> JSSceneObjects = new List<JustSaveSceneId>();
            foreach (JustSaveId IdObj in JSManagedObjects)
            {
                //its either a JustSaveRuntime Id
                if (IdObj is JustSaveRuntimeId)
                {
                    JSRuntimeObjects.Add(IdObj as JustSaveRuntimeId);
                }
                //or a SceneId
                else {
                    JSSceneObjects.Add(IdObj as JustSaveSceneId);
                }
                GetAutosaveFields(IdObj);
            }

            return newSave;
        }


        public void GetAutosaveFields(JustSaveId ObjectId)
        {
            Debug.Log("Getting Autosave Fields in " + ObjectId.gameObject.name);
            GameObject Search = ObjectId.gameObject;
            Component[] Components = ObjectId.GetComponentsInChildren<Component>();
            List<System.Attribute> Attributes = new List<System.Attribute>();

            //getting the attributes
            foreach (Component m_Obj in Components)
            {
                Debug.Log("Checking Component " + m_Obj.GetType().Name);
                object[] m_Attributes = m_Obj.GetType().GetCustomAttributes(typeof(Autosaved), true);
                foreach (object m_Attribute in m_Attributes)
                {
                    Attributes.Add(m_Attribute as Autosaved);
                }
            }

            Debug.Log("Got List of " + Attributes.Count + " Attributes");

            //iterating over the Attributes
            Autosaved m_Autosaved;
            foreach (System.Attribute m_Attribute in Attributes)
            {
                Debug.Log("Found Attribute " + m_Attribute.GetType().Name);
                m_Autosaved = (Autosaved)m_Attribute;
                Debug.Log("Attribute Type: " + m_Autosaved.GetType().ToString());
            }
        }
    }


}
