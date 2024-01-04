using System.Collections.Generic;
using Proyecto.ScriptableObjects.Tutorial;
using UnityEngine;
using UnityEngine.Serialization;

namespace Proyecto.Manager
{
    public class DialogosFinalesManager : Singleton<DialogosFinalesManager>
    {
        #region Inspector Variables
        
        [SerializeField] private List<TutorialScriptableObject> _spanishFinalsTexts;
        [SerializeField] private List<TutorialScriptableObject> _englishFinalsTexts;
        
        #endregion

        #region Public Methods

        public void  ShowFinalText(int puntuacion)
        {
            var text = IdiomaManager.Instance.IsSpanish ? _spanishFinalsTexts[puntuacion].TutorialTextList[Random.Range(0, _spanishFinalsTexts[puntuacion].TutorialTextList.Count)].TutorialDescripcion
                : _englishFinalsTexts[puntuacion].TutorialTextList[Random.Range(0, _englishFinalsTexts[puntuacion].TutorialTextList.Count)].TutorialDescripcion;
            TutorialManager.Instance.PlayFinalText(text);
        }
        
        #endregion
        
    }
}