/*
 * ==============================================================================
 * File Name: UGUITest.cs
 * Description: 
 * 
 * Version 1.0
 * Create Time: 30/08/2018 15:05
 * 
 * Author: taihe
 * Company: DefaultCompany
 * ==============================================================================
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UGUITest : MonoBehaviour {

	public void ChangeCN()
    {
        Localization.SwitchLanguage("zh-Hans");
    }

    public void ChangeEN()
    {
        Localization.SwitchLanguage("en");
    }
}
