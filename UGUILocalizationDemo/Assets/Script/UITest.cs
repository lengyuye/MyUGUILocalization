using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITest : MonoBehaviour
{

    [SerializeField]
    protected LocalizationText _dynamicText;
    // Use this for initialization
    void Start()
    {
        OnLocalize();
    }

    /// <summary>
    /// 本地化
    /// </summary>
    public void OnLocalize()
    {
        if(_dynamicText)
        {
            _dynamicText.text = string.Format(Localization.Get(_dynamicText.KeyString), "123456");
        }
    }

    protected void OnEnable()
    {
        LocalizationManager.OnLocalize += OnLocalize;
    }
    protected void OnDisable()
    {
        if (LocalizationManager.OnLocalize != null)
        {
            LocalizationManager.OnLocalize -= OnLocalize;
        }

    }
}
