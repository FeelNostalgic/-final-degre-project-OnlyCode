using UnityEngine;

namespace Proyecto.Manager
{
    public class ButtonFunctionlity : MonoBehaviour
    {
        #region Public Methods

        public void GoToNivelesButton()
        {
            sfxClick();
            UIManager.Instance.ShowNivelesFromMenuPrincipal();
        }

        public void GoToModoLibreFromMenuPrincipalButton()
        {
            sfxClick();
            UIManager.Instance.StartModoLibre();
        }

        public void GoToOptionsButton()
        {
            sfxClick();
            UIManager.Instance.ShowOptionFromMenuPrincipal();
        }

        public void PlayTutorialButton()
        {
            sfxClick();
            UIManager.Instance.ShowTutorialFromMenuPrincipal();
        }

        public void GoToCreditosButton()
        {
            sfxClick();
            UIManager.Instance.ShowCreditosFromMenuPrincipal();
        }

        public void SalirButton()
        {
            sfxClick();
            Application.Quit();
        }

        public void ReturnToMenuPrincipalButton()
        {
            sfxClick();
            UIManager.Instance.ReturnToMenuPrincipal();
        }

        public void PauseButton()
        {
            if (GameplayManager.Instance.IsGameOver) return;
            sfxClick();
            if (!GameplayManager.Instance.IsGameOnPause) UIManager.Instance.SetPausa(true);
            else UIManager.Instance.SetPausa(false);
        }

        public void ReturnToMenuPrincipalFromPausaButton()
        {
            sfxClick();
            UIManager.Instance.ReturnToMenuPrincipalFromPausa();
            Time.timeScale = 1;
        }

        public void ReturnToMenuPrincipalFromModoLibre()
        {
            sfxClick();
            UIManager.Instance.ReturnToMenuPrincipalFromModoLibre();
        }

        public void GoToMenuNivelesFromFinalScoreButton()
        {
            sfxClick();
            UIManager.Instance.ShowMenuNivelesFromMenuPrincipal();
        }

        #endregion

        #region Private Methods

        private void sfxClick()
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_type.Click);
        }        

        #endregion
    }
}