using System;
using System.IO;
using Proyecto.Utility;
using UnityEngine;

namespace Proyecto.Manager
{
    public class SaveManager : Singleton<SaveManager>
    {
        #region Inpector Variables 

        [SerializeField] private bool DoTutorial;
        
        #endregion

        #region Public Variables

        public bool PlayTutorial
        {
            get => _playTutorial;
        }

        public Score Score => _score;
        public Options Options => _options;

        #endregion

        #region Private Variables
        
        private bool _playTutorial;
        private Score _score;
        private Options _options;
        private string _pathScore;
        private string _pathOptions;

        #endregion

        #region Unity Methods

        private void Awake()
        {
#if UNITY_EDITOR
            Debug.Log("Cargando Save Manager...");         
#endif
            Application.targetFrameRate = 60;
            #if UNITY_EDITOR
            //Debug.Log(Application.persistentDataPath);
            _pathScore =  "score.json";
            _pathOptions = "options.json";
            #else
            _pathScore = Application.persistentDataPath + "/score.json";
            _pathOptions = Application.persistentDataPath + "/options.json";
            #endif
            
            //Cargar datos guardados previamente
            _score = (Score)loadFile<Score>(_pathScore);
            _options = (Options)loadFile<Options>(_pathOptions);

            //Se crea el archivo serializado si no existe
            if (_score == null) //Es la primera vez que se inicia la aplicación
            {
                _playTutorial = true;
                _score = new Score();
                saveFile(_score, _pathScore);
            }
            else
            {
#if UNITY_EDITOR
                _playTutorial = DoTutorial;
#else
                _playTutorial = false;
#endif
            }

            if (_options == null) //Es la primera vez que se inicia la aplicación
            {
                _options = new Options
                {
                    IsVisualHelp = true,
                    IsSostenido = true,
                    IsClasica = true,
                    IsColores = true,
                    Instrument = 0,
                    Volumen = 1f,
                    VelocidadNotas = 3,
                };
                UpdateOptions();
            }
            startOptions();
        }

        #endregion

        #region Public Methods
        
        /// <summary>
        /// Guarda la nueva puntuacion 
        /// </summary>
        /// <param name="level">Nivel a actualizar</param>
        /// <param name="stars">Coronas a actualizar</param>
        /// <param name="lyres">Lyres a actualizar</param>
        public void SaveScore(int level, int stars = -1 , int lyres = -1)
        {
            var currentStars = -1;
            var currentLyres = -1;
            if(stars>0) currentStars = _score.LevelList[level - 1].Stars;
            if(lyres>0) currentLyres = _score.LevelList[level - 1].Lyres;
#if UNITY_EDITOR
            Debug.Log($"Actualizando informacion del nivel {level}: currentStars: {currentStars}, newStars: {stars}, currentLyres: {currentLyres}, newLyres: {lyres} ");
#endif
            //Se compara para guardar la mejor puntuación
            if (stars > -1 && stars > currentStars)
            {
                _score.LevelList[level - 1].Stars = stars;
                saveFile(_score, _pathScore);
                return;
            }

            if (lyres > -1 && lyres > currentLyres)
            {
                _score.LevelList[level - 1].Lyres = lyres;
                saveFile(_score, _pathScore);
            }
        }

        public void UpdateOptions()
        {
            saveFile(_options, _pathOptions);
        }

        #endregion

        #region Private Methods
        
        private void saveFile<T>(T fileToSave, string path)
        {
            //Debug.Log("Guardando informacion: " + fileToSave.GetType());
            string json = JsonUtility.ToJson(fileToSave); 
            File.WriteAllText(path, json);
        }

        private object loadFile<T>(string path)
        {
            // Debug.Log("Cargando informacion");
            try
            {
                using (StreamReader reader = new StreamReader(File.Open(path, FileMode.OpenOrCreate)))
                {
                    return JsonUtility.FromJson<T>(reader.ReadToEnd());
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return null;
        }

        private void startOptions()
        {
            StartCoroutine(VolumeManager.Instance.Start());
            StartCoroutine(VelocidadManager.Instance.Start());
        }
        #endregion
    }
}