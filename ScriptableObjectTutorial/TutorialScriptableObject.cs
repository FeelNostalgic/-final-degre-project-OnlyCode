using System.Collections.Generic;
using UnityEngine;

namespace Proyecto.ScriptableObjects.Tutorial
{
    [CreateAssetMenu(fileName = "TutorialData", menuName = "Scriptable/Tutorial")]
    public class TutorialScriptableObject : ScriptableObject
    {
        public List<TutorialText> TutorialTextList;
    }
} 