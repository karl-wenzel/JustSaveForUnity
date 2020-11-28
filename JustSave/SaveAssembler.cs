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
            JSDictionary<JSSerializable> Spawning = newSave.Spawning;
            JSDictionary<JSSerializable> Autosaves = newSave.Autosaves;

            Spawning.AddOrReplaceValueByKey("lol", new JSTriple(0, 0, 0));

            return newSave;
        }
    }

}
