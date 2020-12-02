using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JustSave;

namespace JustSave
{

    /// <summary>
    /// this class contains only serializable data and represents a state of the application
    /// </summary>
    [Serializable]
    public class Save
    {
        
        JSDictionary<JSDictionary<JSSerializable>> Savedata;
        public readonly JSDictionary<JSSerializable> Runtime;
        public readonly JSDictionary<JSSerializable> Static;

        public Save() {
            Savedata = new JSDictionary<JSDictionary<JSSerializable>>();

            //this Dictionary will store all the information, about where pooled objects have been spawned
            Savedata.AddOrReplaceValueByKey("Runtime", new JSDictionary<JSSerializable>());
            Runtime = Savedata.GetValueByKey("Runtime");

            //this Dictionary will store all the object-specific information, it will get the fields marked "autosave" and map them to an object identifier
            Savedata.AddOrReplaceValueByKey("Static", new JSDictionary<JSSerializable>());
            Static = Savedata.GetValueByKey("Static");

        }

        public override string ToString()
        {
            return Savedata.ToString();
        }
    }
}
