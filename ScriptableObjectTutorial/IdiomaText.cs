using System;
using UnityEngine;

namespace Proyecto.ScriptableObjects.Idioma
{
	[Serializable]
	public class IdiomaText
	{
		#region Public Variables
		
		[TextArea(1, 2)]
		public string TextIdioma;

		#endregion
	}
}