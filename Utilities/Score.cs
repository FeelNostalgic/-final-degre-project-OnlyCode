using System;
using System.Collections.Generic;

namespace Proyecto.Utility
{
    [Serializable]
    public class Score
    {
        #region Public Variables

        public List<StarsAndLyres> LevelList;

        #endregion

        #region Public Methods

        public Score()
        {
            LevelList = new List<StarsAndLyres>();
            for (int i = 0; i < 11; i++)
            {
                LevelList.Add(new StarsAndLyres());
            }
        }

        #endregion
    }

    [Serializable]
    public class StarsAndLyres
    {
        public int Stars;
        public int Lyres;
    }
}