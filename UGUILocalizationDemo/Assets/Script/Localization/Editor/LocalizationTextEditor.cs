/*
 * ==============================================================================
 * File Name: LocalizationTextEditor.cs
 * Description: 用来扩展Text，增加显示属性
 * 
 * Version 1.0
 * Create Time: 30/08/2018 12:00
 * 
 * Author: taihe
 * Company: DefaultCompany
 * ==============================================================================
*/

using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;

[CustomEditor(typeof(LocalizationText))]
public class LocalizationTextEditor : UnityEditor.UI.TextEditor
{

    public override void OnInspectorGUI()
    {
        LocalizationText component = (LocalizationText)target;
        base.OnInspectorGUI();
        component.KeyString = EditorGUILayout.TextField("Key String", component.KeyString);
        component.CustomFont = (UIFont)EditorGUILayout.ObjectField("Custom Font", component.CustomFont, typeof(UIFont), true);
    }
}
