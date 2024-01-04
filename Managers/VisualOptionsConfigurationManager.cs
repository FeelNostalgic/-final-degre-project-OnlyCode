using UnityEngine;
using UnityEngine.Serialization;

namespace Proyecto.Manager
{
    public class VisualOptionsConfigurationManager : Singleton<VisualOptionsConfigurationManager>
    {

        #region Inpector Variables

        [Header("Blancas")] 
        [SerializeField] private GameObject _pentagramaColor;
        [SerializeField] private GameObject _pentagramaMarron;
        
        [Header("Negras")] 
        [Header("Sostenidos")]
        [SerializeField] private GameObject _sostenidos;
        [SerializeField] private GameObject _notacionClasicaSostenidos;
        [SerializeField] private GameObject _notacionAnglosajonaSostenidos;
        
        [Header("Bemoles")]
        [SerializeField] private GameObject _bemoles;
        [SerializeField] private GameObject _notacionClasicaBemoles;
        [SerializeField] private GameObject _notacionAnglosajonaBemoles;

        [Header("Notacion")]
        [SerializeField] private GameObject _notacionClasica;
        [SerializeField] private GameObject _notacionAnglosajona;
        
        #endregion

        #region Private Variables

        private GameObject _currentPentagrama;
        private GameObject _currentNegras;
        private GameObject _currentNotacion;
        private GameObject _currentNotacionNegras;
        private bool _isSostenido;
        private bool _isNotacionClasica;

        #endregion

        #region Unity Methods

        private void Awake()
        {
#if UNITY_EDITOR
            Debug.Log("Cargando visualOptions...");          
#endif
            _currentPentagrama = _pentagramaColor;
            _currentNegras = _sostenidos;
            _currentNotacion = _notacionClasica;
            _currentNotacionNegras = _notacionClasicaSostenidos;
        }

        #endregion

        #region Public Methods
        [ContextMenu("Tools/SetNotacionClasica")]
        public void SetNotacionClasica()
        {
            _isNotacionClasica = true;
            _currentNotacion = changeGameObjectActive(_notacionClasica, _currentNotacion);
            if(_isSostenido) _currentNotacionNegras = changeGameObjectActive(_notacionClasicaSostenidos, _currentNotacionNegras);
            else _currentNotacionNegras = changeGameObjectActive(_notacionClasicaBemoles, _currentNotacionNegras);
            // Debug_Current();
        }
        
        [ContextMenu("Tools/SetNotacionAnglosagona")]
        public void SetNotacionAnglosajona()
        { ;
            _isNotacionClasica = false;
            _currentNotacion = changeGameObjectActive(_notacionAnglosajona, _currentNotacion);
            if(_isSostenido) _currentNotacionNegras = changeGameObjectActive(_notacionAnglosajonaSostenidos, _currentNotacionNegras);
            else _currentNotacionNegras = changeGameObjectActive(_notacionAnglosajonaBemoles, _currentNotacionNegras);
            // Debug_Current();
        }

        [ContextMenu("Tools/SetPentagramaColores")]
        public void SetPentagramaColores()
        {
            VisualKeyManager.Instance.ShowColorsOnColumns = true;
            _currentPentagrama = changeGameObjectActive(_pentagramaColor, _currentPentagrama);
            // Debug_Current();
        }
        
        [ContextMenu("Tools/SetPentagramaMarron")]
        public void SetPentagramaMarron()
        {
            VisualKeyManager.Instance.ShowColorsOnColumns = false;
            _currentPentagrama = changeGameObjectActive(_pentagramaMarron, _currentPentagrama);
            // Debug_Current();
        }
        
        [ContextMenu("Tools/SetSostenidos")]
        public void SetSostenidos()
        {
            _currentNegras = changeGameObjectActive(_sostenidos, _currentNegras);
            if (_isNotacionClasica) _currentNotacionNegras = changeGameObjectActive(_notacionClasicaSostenidos, _currentNotacionNegras);
            else _currentNotacionNegras = changeGameObjectActive(_notacionAnglosajonaSostenidos, _currentNotacionNegras);
            _isSostenido = true;
            // Debug_Current();
        }
        
        [ContextMenu("Tools/SetBemoles")]
        public void SetBemoles()
        {
            _currentNegras = changeGameObjectActive(_bemoles, _currentNegras);
            if (_isNotacionClasica)  _currentNotacionNegras = changeGameObjectActive(_notacionClasicaBemoles, _currentNotacionNegras);
            else  _currentNotacionNegras = changeGameObjectActive(_notacionAnglosajonaBemoles, _currentNotacionNegras);
            _isSostenido = false;
            // Debug_Current();
        }
        
        #endregion

        #region Private Methods

        private GameObject changeGameObjectActive(GameObject newObject, GameObject current)
        {
            current.SetActive(false);
            newObject.SetActive(true);
            return newObject;
        }

        private void Debug_Current()
        {
            //Debug.Log(_currentNegras.name);
            Debug.Log(_currentNotacion.name);
            //Debug.Log(_currentPentagrama.name);
            Debug.Log(_currentNotacionNegras.name);
        }
        #endregion
    }
}