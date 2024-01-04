using Proyecto.Manager;
using UnityEngine;

namespace Proyecto.Levels.Children
{
    public class Level_3 : Level
    {
        #region Public Methods

        public override void AddNuevaNota() //C, F, A
        {
            int randomKey = Random.Range(0, 5);
            var aux = 0;
            switch (randomKey)
            {
                case 0:
                    aux = (int)GameplayManager.Keys.C4_60; //C4
                    break;
                case 1:
                    aux = (int)GameplayManager.Keys.C5_72; //C5
                    break;
                case 2:
                    aux = (int)GameplayManager.Keys.F4_65; //F4
                    break;
                case 3:
                    aux = (int)GameplayManager.Keys.A3_57; //A3
                    break;
                case 4:
                    aux = (int)GameplayManager.Keys.A4_69; //A4
                    break;
            }

            Sucesion.Add(aux);
        }

        #endregion
    }
}