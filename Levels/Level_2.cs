using Proyecto.Manager;
using UnityEngine;

namespace Proyecto.Levels.Children
{
    public class Level_2 : Level
    {
        #region Public Methods
        
        public override void AddNuevaNota() // C, D, E
        {
            int randomKey = Random.Range(0, 6);
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
            }

            Sucesion.Add(aux);
        }
        
        #endregion
    }
}