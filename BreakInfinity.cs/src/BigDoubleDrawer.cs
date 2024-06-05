// Copyright 2024 Joonhyo Choi (asdfchlwnsgy1236); Apache License Version 2.0.

#if UNITY_2021_2_OR_NEWER && UNITY_EDITOR

namespace BreakInfinity {
	using UnityEditor;

	using UnityEngine;

	[CustomPropertyDrawer(typeof(BigDouble))]
	public class BigDoubleDrawer: PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
			int indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			const float GapWidth = 8;
			float width = (position.width - GapWidth) / 2;
			Rect mr = new(position.x, position.y, width, position.height), gr = new(mr.x + mr.width, position.y, GapWidth, position.height), er = new(gr.x + gr.width, position.y, width, position.height);
			SerializedProperty mp = property.FindPropertyRelative("Mantissa"), ep = property.FindPropertyRelative("Exponent");
			EditorGUI.BeginChangeCheck();
			double m = EditorGUI.DelayedDoubleField(mr, GUIContent.none, mp.doubleValue);
			EditorGUI.LabelField(gr, "e");
			double e = EditorGUI.DelayedDoubleField(er, GUIContent.none, ep.doubleValue);
			if(EditorGUI.EndChangeCheck()) {
				BigDouble n = new(m, e);
				mp.doubleValue = n.Mantissa;
				ep.doubleValue = n.Exponent;
			}
			EditorGUI.indentLevel = indent;
			EditorGUI.EndProperty();
		}
	}
}

#endif