using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Proyecto.ScriptableObjects.Tutorial;
using Proyecto.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Proyecto.Manager
{
    public class TutorialManager : Singleton<TutorialManager>
    {
        #region Inspector Variables
        [Header("Dialogos Tutorial Default")]
        [SerializeField] private GameObject _athenaGB;
        [SerializeField] private float _athenaStartingPositionY;
        [SerializeField] private float _athenaStartingPositionX;
        [SerializeField] private GameObject _maskAll;
        [SerializeField] private GameObject _fondoDialogo;
        [SerializeField] private TMP_Text _textDialogo;
        [SerializeField] private TMP_Text _textContinuar;
        
        [Header("Dialogos Tutorial Menu Principal")] 
        [SerializeField] private Image _athenaImangeMenuPrincipal; 
        [SerializeField] private GameObject _maskAllMenuPrincipal;
        [SerializeField] private GameObject _fondoDialogoMenuPrincipal;
        [SerializeField] private TMP_Text _textDialogoMenuPrincipal;
        [SerializeField] private TMP_Text _textContinuarMenuPrincipal;
        
        [Tooltip("Orden: MENU PRINCIPAL, NIVELES, HUD, OPCIONES")]
        [SerializeField] private List<TutorialScriptableObject> _spanishTutorialSequence;
        [SerializeField] private List<TutorialScriptableObject> _englishTutorialSequence;
        
        [Header("Tutorial Niveles")]
        [SerializeField] private GameObject[] _maskListTutorialNiveles;
        
        [Header("Tutorial Opciones")]
        [SerializeField] private GameObject[] _maskListTutorialOpciones;
        
        [Header("Tutorial HUD")]
        [SerializeField] private GameObject[] _maskListTutorialHUD;
        #endregion

        #region Public Variables
        public bool PlayNivelesTutorial => _playNivelesTutorial;
        public bool PlayHUDTutorial => _playHUDTutorial;
        public bool PlayOpcionesTutorial => _playOpcionesTutorial;
        
        #endregion

        #region Private Variables
        
        private bool _playNivelesTutorial;
        private bool _playHUDTutorial;
        private bool _playOpcionesTutorial;
        private Vector2 _fondoDialogoInitialSize;
        private float _athenaInitialPosX;
        private GameObject _currentMask;
        private bool _playTutorialAgain;

        #region Const variables

        private const float TIME_TO_SHOW_TEXT = 0.2f;
        private const float TIME_TO_SHOW_IMAGE = 0.4f;
        private const string SPANISH_CONTINUAR_TEXT = "Toca para continuar";
        private const string ENGLISH_CONTINUAR_TEXT = "Touch to continue";

        #endregion
        
        #endregion

        #region Unity Methods

        private void Awake()
        {
#if UNITY_EDITOR
            Debug.Log("Cargando Tutorial Manager...");         
#endif
            if (SaveManager.Instance.PlayTutorial)
            {
                _playNivelesTutorial = true;
                _playHUDTutorial = true;
                _playOpcionesTutorial = true;
            }

            _playTutorialAgain = true;

            var AthenaRect = _athenaGB.GetComponent<RectTransform>();
            _athenaInitialPosX = AthenaRect.anchoredPosition3D.x;
            
            var fondoRect = _fondoDialogo.GetComponent<RectTransform>();
            _fondoDialogoInitialSize = fondoRect.sizeDelta;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Play a coroutine with the tutorial
        /// </summary>
        /// <param name="index">1 -> Menu Principal, 2 -> Niveles, 3-> HUD, 4 -> Opciones, 5 -> Menu Principal with tutorial button</param>
        public void PlayTutorial(int index)
        {
            switch (index)
            {
                case 1: StartCoroutine(playMenuPrincipalTutorialCoroutine(IdiomaManager.Instance.IsSpanish ? _spanishTutorialSequence[index-1].TutorialTextList : _englishTutorialSequence[index-1].TutorialTextList));
                    break;
                case 2: StartCoroutine(playNivelesTutorialCoroutine(IdiomaManager.Instance.IsSpanish ? _spanishTutorialSequence[index-1].TutorialTextList : _englishTutorialSequence[index-1].TutorialTextList,_maskListTutorialNiveles,0));
                    _playNivelesTutorial = false;
                    break;
                case 3: StartCoroutine(playHUDTutorialCoroutine(IdiomaManager.Instance.IsSpanish ? _spanishTutorialSequence[index-1].TutorialTextList : _englishTutorialSequence[index-1].TutorialTextList,_maskListTutorialHUD,_athenaStartingPositionY));
                    _playHUDTutorial = false;
                    break;
                case 4: StartCoroutine(playOpcionesTutorialCoroutine(IdiomaManager.Instance.IsSpanish ? _spanishTutorialSequence[index-1].TutorialTextList : _englishTutorialSequence[index-1].TutorialTextList,_maskListTutorialOpciones,-15));
                    _playOpcionesTutorial = false;
                    break;
            }
            
            if(!_playNivelesTutorial && !_playOpcionesTutorial && !_playHUDTutorial) UIManager.Instance.SetMenuPrincipalWithTutorial();
        }

        public void PlayFinalText(string text)
        {
            StartCoroutine(playFinalTextCoroutine(text, _athenaStartingPositionY));
        }

        #endregion

        #region Private Methods

        private IEnumerator playNivelesTutorialCoroutine(List<TutorialText> tutorialText, GameObject[] masks, float startPositionY)
        {
            #region Sequence SHOW DIALOGOS
            var athenaRect = _athenaGB.GetComponent<RectTransform>();
            var fondoRect = _fondoDialogo.GetComponent<RectTransform>();
            
            var sq = showDialogosSequence(startPositionY, athenaRect, fondoRect);

            yield return new WaitWhile(() => sq.IsPlaying());
            
            _textContinuar.text = IdiomaManager.Instance.IsSpanish ? SPANISH_CONTINUAR_TEXT : ENGLISH_CONTINUAR_TEXT;
            _textContinuar.DOFade(1, TIME_TO_SHOW_TEXT).Play();
            
            #endregion

            var maskIndex = 0;
            foreach (var text in tutorialText)
            {
                _textDialogo.text = text.TutorialDescripcion;
                _textDialogo.DOFade(1, TIME_TO_SHOW_TEXT).Play();
                
                if (text.HasMask)
                {
                    _maskAll.SetActive(false);
                    if(_currentMask != null) _currentMask.SetActive(false);
                    
                    _currentMask = masks[maskIndex];
                    maskIndex++;
                    _currentMask.SetActive(true);
                }
                else
                {
                    if(_currentMask != null)_currentMask.SetActive(false);
                    _maskAll.SetActive(true);
                }
                
                yield return new WaitForSeconds(.25f);
                yield return new WaitUntil(() => Input.GetMouseButtonDown(0)); //Espera un toque en pantalla
                AudioManager.Instance.PlaySFXSound(AudioManager.SFX_type.Click);

                if (text.OrdenTutorial.Equals(tutorialText.Count.ToString())) continue;
                var t = _textDialogo.DOFade(0, TIME_TO_SHOW_TEXT).Play();
                yield return new WaitWhile(() => t.IsPlaying());

            }

            #region Sequence HIDE DIALOGOS

            sq = hideDialogosSequence(fondoRect, athenaRect);

            yield return new WaitWhile(() => sq.IsPlaying());

            #endregion

            UIManager.Instance.SetActiveTutorialPanel(false);
            _maskAll.SetActive(true);
            _currentMask.SetActive(false);
            
        }

        private IEnumerator playMenuPrincipalTutorialCoroutine(List<TutorialText> tutorialText)
        {
            if (_playTutorialAgain) activeTutorial();
            else desactiveTutorial();
            
            _athenaImangeMenuPrincipal.raycastTarget = false;
            #region Sequence SHOW DIALOGOS

            Sequence sq = DOTween.Sequence();

            //Hacer aparecer caja texto
            var fondoRect = _fondoDialogoMenuPrincipal.GetComponent<RectTransform>();
            sq.Append(fondoRect.DOSizeDelta(_fondoDialogoInitialSize, TIME_TO_SHOW_IMAGE).SetEase(Ease.OutExpo));

            sq.OnPlay(() =>
            {
                _maskAllMenuPrincipal.SetActive(true);
                _fondoDialogoMenuPrincipal.SetActive(true);
                fondoRect.sizeDelta = new Vector2(0, 0);
                _textDialogoMenuPrincipal.text = "";
                _textContinuarMenuPrincipal.text = "";
                
            });
            
            sq.Play();
            
            yield return new WaitWhile(() => sq.IsPlaying());
            
            _textContinuarMenuPrincipal.text = IdiomaManager.Instance.IsSpanish ? SPANISH_CONTINUAR_TEXT : ENGLISH_CONTINUAR_TEXT;
            _textContinuarMenuPrincipal.DOFade(1, TIME_TO_SHOW_TEXT).Play();
            
            #endregion
            
            _textDialogoMenuPrincipal.text = tutorialText[_playTutorialAgain ? 0 : 1].TutorialDescripcion;
            _textDialogoMenuPrincipal.DOFade(1, TIME_TO_SHOW_TEXT).Play();
            _playTutorialAgain = !_playTutorialAgain;
            
            yield return new WaitForSeconds(.25f);
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0)); //Espera un toque en pantalla
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_type.Click);
            
            #region Sequence HIDE DIALOGOS

            sq = DOTween.Sequence();
            
            //Hacer desaparecer caja texto
            sq.Append(_textDialogoMenuPrincipal.DOFade(0, TIME_TO_SHOW_TEXT));
            sq.Join(_textContinuarMenuPrincipal.DOFade(0, TIME_TO_SHOW_TEXT));
            sq.Append(fondoRect.DOSizeDelta(new Vector2(0, 0), TIME_TO_SHOW_IMAGE).SetEase(Ease.InExpo));

            sq.Play();
            
            yield return new WaitWhile(() => sq.IsPlaying());
            
            _maskAllMenuPrincipal.SetActive(false);
            _fondoDialogoMenuPrincipal.SetActive(false);
            
            #endregion
            _athenaImangeMenuPrincipal.raycastTarget = true;
        }
        
        private IEnumerator playHUDTutorialCoroutine(List<TutorialText> tutorialText, GameObject[] masks, float startPositionY)
        {
            #region Sequence SHOW DIALOGOS

            Sequence sq = DOTween.Sequence();
            
            //Hacer aparecer Athena
            var AthenaRect = _athenaGB.GetComponent<RectTransform>();
            
            sq.Append(AthenaRect.DOAnchorPosX(_athenaInitialPosX, TIME_TO_SHOW_IMAGE).SetEase(Ease.OutQuart));

            //Hacer aparecer caja texto
            var fondoRect = _fondoDialogo.GetComponent<RectTransform>();
            //Mover arriba
            fondoRect.anchoredPosition3D = new Vector3(fondoRect.anchoredPosition3D.x, 0, 0);
            //Girar 180 X
            fondoRect.rotation = Quaternion.Euler(new Vector3(180,0,0));
            
            sq.Append(fondoRect.DOSizeDelta(_fondoDialogoInitialSize, TIME_TO_SHOW_IMAGE).SetEase(Ease.OutExpo));

            sq.OnPlay(() =>
            {
                UIManager.Instance.SetTutorialY(startPositionY);
                AthenaRect.anchoredPosition3D += Vector3.right * _athenaStartingPositionX;
                fondoRect.sizeDelta = new Vector2(0, 0);
                _textDialogo.text = "";
                _textContinuar.text = "";
                UIManager.Instance.SetActiveTutorialPanel(true);
            });
            
            sq.Play();
            
            yield return new WaitWhile(() => sq.IsPlaying());
            
            _textContinuar.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3(180,0,0));
            _textDialogo.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3(180,0,0));
            _textContinuar.text = IdiomaManager.Instance.IsSpanish ? SPANISH_CONTINUAR_TEXT : ENGLISH_CONTINUAR_TEXT;
            _textContinuar.DOFade(1, TIME_TO_SHOW_TEXT).Play();
            
            #endregion

            var maskIndex = 0;
            foreach (var text in tutorialText)
            {
                _textDialogo.text = text.TutorialDescripcion;
                _textDialogo.DOFade(1, TIME_TO_SHOW_TEXT).Play(); //Poner texto nuevo
                
                if (text.HasMask)
                {
                    _maskAll.SetActive(false);
                    if(_currentMask != null) _currentMask.SetActive(false);
                    
                    _currentMask = masks[maskIndex];
                    maskIndex++;
                    _currentMask.SetActive(true);
                }
                else
                {
                    if(_currentMask != null)_currentMask.SetActive(false);
                    _maskAll.SetActive(true);
                }
                
                yield return new WaitForSeconds(.25f);
                yield return new WaitUntil(() => Input.GetMouseButtonDown(0)); //Espera un toque en pantalla
                AudioManager.Instance.PlaySFXSound(AudioManager.SFX_type.Click);

                if (text.OrdenTutorial.Equals(tutorialText.Count.ToString())) continue; //Ultimo texto
                
                var t = _textDialogo.DOFade(0, TIME_TO_SHOW_TEXT).Play(); //Quitar texto antiguo
                yield return new WaitWhile(() => t.IsPlaying());
            }

            #region Sequence HIDE DIALOGOS

            sq = DOTween.Sequence();
            
            //Hacer desaparecer caja texto
            sq.Append(_textDialogo.DOFade(0, TIME_TO_SHOW_TEXT));
            sq.Join(_textContinuar.DOFade(0, TIME_TO_SHOW_TEXT));
            sq.Append(fondoRect.DOSizeDelta(new Vector2(0, 0), TIME_TO_SHOW_IMAGE).SetEase(Ease.InExpo));

            //Hacer desaparecer athena
            sq.Append(AthenaRect.DOAnchorPosX(360, TIME_TO_SHOW_IMAGE).SetEase(Ease.InQuart));
            
            sq.Play();
            
            yield return new WaitWhile(() => sq.IsPlaying());

            fondoRect.rotation = Quaternion.Euler(new Vector3(0,0,0));
            _textContinuar.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0,0,0));
            _textDialogo.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0,0,0));
            fondoRect.anchoredPosition3D = new Vector3(-517, fondoRect.anchoredPosition3D.y, 0);
            UIManager.Instance.SetActiveTutorialPanel(false);
            
            #endregion
            
            CameraController.Instance.IsStarted = true;
        }
        
        private IEnumerator playOpcionesTutorialCoroutine(List<TutorialText> tutorialText, GameObject[] masks, float startPositionY)
        {
            #region Sequence SHOW DIALOGOS

            var athenaRect = _athenaGB.GetComponent<RectTransform>();
            var fondoRect = _fondoDialogo.GetComponent<RectTransform>();
            
            var sq = showDialogosSequence(startPositionY, athenaRect, fondoRect);
            
            sq.Play();
            
            yield return new WaitWhile(() => sq.IsPlaying());
            
            _textContinuar.text = IdiomaManager.Instance.IsSpanish ? SPANISH_CONTINUAR_TEXT : ENGLISH_CONTINUAR_TEXT;
            _textContinuar.DOFade(1, TIME_TO_SHOW_TEXT).Play();
            
            #endregion
            
            var maskIndex = 0;
            foreach (var text in tutorialText)
            {
                if (text.OrdenTutorial.Equals("5"))//Subir dialogo
                {
                    //Ocultar dialogo
                    var tween = _textContinuar.DOFade(0, TIME_TO_SHOW_TEXT).Play();
                    yield return new WaitWhile(tween.IsPlaying);
                    var tween2 = fondoRect.DOSizeDelta(new Vector2(0, 0), TIME_TO_SHOW_IMAGE).SetEase(Ease.InExpo).Play();
                    yield return new WaitWhile(tween2.IsPlaying);
                    
                    //Mover arriba
                    fondoRect.anchoredPosition3D = new Vector3(fondoRect.anchoredPosition3D.x, 0, 0);
                    
                    //Girar 180 X
                    fondoRect.rotation = Quaternion.Euler(new Vector3(180,0,0));

                    //Mostrar
                    tween2 = fondoRect.DOSizeDelta(_fondoDialogoInitialSize, TIME_TO_SHOW_IMAGE).SetEase(Ease.InExpo).Play();
                    yield return new WaitWhile(tween2.IsPlaying);
                    _textContinuar.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3(180,0,0));
                    _textDialogo.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3(180,0,0));
                    tween = _textContinuar.DOFade(1, TIME_TO_SHOW_TEXT).Play();
                    yield return new WaitWhile(tween.IsPlaying);
                }

                if (text.OrdenTutorial.Equals("7"))//Mover athena y dialogo a la derecha
                {
                    //Ocultar dialogo
                    var tween = _textContinuar.DOFade(0, TIME_TO_SHOW_TEXT).Play();
                    yield return new WaitWhile(tween.IsPlaying);
                    var tween2 = fondoRect.DOSizeDelta(new Vector2(0, 0), TIME_TO_SHOW_IMAGE).SetEase(Ease.InExpo).Play();
                    yield return new WaitWhile(tween2.IsPlaying);
                    
                    //Mover derecha
                    fondoRect.anchoredPosition3D = new Vector3(-1300, -110f, 0);
                    
                    //Girar 180 Z
                    fondoRect.rotation = Quaternion.Euler(new Vector3(180,0,180));

                    //Mover athena
                    tween2 = athenaRect.DOAnchorPosX(-1500, TIME_TO_SHOW_IMAGE).SetEase(Ease.OutQuart).Play();
                    yield return new WaitWhile(tween2.IsPlaying);
                    
                    //Mostrar dialogo
                    tween2 = fondoRect.DOSizeDelta(_fondoDialogoInitialSize, TIME_TO_SHOW_IMAGE).SetEase(Ease.InExpo).Play();
                    yield return new WaitWhile(tween2.IsPlaying);
                    _textContinuar.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3(180,0,180));
                    _textDialogo.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3(180,0,180));
                    
                    tween = _textContinuar.DOFade(1, TIME_TO_SHOW_TEXT).Play();
                    yield return new WaitWhile(tween.IsPlaying);
                }
                
                _textDialogo.text = text.TutorialDescripcion;
                _textDialogo.DOFade(1, TIME_TO_SHOW_TEXT).Play(); //Poner texto nuevo
                
                if (text.HasMask)
                {
                    _maskAll.SetActive(false);
                    if(_currentMask != null) _currentMask.SetActive(false);
                    
                    _currentMask = masks[maskIndex];
                    maskIndex++;
                    _currentMask.SetActive(true);
                }
                else
                {
                    if(_currentMask != null)_currentMask.SetActive(false);
                    _maskAll.SetActive(true);
                }
                
                yield return new WaitForSeconds(.25f);
                yield return new WaitUntil(() => Input.GetMouseButtonDown(0)); //Espera un toque en pantalla
                AudioManager.Instance.PlaySFXSound(AudioManager.SFX_type.Click);

                if (text.OrdenTutorial.Equals(tutorialText.Count.ToString())) continue; //Ultimo texto
                
                var t = _textDialogo.DOFade(0, TIME_TO_SHOW_TEXT).Play(); //Quitar texto antiguo
                yield return new WaitWhile(() => t.IsPlaying());

            }
            
            #region Sequence HIDE DIALOGOS

            sq = DOTween.Sequence();
            
            //Hacer desaparecer caja texto
            sq.Append(_textDialogo.DOFade(0, TIME_TO_SHOW_TEXT));
            sq.Join(_textContinuar.DOFade(0, TIME_TO_SHOW_TEXT));
            sq.Append(fondoRect.DOSizeDelta(new Vector2(0, 0), TIME_TO_SHOW_IMAGE).SetEase(Ease.InExpo));

            //Hacer desaparecer athena
            sq.Append(athenaRect.DOAnchorPosX(360, TIME_TO_SHOW_IMAGE).SetEase(Ease.InQuart));
            
            sq.Play();
            
            yield return new WaitWhile(() => sq.IsPlaying());

            fondoRect.rotation = Quaternion.Euler(new Vector3(0,0,0));
            _textContinuar.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0,0,0));
            _textDialogo.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0,0,0));
            fondoRect.anchoredPosition3D = new Vector3(-517, fondoRect.anchoredPosition3D.y, 0);
            
            #endregion

            UIManager.Instance.SetActiveTutorialPanel(false);
            _maskAll.SetActive(true);
            _currentMask.SetActive(false);
            
        }
        
        private IEnumerator playFinalTextCoroutine(string text, float startPositionY)
        {
            #region Sequence SHOW DIALOGOS

            var athenaRect = _athenaGB.GetComponent<RectTransform>();
            var fondoRect = _fondoDialogo.GetComponent<RectTransform>();
            
            var sq = showDialogosSequence(startPositionY, athenaRect, fondoRect);
            
            sq.Play();
            
            yield return new WaitWhile(() => sq.IsPlaying());
            
            _textContinuar.text = IdiomaManager.Instance.IsSpanish ? SPANISH_CONTINUAR_TEXT : ENGLISH_CONTINUAR_TEXT;
            _textContinuar.DOFade(1, TIME_TO_SHOW_TEXT).Play();
            
            #endregion
            
            _textDialogo.text = text;
            _textDialogo.DOFade(1, TIME_TO_SHOW_TEXT).Play();
            
            yield return new WaitForSeconds(.25f);
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0)); //Espera un toque en pantalla
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_type.Click);
            
            #region Sequence HIDE DIALOGOS

            sq = hideDialogosSequence(fondoRect, athenaRect);

            yield return new WaitWhile(() => sq.IsPlaying());
            
            UIManager.Instance.SetActiveTutorialPanel(false);

            #endregion
        }
        
        private Sequence showDialogosSequence(float startPositionY, RectTransform athenaRect, RectTransform fondoRect)
        {
            Sequence sq = DOTween.Sequence();

            //Hacer aparecer Athena
            
            sq.Append(athenaRect.DOAnchorPosX(_athenaInitialPosX, TIME_TO_SHOW_IMAGE).SetEase(Ease.OutQuart));

            //Hacer aparecer caja texto
            sq.Append(fondoRect.DOSizeDelta(_fondoDialogoInitialSize, TIME_TO_SHOW_IMAGE).SetEase(Ease.OutExpo));
            
            sq.OnPlay(() =>
            {
                UIManager.Instance.SetTutorialY(startPositionY);
                athenaRect.anchoredPosition3D += Vector3.right * _athenaStartingPositionX;
                fondoRect.sizeDelta = new Vector2(0, 0);
                _textDialogo.text = "";
                _textContinuar.text = "";
                UIManager.Instance.SetActiveTutorialPanel(true);
            });

            sq.Play();
            return sq;
        }
        
        private Sequence hideDialogosSequence(RectTransform fondoRect, RectTransform athenaRect)
        {
            Sequence sq;
            sq = DOTween.Sequence();

            //Hacer desaparecer caja texto
            sq.Append(_textDialogo.DOFade(0, TIME_TO_SHOW_TEXT));
            sq.Join(_textContinuar.DOFade(0, TIME_TO_SHOW_TEXT));
            sq.Append(fondoRect.DOSizeDelta(new Vector2(0, 0), TIME_TO_SHOW_IMAGE).SetEase(Ease.InExpo));

            //Hacer desaparecer athena
            sq.Append(athenaRect.DOAnchorPosX(_athenaStartingPositionX, TIME_TO_SHOW_IMAGE).SetEase(Ease.InQuart));

            sq.Play();
            return sq;
        }
        
        private void activeTutorial()
        {
            _playNivelesTutorial = true;
            _playHUDTutorial = true;
            _playOpcionesTutorial = true;
        }
        
        private void desactiveTutorial()
        {
            _playNivelesTutorial = false;
            _playHUDTutorial = false;
            _playOpcionesTutorial = false;
        }
        
        #endregion
    }
}