﻿using RoR2EditorKit.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace RoR2EditorKit.Core.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(EnabledAndDisabledInspectorsSettings.InspectorSetting))]
    public class InspectorSettingPropertyDrawer : EditorGUILayoutPropertyDrawer
    {
        protected override void DrawPropertyDrawer(SerializedProperty property)
        {
            var isEnabled = property.FindPropertyRelative("isEnabled");
            var displayName = property.FindPropertyRelative("inspectorName");

            EditorGUILayout.PropertyField(isEnabled, new GUIContent(ObjectNames.NicifyVariableName(displayName.stringValue)));
        }
    }
}
