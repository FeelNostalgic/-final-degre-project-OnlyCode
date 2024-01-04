using Proyecto.Manager;
using UnityEngine;

namespace Proyecto.Levels.Children
{
    public class Level_4 : Level
    {
        #region Public Methods

        public override void AddNuevaNota() //G, A, B
        {
            int randomKey = Random.Range(0, 5);
            var aux = 0;
            switch (randomKey)
            {
                case 0:
                    aux = (int)GameplayManager.Keys.G4_67; //G4
                    break;
                case 1:
                    aux = (int)GameplayManager.Keys.A3_57; //A3
                    break;
                case 2:
                    aux = (int)GameplayManager.Keys.A4_69; //A4
                    break;
                case 3:
                    aux = (int)GameplayManager.Keys.B3_59; //B3
                    break;
                case 4:
                    aux = (int)GameplayManager.Keys.B4_71; //B4
                    break;
            }

            Sucesion.Add(aux);
        }

        #endregion
    }
}