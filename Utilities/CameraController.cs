using DG.Tweening;
using Proyecto.Manager;
using UnityEngine;
using UnityEngine.Serialization;

namespace Proyecto.Utility
{
    public class CameraController : Singleton<CameraController>
    {
        #region Inspector Variables

        [SerializeField] private GameObject _finalPosition;
        [SerializeField] private float _duration;
        [SerializeField] private GameObject _temploGB;
        [SerializeField] private GameObject _sueloAux;
        [SerializeField] private GameObject _toHide;
        [SerializeField] private float _sizeIfScreenOver16_9;
        [SerializeField] private float _sizeIfScreenBelow16_9;
        
        #endregion

        #region Public Variables

        public bool IsStarted
        {
            get => _isStarted;
            set => _isStarted = value;
        }

        #endregion

        #region Private Variables

        private bool _isStarted;
        private Vector3 _startPosition;
        private Quaternion _startRotation;
        private float _startOrthograpgicSize;
        private int _startMask;

        #endregion

        #region Unity Methods

        private void Start()
        {
#if UNITY_EDITOR
            Debug.Log("Cargando Camara Controller...");         
#endif
            _startPosition = Camera.main.transform.position;
            _startRotation = Camera.main.transform.rotation;
            _startOrthograpgicSize = Camera.main.orthographicSize;
            _startMask = Camera.main.cullingMask;
            if (Camera.main.aspect > 2)
            {
                _finalPosition.GetComponent<Camera>().orthographicSize = _sizeIfScreenOver16_9;
            }

            if (Camera.main.aspect < 1.7)
            {
                _finalPosition.GetComponent<Camera>().orthographicSize = _sizeIfScreenBelow16_9;
            }
            _temploGB.SetActive(false);
        }

        #endregion

        #region Public Methods

        public void MoveToGameplayPosition()
        {
            if (!_isStarted)
            {
                Sequence mySequence = DOTween.Sequence();
                mySequence.Insert(.4f, Camera.main.transform.DOMove(_finalPosition.transform.position, _duration));
                mySequence.Insert(.4f,
                    Camera.main.transform.DORotateQuaternion(_finalPosition.transform.rotation, _duration));
                mySequence.Insert(.4f,
                    Camera.main.DOOrthoSize(_finalPosition.GetComponent<Camera>().orthographicSize, _duration));
                mySequence.Play();
                mySequence.OnComplete(() =>
                {
                    Camera.main.cullingMask = _finalPosition.GetComponent<Camera>().cullingMask;
                    _toHide.SetActive(false);
                    _sueloAux.SetActive(true);
                    UIManager.Instance.ShowHUD();
                    if (!GameplayManager.Instance.IsModoLibre && TutorialManager.Instance.PlayHUDTutorial) TutorialManager.Instance.PlayTutorial(3);
                    else _isStarted = true;
                });
                mySequence.OnPlay(()=>
                {
                    _temploGB.SetActive(true);
                });
            }
        }

        public void MoveToStartPosition()
        {
            Camera.main.transform.position = _startPosition;
            Camera.main.transform.rotation = _startRotation;
            Camera.main.orthographicSize = _startOrthograpgicSize;
            Camera.main.cullingMask = _startMask;
            _isStarted = false;
            _toHide.SetActive(true);
            _sueloAux.SetActive(false);
            _temploGB.SetActive(false);
        }

        // private void OnValidate()
        // {
        //     Debug.Log(Camera.main.aspect);
        // }

        #endregion
    }
}