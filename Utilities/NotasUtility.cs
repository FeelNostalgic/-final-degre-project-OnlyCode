using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Proyecto.Utility
{
    public class NotasUtility : MonoBehaviour
    {
        #region Inspector Variables

        [SerializeField] private List<Image> _notasImagenes;

        #endregion

        public List<Image> NotasImagenes => _notasImagenes;
    }
}