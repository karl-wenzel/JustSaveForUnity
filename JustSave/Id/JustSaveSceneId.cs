using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JustSave;

namespace JustSave
{

    public class JustSaveSceneId : JustSaveId
    {
        private string id;

        //secondary Id in the rare case, that 2 scene objects are at the exact same position
        private string secId;

        private void Start()
        {
            id = gameObject.GetComponent<Transform>().position.x + gameObject.GetComponent<Transform>().position.y + gameObject.GetComponent<Transform>().position.z + "";
            secId = gameObject.name;
        }

        /// <summary>
        /// generated on start from gameobject position
        /// </summary>
        public string getId() {
            return id;
        }

        /// <summary>
        /// generated on start from gameobject name
        /// </summary>
        public string getSecId() {
            return secId;
        }

    }

}
