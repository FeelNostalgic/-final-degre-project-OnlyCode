using System.Collections;
using Proyecto.Levels;
using Proyecto.Utility;
using UnityEngine;

namespace Proyecto.Manager
{
    public class GameplayManager : Singleton<GameplayManager>
    {
        #region Inspector Variables

        [SerializeField] private int _maxNotas;
        [SerializeField] private float _timeBetweenRounds;

        #endregion

        #region Public variables

        public bool IsGameOnPause
        {
            get =>_isGameOnPause;
            set => _isGameOnPause = value;
        }

        public bool ShowVisualHelp
        {
            get => _showVisualHelp;
            set => _showVisualHelp = value;
        }
        public bool IsModoLibre
        {
            get => _isModoLibre;
            set => _isModoLibre = value;
        }

        public bool IsCountLimited
        {
            get => _isCountLimited;
            set => _isCountLimited = value;
        }

        public bool IsGameOver => _isGameOver;
        
        public enum Keys 
        {
            A3_57,
            A3_Bb_58,
            B3_59,
            C4_60,
            C4_Db_61,
            D4_62,
            D4_Eb_63,
            E4_64,
            F4_65,
            F4_Gb_66,
            G4_67,
            G4_Ab_68,
            A4_69,
            A4_Bb_70,
            B4_71,
            C5_72,
            C5_Db_73,
            D5_74,
            D5_Eb_75,
            E5_76
        }
        
        #endregion

        #region Private variables
        
        private bool _isGameOnPause;
        private bool _showVisualHelp;
        private bool _isModoLibre;
        private bool _isCountLimited;
        private bool _isGameOver;
        private int _currentIndex;
        private int _currentNotas;
        private float _timerBewteenRounds;
        private Level _currentLevel;
        private bool _isANewKey;
        private int _levelNum;
        
        #endregion

        #region Unity Methods

        private void Start()
        {
#if UNITY_EDITOR
            Debug.Log("Cargando Gameplay Manager...");         
#endif
            _timerBewteenRounds = _timeBetweenRounds * .5f;
            _isGameOnPause = true;
        }

        private void Update()
        {
            if (_isANewKey && CameraController.Instance.IsStarted && !_isGameOnPause)
            {
                if (_timerBewteenRounds > _timeBetweenRounds)
                {
                    _timerBewteenRounds = 0;

                #if UNITY_EDITOR
                    showSucesionNotasEnLog();
                #endif
                    
                    //Mostar notas en teclas
                    if (_showVisualHelp) VisualKeyManager.Instance.PlaySequence(_currentLevel.Sucesion);
                    else VisualKeyManager.Instance.PlaySucesionSound(_currentLevel.Sucesion);
                    
                    _isANewKey = false;
                }
                else
                {
                    _timerBewteenRounds += Time.deltaTime;
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Funcion para comprobar si el jugador esta pulsado correctamente el patron
        /// </summary>
        /// <param name="pulsedKey"></param>
        public void CheckPattern(Collider pulsedKey)
        {
            var sucesion = _currentLevel.Sucesion;
            var pulsedKey_Name = pulsedKey.name;
            var column_Name = VisualKeyManager.Instance.Columns[sucesion[_currentIndex]].name;
//        Debug.Log("Valor pulsado: " +  pulsedKey_Name + " | valor esperado: " + column_Name);
            if (pulsedKey_Name == column_Name) //Acierta la nota
            {
                var color = VisualKeyManager.Instance.ShowHit(true, pulsedKey);

                _currentIndex++; //Se pasa a la siguiente nota de la secuencia
                if(!color.Equals(Color.white)) UIManager.Instance.UpdateColorNota(color);
                if (_currentIndex >= sucesion.Count) //Si no quedan más notas en la secuencia se añade una nueva nota
                {
                    _currentIndex = 0;
                    _currentNotas++;

                    if (_currentNotas >= _maxNotas && _isCountLimited) //Se ha completado el nivel
                    {
                        //mostar resultado por UI
                        StartCoroutine(setScoreCoroutine());
                        return;
                    }
                    
                    _currentLevel.AddNuevaNota();
                    
                    StartCoroutine(showRondaCoroutine());
                }

                return;
            }

            //Falla la nota
            StartCoroutine(finalScoreFromWrongHitCoroutine(pulsedKey, sucesion[_currentIndex]));
        }

        public void StartNewLevel(Level newLevel, int levelNum)
        {
            _currentLevel = newLevel;
            _currentLevel.StartLevel();
            _levelNum = levelNum;
            _currentNotas = 2;
            _currentIndex = 0;
            _isANewKey = true;
            _isGameOnPause = false;
            _isGameOver = false;
            if(levelNum == 11) _isCountLimited = false;
            UIManager.Instance.RestartRondaActual();
            UIManager.Instance.SetupProgresoNotas();
        }

        #endregion

        #region Private Methods

        private IEnumerator showRondaCoroutine()
        {
            VisualKeyManager.Instance.IsVisualComplete = false;
            UIManager.Instance.UpdateRondaActual(_currentNotas - 2);
            yield return new WaitForSeconds(0.15f);
            StartCoroutine(UIManager.Instance.UpdateProgresoNotas());
            yield return new WaitForSeconds(0.15f);
            _isANewKey = true;
        }
        
        /// <summary>
        /// Funcion que muestra la sucesion de notas
        /// </summary>
        private void showSucesionNotasEnLog()
        {
            Debug.Log("================================");
            var sucesion = _currentLevel.Sucesion;
            for (int i = 0; i < sucesion.Count; i++) //Mostrar los valores elegidos
            {
                Debug.Log((Keys)sucesion[i]);
            }
        }

        /// <summary>
        /// Funcion que comprueba la puntuacion
        /// </summary>
        private IEnumerator setScoreCoroutine()
        {
            _isGameOnPause = true;
            
            if (!_isCountLimited)
            {
                if (_showVisualHelp) SaveManager.Instance.SaveScore(_levelNum,stars:_currentNotas-2);
                else SaveManager.Instance.SaveScore(_levelNum, lyres:_currentNotas-2);
                
                UIManager.Instance.SetFinalScore();
                yield break;
            }
            
            var stars = 0;
            var lyres = 0;
            if (_currentNotas == 5 || _currentNotas == 6)
            {
                if (_showVisualHelp) stars = 1;
                else lyres = 1;
            }

            if (_currentNotas == 7 || _currentNotas == 8)
            {
                if (_showVisualHelp) stars = 2;
                else lyres = 2;
            }

            if (_currentNotas == 9 || _currentNotas == 10)
            {
                if (_showVisualHelp) stars = 3;
                else lyres = 3;
            }

            if (_currentNotas == 10)
            {
                UIManager.Instance.UpdateRondaActual(_currentNotas - 2);
                AudioManager.Instance.PlaySFXSound(AudioManager.SFX_type.Victory);
                yield return new WaitForSeconds(1.35f);
            }
            
            if (_showVisualHelp)
            {
                SaveManager.Instance.SaveScore(_levelNum, stars: stars);
                UIManager.Instance.SetFinalScore(stars);
            }
            else
            {
                SaveManager.Instance.SaveScore(_levelNum, lyres: lyres);
                UIManager.Instance.SetFinalScore(lyres:lyres);
            }
        }

        private IEnumerator finalScoreFromWrongHitCoroutine(Collider pulsedKey, int correctNote)
        {
            _isGameOnPause = true;
            _isGameOver = true;
            VisualKeyManager.Instance.ShowHit(false, pulsedKey);
            yield return new WaitForSeconds(.35f);
            VisualKeyManager.Instance.ShowCorrectNote(correctNote);
            yield return new WaitWhile(() => !VisualKeyManager.Instance.IsVisualComplete);
            _timerBewteenRounds = _timeBetweenRounds*2;
            StartCoroutine(setScoreCoroutine());
        }

        #endregion
    }
}