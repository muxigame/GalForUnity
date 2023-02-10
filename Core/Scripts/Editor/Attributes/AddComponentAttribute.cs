using System;
using UnityEngine;

namespace GalForUnity.Core.Editor.Attributes {
	public class AddComponentAttribute : PropertyAttribute {
		public Type type1;
		public AddComponentAttribute(Type type){
			type1 = type;
		}

	}
}
