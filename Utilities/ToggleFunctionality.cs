using System;
using Proyecto.Manager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Proyecto.Utility
{
    public class ToggleFunctionality : MonoBehaviour, IPointerDownHandler
    {
        #region Inspector Variables
        
        [SerializeField] private _type _optionType;
        [SerializeField] private Sprite _onImage;

        #endregion

        #region Private Variables

        private enum _type
        {
            None, AyudaVisual, Colores, Notacion, NotacionNegras
        }

        private bool _isOn;
        private Sprite _offImage;
        private Image _image;
        private RectTransform _rectTransform;
        
        #endregion

        #region Unity Methods

        public void Awake()
        {
            _image = GetComponent<Image>();
            _rectTransform = GetComponent<RectTransform>();
            _offImage = _image.sprite;
            _isOn = getStartingValueFromOptions(); //Cargar opciones desde json
            if (_isOn)
            {
                _image.sprite = _onImage;
                _rectTransform.rotation = Quaternion.Euler(0,0,180);
            }
            Debug.Log("Cargando " + _optionType);
            doFunctionality();
        }

        #endregion
        
        #region Public Methods

        public void OnPointerDown(PointerEventData eventData)
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_type.Click);
            _isOn = !_isOn;
            updateSprite();
            doFunctionality();
        }
        
        #endregion

        #region Private Methods

        private bool getStartingValueFromOptions()
        {
            switch (_optionType)
            {
                case _type.None:
                    break;
                case _type.AyudaVisual:
                    return SaveManager.Instance.Options.IsVisualHelp;
                case _type.Colores:
                    return SaveManager.Instance.Options.IsColores;
                case _type.Notacion:
                    return SaveManager.Instance.Options.IsClasica;
                case _type.NotacionNegras:
                    return SaveManager.Instance.Options.IsSostenido;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
        }
        
        private void updateSprite()
        {
            if (_isOn)
            {
                _image.sprite = _onImage;
                _rectTransform.rotation = Quaternion.Euler(0,0,180);
            }
            else
            {
                _image.sprite = _offImage;
                _rectTransform.rotation = Quaternion.Euler(0,0,0);
            }
        }

        private void doFunctionality()
        {
            switch (_optionType)
            {
                case _type.None:
                    break;
                case _type.AyudaVisual:
                    GameplayManager.Instance.ShowVisualHelp = _isOn;
                    SaveManager.Instance.Options.IsVisualHelp = _isOn;
                    break;
                case _type.Colores:
                    if(_isOn) VisualOptionsConfigurationManager.Instance.SetPentagramaColores();
                    else VisualOptionsConfigurationManager.Instance.SetPentagramaMarron();
                    SaveManager.Instance.Options.IsColores = _isOn;
                    break;
                case _type.Notacion:
                    if(_isOn) VisualOptionsConfigurationManager.Instance.SetNotacionClasica();
                    else VisualOptionsConfigurationManager.Instance.SetNotacionAnglosajona();
                    SaveManager.Instance.Options.IsClasica = _isOn;
                    break;
                case _type.NotacionNegras:
                    if(_isOn) VisualOptionsConfigurationManager.Instance.SetSostenidos();
                    else VisualOptionsConfigurationManager.Instance.SetBemoles();
                    SaveManager.Instance.Options.IsSostenido = _isOn;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        #endregion
    }
}