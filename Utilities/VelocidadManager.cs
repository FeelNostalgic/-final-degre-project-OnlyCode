using System.Collections;
using Proyecto.Manager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Proyecto.Utility
{
    public class VelocidadManager : Singleton<VelocidadManager>, IPointerDownHandler
    {
        [SerializeField] private Slider _slider; // Slider con valores 0.001 -> 1

        public IEnumerator Start()
        {
#if UNITY_EDITOR
            Debug.Log("Cargando Velocidad Manager...");         
#endif
            _slider.value = SaveManager.Instance.Options.VelocidadNotas; //Cargar opciones desde json
            updateVelocidad(SaveManager.Instance.Options.VelocidadNotas);
            _slider.onValueChanged.AddListener(updateVelocidad);
            yield return 0;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_type.Click);
        }
        
        private void updateVelocidad(float value)
        {
            int i = (int) value;
            switch (i)
            {
                case 1:
                    VisualKeyManager.Instance.TimeToLight = .9f; break; //Old: 1f
                case 2:
                    VisualKeyManager.Instance.TimeToLight = 0.75f; break; //Old: .8f
                case 3:
                    VisualKeyManager.Instance.TimeToLight = 0.55f; break; //Old: .6f
                case 4:
                    VisualKeyManager.Instance.TimeToLight = 0.45f; break; //Old: .4f
                case 5:
                    VisualKeyManager.Instance.TimeToLight = 0.3f; break; //Old: .25f
            }
            
            SaveManager.Instance.Options.VelocidadNotas = i;
        }
    }
}