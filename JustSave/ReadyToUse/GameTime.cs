using System;
using UnityEngine;
using Unity;

namespace JustSave
{
    /// <summary>
    /// a class to provide persistent time-access across saves
    /// </summary>
    [RequireComponent(typeof(JustSaveId))]
    class GameTime : Savable
    {
        [HideInInspector]
        [Autosaved]
        public double CurrentTime;

        private void Update()
        {
            CurrentTime += Time.deltaTime;
        }
    }
}
