using System.Collections;
using Proyecto.Levels.Children;
using Proyecto.Utility;
using UnityEngine;

namespace Proyecto.Manager
{
    public class LevelsManager : MonoBehaviour
    {
        #region Private Methods

        private int _currentLevel;

        #endregion

        #region Public Methods

        public void StartLevel(int level)
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_type.Click);
            _currentLevel = level;
            UIManager.Instance.SetActiveNivelesPanel(false);
            CameraController.Instance.MoveToGameplayPosition();

            GameplayManager.Instance.IsCountLimited = true;   
            switch (level)
            {
                case 1:
                    GameplayManager.Instance.StartNewLevel(new Level_1(), 1);
                    break;
                case 2:
                    GameplayManager.Instance.StartNewLevel(new Level_2(), 2);
                    break;
                case 3:
                    GameplayManager.Instance.StartNewLevel(new Level_3(), 3);
                    break;
                case 4:
                    GameplayManager.Instance.StartNewLevel(new Level_4(), 4);
                    break;
                case 5:
                    GameplayManager.Instance.StartNewLevel(new Level_5(), 5);
                    break;
                case 6:
                    GameplayManager.Instance.StartNewLevel(new Level_6(), 6);
                    break;
                case 7:
                    GameplayManager.Instance.StartNewLevel(new Level_7(), 7);
                    break;
                case 8:
                    GameplayManager.Instance.StartNewLevel(new Level_8(), 8);
                    break;
                case 9:
                    GameplayManager.Instance.StartNewLevel(new Level_9(), 9);
                    break;
                case 10:
                    GameplayManager.Instance.StartNewLevel(new Level_10(), 10);
                    break;
                case 11:
                    GameplayManager.Instance.StartNewLevel(new Level_10(), 11);
                    break;
            }
        }

        public void RestartLevel()
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_type.Click);
            UIManager.Instance.SetActiveFinalScorePanel(false);
            StartCoroutine(restartLevelCoroutine());
        }

        #endregion

        #region Private Methods

        private IEnumerator restartLevelCoroutine()
        {
            VisualKeyManager.Instance.RestartColors();
            yield return new WaitForSeconds(1f);
            GameplayManager.Instance.IsCountLimited = true;
            
            switch (_currentLevel)
            {
                case 1:
                    GameplayManager.Instance.StartNewLevel(new Level_1(), 1);
                    break;
                case 2:
                    GameplayManager.Instance.StartNewLevel(new Level_2(), 2);
                    break;
                case 3:
                    GameplayManager.Instance.StartNewLevel(new Level_3(), 3);
                    break;
                case 4:
                    GameplayManager.Instance.StartNewLevel(new Level_4(), 4);
                    break;
                case 5:
                    GameplayManager.Instance.StartNewLevel(new Level_5(), 5);
                    break;
                case 6:
                    GameplayManager.Instance.StartNewLevel(new Level_6(), 6);
                    break;
                case 7:
                    GameplayManager.Instance.StartNewLevel(new Level_7(), 7);
                    break;
                case 8:
                    GameplayManager.Instance.StartNewLevel(new Level_8(), 8);
                    break;
                case 9:
                    GameplayManager.Instance.StartNewLevel(new Level_9(), 9);
                    break;
                case 10:
                    GameplayManager.Instance.StartNewLevel(new Level_10(), 10);
                    break;
                case 11:
                    GameplayManager.Instance.StartNewLevel(new Level_10(), 11);
                    break;
            }
        }

        #endregion
    }
}