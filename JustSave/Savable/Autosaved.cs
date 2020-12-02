using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JustSave
{
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class Autosaved : System.Attribute
    {
        readonly string message;

        public string GetMessage() {
            return message;
        }

        public Autosaved(string message)
        {
            this.message = message;
        }

        public Autosaved() {
            this.message = "";
        }
    }

}
