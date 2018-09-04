/*
 * ==============================================================================
 * File Name: LocalizationManager.cs
 * Description: 文本本地化管理器
 * 
 * Version 1.0
 * Create Time: 30/08/2018 14:53
 * 
 * Author: taihe
 * Company: DefaultCompany
 * ==============================================================================
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager
{
    public delegate void GameVoidDelegate();
    public static GameVoidDelegate OnLocalize;

    public const string kUILayerName = "UI";

    /// <summary>
    /// 初始化值为了和Text的初始值保持一致
    /// </summary>
    /// <param name="txt"></param>
    public static void InitValue(LocalizationText txt)
    {
        txt.color = new Color(50f / 255f, 50f / 255f, 50f / 255f);
        RectTransform contentRT = txt.GetComponent<RectTransform>();
        contentRT.sizeDelta = new Vector2(160f, 30f);
        txt.gameObject.layer = LayerMask.NameToLayer(kUILayerName);
    }
}
