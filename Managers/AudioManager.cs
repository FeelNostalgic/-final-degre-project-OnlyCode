using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Proyecto.Manager
{
    public class AudioManager : Singleton<AudioManager>
    {
        #region Inspector Methods

        [Header("Music")] 
        [SerializeField] private AudioSource _kidAudioSource;
        [SerializeField] private List<AudioClip> _kidAudiosSostenidos;
        [SerializeField] private List<AudioClip> _kidAudiosBemoles;

        [Header("SFX")] 
        [SerializeField] private List<AudioClip> _sfxClipList;
        [SerializeField] private AudioSource _sfxAudioSource;

        #endregion

        #region Public Variables

        public bool IsKid => _isKid;

        public enum SFX_type
        {
            Fail,
            Click,
            Victory
        }
        
        #endregion

        #region Private Variables

        private bool _isKid;

        #endregion

        #region Unity Methods

        private void Awake()
        {
#if UNITY_EDITOR
            Debug.Log("Cargando Audio Manager...");         
#endif
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Hacer sonar una nota dado el nombre de la columna
        /// (Usado para mostrar el hit)
        /// </summary>
        /// <param name="key"></param>
        public void PlayClip(string key)
        {
            if (IsKid)
            {
                var i = findAudioClipIndex(key);
                _kidAudioSource.PlayOneShot(SaveManager.Instance.Options.IsSostenido ? _kidAudiosSostenidos[i] : _kidAudiosBemoles[i]);
            }
            else
            {
                var i = findSoundMIDIIndex(key);
                var duration = _kidAudiosSostenidos[0].length  * calculatePercentOfDuration(); // VisualKeyManager.Instance.TimeToLight;
                MidiPlayer.MidiPlayer.Instance.PlayNote(i, (long)duration * 1000);
            }
        }

        /// <summary>
        /// Hace sonar una nota dado el indice de la columna
        /// (Usado para mostrar la sucesion)
        /// </summary>
        /// <param name="key"></param>
        public void PlayClip(int key)
        {
            
            if (IsKid)
            {
                _kidAudioSource.Stop();
                _kidAudioSource.PlayOneShot(SaveManager.Instance.Options.IsSostenido ? _kidAudiosSostenidos[key] : _kidAudiosBemoles[key]);
            }
            else
            {
                var duration = _kidAudiosSostenidos[0].length * calculatePercentOfDuration();
                var midi = key + 57;
                MidiPlayer.MidiPlayer.Instance.PlayNote(midi, (long)duration * 1000);
            }
        }

        /// <summary>
        /// Cambia el intrumento del midiplayer o kid
        /// </summary>
        /// <param name="index"> 0 -> Piano | 1 -> Kid | 2 -> harpsichord | 3 -> Vibraphono | 4 -> Pipe Organ | 5 -> Harmonica | 6 -> Overdrive Guitar | 7 -> Violin | 8 -> Concept choir | 9 -> Oboe | 10 -> Piccolo </param>
        public void ChangeInstrument(int index)
        {
            if (index == 1)
            {
                _isKid = true;
            }
            else
            {
                _isKid = false;
                MidiPlayer.MidiPlayer.Instance.ChangeInstrument(index == 0 ? index : index - 1);
            }
        }

        public void PlaySFXSound(SFX_type type)
        {
            _sfxAudioSource.volume = 1;
            _sfxAudioSource.pitch = 1;
            switch (type)
            {
                case SFX_type.Fail:
                    playSFXSound((int)type);
                    break;
                case SFX_type.Click:
                    _sfxAudioSource.pitch = Random.Range(1, 1.3f);
                    playSFXSound((int)type);
                    break;
                case SFX_type.Victory:
                    _sfxAudioSource.volume = 0.75f;
                    playSFXSound((int)type);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        #endregion

        #region Private Methods

        private void playSFXSound(int index)
        {
            _sfxAudioSource.PlayOneShot(_sfxClipList[index]);
        }

        private int findSoundMIDIIndex(string key)
        {
            for (int i = 57; i < 77; i++)
            {
                if (key.Contains(i.ToString()))
                {
                    return i;
                }
            }

            return -1;
        }

        private int findAudioClipIndex(string key)
        {
            var column = VisualKeyManager.Instance.Columns;
            for (int i = 0; i < column.Length; i++)
            {
                if (column[i].name == key)
                {
                    return i;
                }
            }

            return -1;
        }

        private float calculatePercentOfDuration()
        {
            return .9f - SaveManager.Instance.Options.VelocidadNotas * .08f;//A valor más alto menos segundos dura la nota
        }
        
        #endregion
    }
}