using System;
using UnityEngine;

namespace Proyecto.ScriptableObjects.Tutorial
{
    [Serializable]
    public class TutorialText
    {
        #region Public Variables

        public string OrdenTutorial;
        [TextArea(3, 5)]
        public string TutorialDescripcion;
        public bool HasMask;

        #endregion
    }
}