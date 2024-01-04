using Proyecto.Manager;
using UnityEngine;

namespace Proyecto.Levels.Children
{
    public class Level_6 : Level
    {
        #region Public Methods

        public override void AddNuevaNota() //C, D, E, F, G, A
        {
            int randomKey = Random.Range(0, 10);
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
                    aux = (int)GameplayManager.Keys.D4_62; //D4
                    break;
                case 3:
                    aux = (int)GameplayManager.Keys.D5_74; //D5
                    break;
                case 4:
                    aux = (int)GameplayManager.Keys.E4_64; //E4
                    break;
                case 5:
                    aux = (int)GameplayManager.Keys.E5_76; //E5
                    break;
                case 6:
                    aux = (int)GameplayManager.Keys.G4_67; //G4
                    break;
                case 7:
                    aux = (int)GameplayManager.Keys.A3_57; //A3
                    break;
                case 8:
                    aux = (int)GameplayManager.Keys.A4_69; //A4
                    break;
                case 9:
                    aux = (int)GameplayManager.Keys.F4_65; //F4
                    break;
            }

            Sucesion.Add(aux);
        }

        #endregion
    }
}