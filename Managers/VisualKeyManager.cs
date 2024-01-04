using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Proyecto.Utility;
using UnityEngine;

namespace Proyecto.Manager
{
    public class VisualKeyManager : Singleton<VisualKeyManager>
    {
        #region Inspector Variables

        [SerializeField] private Renderer[] _columns;
        [Header("Light")]
        [SerializeField] private Color _defaultColor;
        
        [Header("Hit")]
        [SerializeField] private float _timeToHitLight;
        [SerializeField] private Color _colorHitWrong;

        #endregion
        
        #region Public Variables
        
        public Renderer[] Columns => _columns;
        public float TimeToLight
        {
            set => _timeToLight = value; 
        }
        public float TimeToHitLight => _timeToHitLight;
        public bool IsVisualComplete
        {
            get => _isVisualComplete;
            set => _isVisualComplete = value;
        }

        public bool ShowColorsOnColumns
        {
            set => _showColorsOnColumns = value;
        }

        #endregion

        #region Private Variables
        
        private float _timeToLight;
        private bool _isVisualComplete;
        private bool _showColorsOnColumns;
        private Sequence _visualKeySequence;
        private Tween _lastTween;
        private static Color _whiteDefault;
        private static Color _darkDefault;
        private Tween[] _listOfTweensActive;

        #endregion

        #region Unity Methods

        private void Awake()
        {
#if UNITY_EDITOR
            Debug.Log("Cargando Visual Key Manager...");         
#endif
            _listOfTweensActive = new Tween[_columns.Length];
            _whiteDefault = _columns[0].materials[4].color;
            _darkDefault = _columns[1].materials[1].color;
        }

        #endregion

        #region Public Methods
        
        /// <summary>
        /// Pasado una sucesion muestra la ilumincación de las notas con el color correspondiente
        /// </summary>
        /// <param name="sucesion"></param>
        public void PlaySequence(List<int> sucesion)
        {
            _isVisualComplete = false;
            _visualKeySequence = DOTween.Sequence();
            var indexMaterial = 0;
            Color colorToLight;
            var notaIndex = 0;
            foreach (var i in sucesion)
            {
                if (_columns[i].name.Contains("#")) indexMaterial = 1;
                else indexMaterial = 4;
                colorToLight = _showColorsOnColumns
                    ? _columns[i].GetComponent<ColorManager>().ColorColumna
                    : _defaultColor;

                var toLight = colorToLight;
                _visualKeySequence.Append(_columns[i].materials[indexMaterial]
                    .DOColor(colorToLight, _timeToLight)
                    .SetLoops(2, LoopType.Yoyo)
                    .SetEase(Ease.OutCubic)
                    .OnPlay(() =>
                    {
                        AudioManager.Instance.PlayClip(i);

                        UIManager.Instance.ShowNotaColorProgreso(toLight, notaIndex, _timeToLight*.8f);
                        notaIndex++;
                    }));
                _visualKeySequence.AppendInterval(i == sucesion[^1] ? 0.1f : secondsBetweenNotes());
            }
            
            _visualKeySequence.Play().OnComplete(() =>
            {
                _isVisualComplete = true; 
                UIManager.Instance.ShowFirstGroup();
            });
            StartCoroutine(controlSequenceCoroutine(_visualKeySequence)); 
        }

        /// <summary>
        /// Pasada una sucesion hace sonar las notas en orden
        /// </summary>
        /// <param name="sucesion"></param>
        public void PlaySucesionSound(List<int> sucesion)
        {
            StartCoroutine(playSoundsCoroutine(sucesion));
        }

        /// <summary>
        /// Ilumina la columna del color de la nota si es correcto, rojo si es incorrecto
        /// </summary>
        /// <param name="IsCorrect"></param>
        /// <param name="collider"></param>
        /// <returns></returns>
        public Color ShowHit(bool IsCorrect, Collider collider)
        { ;
            var indexMaterial = 0;
            var _renderer = collider.GetComponent<Renderer>();
            var HitColor = _colorHitWrong;
            if (collider.name.Contains("#")) indexMaterial = 1;
            else indexMaterial = 4;
            if (IsCorrect)
            {
                var colorManager = collider.GetComponent<ColorManager>();
                var index = colorManager.index;
                
                if(_listOfTweensActive[index] != null) _listOfTweensActive[index].Kill();
                
                if(indexMaterial == 1) _renderer.materials[indexMaterial].color = _darkDefault;
                else _renderer.materials[indexMaterial].color = _whiteDefault;
                
                HitColor = _showColorsOnColumns ? colorManager.ColorColumna : _defaultColor;
                
                _lastTween =_renderer.materials[indexMaterial]
                        .DOColor(HitColor, _timeToHitLight)
                        .SetLoops(2, LoopType.Yoyo)
                        .SetEase(Ease.OutCubic)
                        .Play()
                        .OnPlay(() =>
                        {
                            AudioManager.Instance.PlayClip(collider.name);
                        });
                
                _listOfTweensActive[index] = _lastTween;
            }
            else
            {
                _renderer.materials[indexMaterial]
                    .DOColor(HitColor, _timeToHitLight)
                    .SetLoops(2, LoopType.Yoyo)
                    .SetEase(Ease.OutCubic)
                    .Play()
                    .OnPlay(() => AudioManager.Instance.PlaySFXSound(AudioManager.SFX_type.Fail));
            }

            return IsCorrect ? HitColor : Color.white;
        }

        public void ShowCorrectNote(int correctIndex)
        {
            _isVisualComplete = false;
            var indexMaterial = 0;
            Color colorToLight;
            _visualKeySequence = DOTween.Sequence();
            if (_columns[correctIndex].name.Contains("#")) indexMaterial = 1;
            else indexMaterial = 4;
            colorToLight = _showColorsOnColumns
                ? _columns[correctIndex].GetComponent<ColorManager>().ColorColumna
                : _defaultColor;

            _visualKeySequence.Append(_columns[correctIndex].materials[indexMaterial]
                .DOColor(colorToLight, .4f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutCubic)
                .OnPlay(() => { AudioManager.Instance.PlayClip(correctIndex); }));
            
            _visualKeySequence.Append(_columns[correctIndex].materials[indexMaterial]
                .DOColor(colorToLight, .4f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutCubic)
                .OnPlay(() => { AudioManager.Instance.PlayClip(correctIndex); }));

            _visualKeySequence
                .Play()
                .OnComplete(() => { _isVisualComplete = true; });
            //StartCoroutine(controlSequenceCoroutine(_visualKeySequence)); 
        }

        /// <summary>
        /// Devuelve las columnas a su color original
        /// </summary>
        public void RestartColors()
        {
            foreach (var c in _columns)
            {
                if (c.name.Contains("#")) c.materials[1].color = _darkDefault;
                else c.materials[4].color = _whiteDefault;
            }
        }
        
        #endregion

        #region Private Methods
        /// <summary>
        /// Controla si se vuelve al menu principal para matar la sequence
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        private IEnumerator controlSequenceCoroutine(Sequence sequence)
        {
            while (true)
            {
                if (!CameraController.Instance.IsStarted)
                {
                    sequence.Kill();
                    yield break;
                }

                yield return 0;
            }
        }

        /// <summary>
        /// Coroutine que se encarga de hacer sonar la notas si la ayuda visual está deshabilidata
        /// </summary>
        /// <param name="sucesion"></param>
        /// <returns></returns>
        private IEnumerator playSoundsCoroutine(List<int> sucesion)
        {
            _isVisualComplete = false;
            bool firstNote = true;
            var indexMaterial = 0;
            Color colorToLight;
            bool isFirstCompleted = true;
            var notaIndex = 0;
            foreach (var i in sucesion)
            {
                yield return new WaitWhile(() => !isFirstCompleted);
                yield return new WaitWhile(() => GameplayManager.Instance.IsGameOnPause && !GameplayManager.Instance.IsGameOver);
                if (!CameraController.Instance.IsStarted || GameplayManager.Instance.IsGameOver) yield break;
                
                if (firstNote)
                {
                    firstNote = false;
                    isFirstCompleted = false;
                    if (_columns[i].name.Contains("#")) indexMaterial = 1;
                    else indexMaterial = 4;

                    colorToLight = _showColorsOnColumns ? _columns[i].GetComponent<ColorManager>().ColorColumna : _defaultColor;
                    
                    _columns[i].materials[indexMaterial].DOColor(colorToLight, _timeToLight)
                        .SetLoops(2, LoopType.Yoyo)
                        .SetEase(Ease.OutCubic)
                        .OnPlay((() =>
                        {
                            AudioManager.Instance.PlayClip(i);
                            UIManager.Instance.ShowNotaColorProgreso(colorToLight, notaIndex, _timeToLight*.8f);
                            notaIndex++;
                        }))
                        .Play()
                        .OnComplete(()=> isFirstCompleted = true);
                    continue;
                }
                yield return new WaitForSeconds(secondsBetweenNotes());
                
                AudioManager.Instance.PlayClip(i);
                UIManager.Instance.ShowNotaColorProgreso(_defaultColor, notaIndex, _timeToLight*.8f);
                
                notaIndex++;
                yield return new WaitForSeconds(_timeToLight * 2);
            }
            
            UIManager.Instance.ShowFirstGroup();
            _isVisualComplete = true;
        }

        private float secondsBetweenNotes()
        {
            return .65f - SaveManager.Instance.Options.VelocidadNotas * .02f; //A valor más alto menos segundos entre notas
        }

        #endregion
    }
}