using UnityEngine;

namespace Proyecto.Levels.Children
{
    public class Level_10 : Level
    {
        #region Public Methods

        public override void AddNuevaNota() //C, C#, D, D#, E, F, F#, G, G#, A, A#, B
        {
            int randomKey = Random.Range(0, 20);
#if UNITY_EDITOR
            randomKey = 3;
#endif
            Sucesion.Add(randomKey);
        }

        #endregion
    }
}