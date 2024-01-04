
using System.Collections.Generic;

namespace Proyecto.Levels
{
    public abstract class Level
    {
        public List<int> Sucesion;
        
        private const int NOTAS_INICIALES = 3;

        public void StartLevel()
        {
            Sucesion = new List<int>();
            for (int i = 0; i < NOTAS_INICIALES; i++) //Se seleccionan valores aleatorios
            {
                AddNuevaNota();
            }
        }
        public abstract void AddNuevaNota();
    }
}

