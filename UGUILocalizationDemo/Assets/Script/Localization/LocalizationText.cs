using System;
using UnityEngine.UI;
using UnityEngine;
/// <summary>
/// Custom Text Control used for localization text.
/// </summary>
[AddComponentMenu("UI/Text_Local", 10)]
public class LocalizationText : Text
{
    protected override void Awake()
    {
        base.Awake();
        LocalizationManager.InitValue(this);
        if(CustomFont!=null)
        {
            font = CustomFont.UseFont;
        }
      
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        LocalizationManager.OnLocalize += OnLocalize;  
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        if (LocalizationManager.OnLocalize!=null)
        {
            LocalizationManager.OnLocalize -= OnLocalize;
        }
     
    }

    /// <summary>
    /// 文本的key
    /// </summary>
    public string KeyString;

    /// <summary>
    /// 自定义字体，方便后期替换
    /// </summary>
    public UIFont CustomFont;

    /// <summary>
    /// 重新本地化，用于游戏内切换语言时调用
    /// </summary>
    public void OnLocalize()
    {
        text = Localization.Get(KeyString);
    }

    #region Override Part
    public override string text
    {
        get
        {
            m_Text = Localization.Get(KeyString);
            return m_Text;
        }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                if (string.IsNullOrEmpty(m_Text))
                    return;
                m_Text = "";
                SetVerticesDirty();
            }
            else if (m_Text != value)
            {
                m_Text = value;
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }
    }

    

    #endregion

}
