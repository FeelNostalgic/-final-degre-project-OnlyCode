using System.Collections;
using Proyecto.Manager;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

namespace Proyecto.Utility
{
    public class VolumeManager : Singleton<VolumeManager>, IPointerDownHandler
    {
        [SerializeField] private AudioMixer _mixer; // Exponer variable del mixer
        [SerializeField] private Slider _slider; // Slider con valores 0.001 -> 1

        public IEnumerator Start()
        {
#if UNITY_EDITOR
            Debug.Log("Cargando Volumen Manager...");         
#endif
            _slider.value = SaveManager.Instance.Options.Volumen; //Cargar opciones desde json
            _mixer.SetFloat("MusicVolume", 20 * Mathf.Log10(_slider.value));
            _slider.onValueChanged.AddListener(updateVolume);
            yield return 0;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_type.Click);
        }
        
        private void updateVolume(float value)
        {
            _mixer.SetFloat("MusicVolume", 20 * Mathf.Log10(value));
            SaveManager.Instance.Options.Volumen = value;
        }
    }
}