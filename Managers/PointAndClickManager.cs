using Proyecto.Utility;
using UnityEngine;

namespace Proyecto.Manager
{
    public class PointAndClickManager : MonoBehaviour
    {
        #region Public variables

        [SerializeField] private LayerMask _layerKeys;

        #endregion

        #region Private variables

        private Ray _ray;
        private RaycastHit _hit;

        #endregion

        #region Unity Methods

        private void Update()
        {
            if (!GameplayManager.Instance.IsModoLibre) InputPointAndHitRaycast();
            else InputPointAndClickModoLibre();
        }

        #endregion

        #region Private Methods

        private void InputPointAndHitRaycast()
        {
            if (CameraController.Instance.IsStarted && !GameplayManager.Instance.IsGameOnPause &&
                VisualKeyManager.Instance
                    .IsVisualComplete) //&& VisualKeyManager.Instance.IsFinishFeedback && !GameplayManager.Instance.IsGameOver
            {
                _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(_ray, out _hit, _layerKeys))
                {
                    if (Input.GetMouseButtonDown(0)) //Boton izquierdo
                    {
                        GameplayManager.Instance.CheckPattern(_hit.collider);
                    }
                }
                
#if UNITY_EDITOR
                Debug.DrawRay(_ray.origin, _ray.direction * 100, Color.red);
#endif
            }
        }

        private void InputPointAndClickModoLibre()
        {
            if (CameraController.Instance.IsStarted && !GameplayManager.Instance.IsGameOnPause)
            {
                _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(_ray, out _hit, _layerKeys))
                {
                    if (Input.GetMouseButtonDown(0)) //Boton izquierdo
                    {
                        VisualKeyManager.Instance.ShowHit(true, _hit.collider);
                    }
                }

#if UNITY_EDITOR
                Debug.DrawRay(_ray.origin, _ray.direction * 100, Color.red);
#endif
            }
        }
        
        #endregion
    }
}