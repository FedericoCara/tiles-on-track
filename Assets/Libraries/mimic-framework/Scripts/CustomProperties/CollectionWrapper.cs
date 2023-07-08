using System;
using Mimic.Internal;

//Credits https://github.com/Deadcows/MyBox/blob/master/Types/CollectionWrapper.cs

namespace Mimic
{
	[Serializable]
	public class CollectionWrapper<T> : CollectionWrapperBase
	{
		public T[] Value;
	}
}

namespace Mimic.Internal
{
	[Serializable]
	public class CollectionWrapperBase { }
}

#if UNITY_EDITOR
namespace Mimic.Internal
{
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(CollectionWrapperBase), true)]
	public class CollectionWrapperDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			var collection = property.FindPropertyRelative("Value");
			return EditorGUI.GetPropertyHeight(collection, true);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			var collection = property.FindPropertyRelative("Value");
			EditorGUI.PropertyField(position, collection, label, true);
		}
	}
}
#endif