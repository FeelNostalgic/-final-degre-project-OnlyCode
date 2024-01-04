using Proyecto.ScriptableObjects.Idioma;
using TMPro;
using UnityEngine;

namespace Proyecto.Manager
{
    public class IdiomaManager : Singleton<IdiomaManager>
    {
        #region Inspector Variables

        [SerializeField] private bool SetSpanish;

        [Header("Menu Principal")] [Tooltip("Niveles, Modo libre, Opciones, Créditos")] 
        [SerializeField] private TMP_Text[] _menuPrincipalTexts;

        [Tooltip("0-> Spanish, 1-> English")] 
        [SerializeField] private IdiomaScriptableObject[] _menuPrincipalIdiomas;

        [Header("Niveles")] 
        [Header("Titulo Niveles")]
        [SerializeField] private TMP_Text[] _tituloNivelesTexts;
        [SerializeField] private IdiomaScriptableObject[] _tituloNivelesIdiomas;

        [Header("Description Niveles")]
        [SerializeField] private TMP_Text[] _descriptionNivelesTexts;
        [SerializeField] private IdiomaScriptableObject[] _descriptionNivelesIdiomas;

        [Header("Opciones")]
        [Header("Graficos")]
        [SerializeField] private TMP_Text[] _graficosTexts;
        [SerializeField] private IdiomaScriptableObject[] _graficosIdiomas;

        [Header("Sonido")] 
        [SerializeField] private TMP_Text[] _sonidoTexts;
        [SerializeField] private TMP_Dropdown _intrumentsList;
        [SerializeField] private IdiomaScriptableObject[] _sonidoIdiomas;

        [Header("Creditos")]
        [SerializeField] private TMP_Text[] _creditosTexts;
        [SerializeField] private IdiomaScriptableObject[] _creditosIdiomas;

        [Header("Pausa")]
        [SerializeField] private TMP_Text[] _pausaTexts;
        [SerializeField] private IdiomaScriptableObject[] _pausaIdiomas;

        [Header("Panel Final")]
        [SerializeField] private TMP_Text[] _finalTexts;
        [SerializeField] private IdiomaScriptableObject[] _finalIdiomas;

        #endregion

        #region Public Variables

        public bool IsSpanish => _IsSpanish;

        #endregion

        #region Private Variables

        private bool _IsSpanish;

        #endregion

        #region Unity Methods

        private void Awake()
        {
#if UNITY_EDITOR
            Debug.Log("Cargando idioma...");         
#endif
            if (Application.systemLanguage == SystemLanguage.Spanish) //Set language in Spanish
                _IsSpanish = true;

#if UNITY_EDITOR
            _IsSpanish = SetSpanish;
#endif
            setIdiomaMenuPrincipal();
            setIdiomaNiveles();
            setIdiomaOpcionesGraficos();
            setIdiomaOpcionesSonidos();
            setIdiomaCreditos();
            setIdiomaPausa();
            setIdiomaFinalPanel();
        }

        #endregion

        #region Private Methods

        private void setIdiomaMenuPrincipal()
        {
            setIdioma(_menuPrincipalTexts, _menuPrincipalIdiomas);
        }

        private void setIdiomaNiveles()
        {
            setIdioma(_tituloNivelesTexts, _tituloNivelesIdiomas);
            setIdioma(_descriptionNivelesTexts, _descriptionNivelesIdiomas);
        }

        private void setIdiomaOpcionesGraficos()
        {
            setIdioma(_graficosTexts, _graficosIdiomas);
        }

        private void setIdiomaOpcionesSonidos()
        {
            var index = setIdioma(_sonidoTexts, _sonidoIdiomas);
            foreach (var intrument in _intrumentsList.options)
            {
                intrument.text = _sonidoIdiomas[_IsSpanish ? 0 : 1].IdiomaTextList[index].TextIdioma;
                index++;
            }
        }

        private void setIdiomaCreditos()
        {
            setIdioma(_creditosTexts, _creditosIdiomas);
        }

        private void setIdiomaPausa()
        {
            setIdioma(_pausaTexts, _pausaIdiomas);
        }

        private void setIdiomaFinalPanel()
        {
            setIdioma(_finalTexts, _finalIdiomas);
        }

        private int setIdioma(TMP_Text[] texts, IdiomaScriptableObject[] idiomas)
        {
            var index = 0;
            foreach (var tmp in texts)
            {
                tmp.text = idiomas[_IsSpanish ? 0 : 1].IdiomaTextList[index].TextIdioma; 
                index++;
            }

            return index;
        }

        #endregion
    }
}