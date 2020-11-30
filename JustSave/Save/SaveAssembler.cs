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
            }
            //TODO: continue working on this code
            //System.Attribute.GetCustomAttributes(JSRuntimeObjects[0]);
            //JSRuntimeObjects[0]
            

            return newSave;
        }
    }

}
