using System.Collections.Generic;
using UnityEngine;

namespace Proyecto.ScriptableObjects.Idioma 
{
	[CreateAssetMenu(fileName = "IdiomaData", menuName = "Scriptable/Idioma")]
	public class IdiomaScriptableObject  : ScriptableObject
	{
		public List<IdiomaText> IdiomaTextList;
	}
}