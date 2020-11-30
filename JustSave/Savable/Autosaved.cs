using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JustSave
{

    public class Autosaved : System.Attribute
    {
        string id;

        public Autosaved(string id)
        {
            this.id = id;
        }
    }

}
