using Proyecto.Manager;
using UnityEngine;

namespace Proyecto.Levels.Children
{
    public class Level_1 : Level
    {
        #region Public Methods
        
        public override void AddNuevaNota() //C, E, G
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
                    aux = (int)GameplayManager.Keys.E4_64; //E4
                    break;
                case 3:
                    aux = (int)GameplayManager.Keys.E5_76; //E5
                    break;
                case 4:
                    aux = (int)GameplayManager.Keys.G4_67; //G4
                    break;
            }

            Sucesion.Add(aux);
        }
        
        #endregion
    }
}