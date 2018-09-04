//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Localization manager is able to parse localization information from text assets.
/// Using it is simple: text = Localization.Get(key), or just add a UILocalize script to your labels.
/// You can switch the language by using Localization.language = "French", for example.
/// This will attempt to load the file called "French.txt" in the Resources folder,
/// or a column "French" from the Localization.csv file in the Resources folder.
/// If going down the TXT language file route, it's expected that the file is full of key = value pairs, like so:
/// 
/// LABEL1 = Hello
/// LABEL2 = Music
/// Info = Localization Example
/// 
/// In the case of the CSV file, the first column should be the "KEY". Other columns
/// should be your localized text values, such as "French" for the first row:
/// 
/// KEY,English,French
/// LABEL1,Hello,Bonjour
/// LABEL2,Music,Musique
/// Info,"Localization Example","Par exemple la localisation"
/// </summary>

public static class Localization
{
	public enum LanguageType
	{	 
		ChineseSimplified,
		ChineseTraditional,
        English,    
	}

	public static bool IsForcedUpdateByBundle=false;//是否為bundle強制更新

	public static LanguageType GetCurrentLangageType()
	{
		LanguageType lType = LanguageType.English;
		switch (language)
		{
			case "zh-Hans":
				lType = LanguageType.ChineseSimplified;
				break;
			case "zh-HMT":
				lType = LanguageType.ChineseTraditional;
				break;
			case "en":
				lType = LanguageType.English;
				break;
			default:
				lType = LanguageType.English;
				break;

		}
		return lType;
	}


	public class ins
	{
		public string Get(string aa){
						return aa;
				}
	}
	private static ins i;
	public static ins instance {
				get { if(i == null) {i = new ins();}return i;}}
	public delegate byte[] LoadFunction (string path);

	/// <summary>
	/// Want to have Localization loading be custom instead of just Resources.Load? Set this function.
	/// </summary>

	static public LoadFunction loadFunction = TestLoadFunction;

	/// <summary>
	/// Whether the localization dictionary has been loaded.
	/// </summary>
 
	static public bool localizationHasBeenSet = false;

	// Loaded languages, if any
	static string[] mLanguages = null;

    // Key = Value dictionary (single language)
    public static Dictionary<string, string> mOldDictionary = new Dictionary<string, string>();

	// Key = Values dictionary (multiple languages)
	static Dictionary<string, string[]> mDictionary = new Dictionary<string, string[]>();

	// Index of the selected language within the multi-language dictionary
	static int mLanguageIndex = -1;

    // Currently selected language
    static string mLanguage;

	/// <summary>
	/// Localization dictionary. Dictionary key is the localization key. Dictionary value is the list of localized values (columns in the CSV file).
	/// Be very careful editing this via code, and be sure to set the "KEY" to the list of languages.
	/// </summary>

	static public Dictionary<string, string[]> dictionary
	{
		get
		{
			if (!localizationHasBeenSet) language = PlayerPrefs.GetString("Language", "en");
			return mDictionary;
		}
		set
		{
			localizationHasBeenSet = (value != null);
			mDictionary = value;
		}
	}

	/// <summary>
	/// List of loaded languages. Available if a single Localization.csv file was used.
	/// </summary>

	static public string[] knownLanguages
	{
		get
		{
			//if (!localizationHasBeenSet) LoadDictionary(PlayerPrefs.GetString("Language", "en"));
			return mLanguages;
		}
	}

	/// <summary>
	/// Name of the currently active language.
	/// </summary>

	static public string language
	{
		get
		{
			if (string.IsNullOrEmpty(mLanguage))
			{
				string[] lan = knownLanguages;
				LoadLanguageConfig();
				PlayerPrefs.SetString("Language", mLanguage);
				LoadAndSelect(mLanguage);
			}
			return mLanguage;
		}
		set
		{
			//如果language已经有值，不再赋值，但是強制走bundleUpdate更新是可以的
			if ((string.IsNullOrEmpty(mLanguage) && mLanguage != value) || Localization.IsForcedUpdateByBundle)
			{
                Localization.IsForcedUpdateByBundle = false;
                mLanguage = value;
				LoadAndSelect(value);
			}
		}
	}

	/// <summary>
	/// Load the specified localization dictionary.
	/// </summary>

	static bool LoadDictionary (string value)
	{
		// Try to load the Localization CSV
		byte[] bytes = null;

		if (!localizationHasBeenSet)
		{

			if (loadFunction == null)
			{
				TextAsset asset = Resources.Load<TextAsset>("Localization");
				
				if (asset != null) bytes = asset.bytes;
			}
			else bytes = loadFunction("Localization");
			localizationHasBeenSet = true;

            ByteReader reader = new ByteReader(bytes);

            List<string> strList = new List<string>();
            while (reader.canRead)
            {
                string str = reader.ReadLine();
                if (str != null)
                {
                    strList.Add(str);
                }
            }

            mLanguages = new string[strList.Count];
            for (int i = 0; i < mLanguages.Length; ++i)
            {
                mLanguages[i] = strList[i];
            }

            mDictionary.Clear();
		}

		// Try to load the localization file
		//if (LoadCSV(bytes)) return true;

		// If this point was reached, the localization file was not present
		if (string.IsNullOrEmpty(value)) return false;

		// Not a referenced asset -- try to load it dynamically
		if (loadFunction == null)
		{
			TextAsset asset = Resources.Load<TextAsset>(value);
			if (asset != null) bytes = asset.bytes;
		}
		else bytes = loadFunction(value);

		if (bytes != null)
		{
			Set(value, bytes);
			return true;
		}
		return false;
	}
	
	
	//FIXME, use assetbundle load instead
	static public byte[] TestLoadFunction (string path)
	{
		TextAsset asset = Resources.Load<TextAsset>(path);
		
		if (asset != null)
			return asset.bytes;
		else
			return null;
	}

	/// <summary>
	/// Load the specified language.
	/// </summary>

	static bool LoadAndSelect (string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			if (mDictionary.Count == 0 && !LoadDictionary(value)) return false;
			if (SelectLanguage(value)) return true;
		}

		// Old style dictionary
		if (mOldDictionary.Count > 0) return true;

		// Either the language is null, or it wasn't found
		mOldDictionary.Clear();
		mDictionary.Clear();
		if (string.IsNullOrEmpty(value)) PlayerPrefs.DeleteKey("Language");
		return false;
	}

	/// <summary>
	/// Load the specified asset and activate the localization.
	/// </summary>

	static public void Load (TextAsset asset)
	{
		ByteReader reader = new ByteReader(asset);
		Set(asset.name, reader.ReadDictionary());
	}
/*
	static public void Load (string languageName)
	{
		string fileName = languageName + ".txt";
		string url;
		if (System.IO.File.Exists(System.IO.Path.Combine(Application.streamingAssetsPath, fileName)))
		{
			url = "file://" + Application.streamingAssetsPath + '/' + fileName;
		}
		else
		{
			url = Application.streamingAssetsPath + '/' + fileName;
		}
		
		
		using (WWW www = new WWW(url))
		{
			while (!www.isDone && string.IsNullOrEmpty(www.error))
			{
				yield return null;
			}
			
			if (string.IsNullOrEmpty(www.error))
			{
				AppLogger.Debug("read data [OK] " + url);
				ByteReader reader = new ByteReader(www.bytes);
				Set(languageName, reader.ReadDictionary());
			}
			else
			{
				AppLogger.Error("read data [Fail] " + url);
				AppLogger.Error(www.error);
				yield break;
			}
		}	
		
		
	}
*/	

	/// <summary>
	/// Set the localization data directly.
	/// </summary>

	static public void Set (string languageName, byte[] bytes)
	{
		ByteReader reader = new ByteReader(bytes);
		Set(languageName, reader.ReadDictionary());
	}

	/// <summary>
	/// Select the specified language from the previously loaded CSV file.
	/// </summary>

	static bool SelectLanguage (string language)
	{
		mLanguageIndex = -1;

		if (mDictionary.Count == 0) return false;

		string[] keys;

		if (mDictionary.TryGetValue("KEY", out keys))
		{
			for (int i = 0; i < keys.Length; ++i)
			{
				if (keys[i] == language)
				{
					mOldDictionary.Clear();
					mLanguageIndex = i;
					mLanguage = language;
					PlayerPrefs.SetString("Language", mLanguage);
                    Broadcast();
                    return true;
				}
			}
		}
		return false;
	}

    static void Broadcast()
    {
        //UIRoot.Broadcast("OnLocalize");
    }



	/// <summary>
	/// Load the specified asset and activate the localization.
	/// </summary>

	static public void Set (string languageName, Dictionary<string, string> dictionary)
	{
		mLanguage = languageName;
		PlayerPrefs.SetString("Language", mLanguage);
		mOldDictionary = dictionary;
		localizationHasBeenSet = false;
		mLanguageIndex = -1;

	    if (mLanguages == null)
	    {
	        mLanguages = new string[] {languageName};
	    }

		List<string> tempLanguages = new List<string>();
		for (int j = 0; j < mLanguages.Length; j++)
		{
			tempLanguages.Add(mLanguages[j]);
		}

		if (!tempLanguages.Contains(languageName))
	    {
	        string[] temp = new string[mLanguages.Length + 1];
	        for (int i = 0; i < mLanguages.Length; i++)
	        {
	            temp[i] = mLanguages[i];
	        }
	        temp[mLanguages.Length] = languageName;
	        mLanguages = temp;
	    }
        Broadcast();

    }

	/// <summary>
	/// Localize the specified value.
	/// </summary>

	static public string Get (string key)
	{
		//如果key不是以IDS_开头，直接return
		if (string.IsNullOrEmpty(key) || !key.StartsWith("IDS_"))
		{
			return key;
		}
	
		// Ensure we have a language to work with
		if (!localizationHasBeenSet) language = PlayerPrefs.GetString("Language", "en");

		string val;
		string[] vals;
#if UNITY_IPHONE || UNITY_ANDROID
		string mobKey = key + " Mobile";

		if (mLanguageIndex != -1 && mDictionary.TryGetValue(mobKey, out vals))
		{
			if (mLanguageIndex < vals.Length)
				return vals[mLanguageIndex];
		}
		else if (mOldDictionary.TryGetValue(mobKey, out val)) return val;
#endif
		if (mLanguageIndex != -1 && mDictionary.TryGetValue(key, out vals))
		{
			if (mLanguageIndex < vals.Length)
				return vals[mLanguageIndex];
		}
		else if (mOldDictionary.TryGetValue(key, out val)) return val;

#if UNITY_EDITOR
//		Debug.LogWarning("Localization key not found: '" + key + "'");
#endif
		return key;
	}

	/// <summary>
	/// Localize the specified value and format it.
	/// </summary>

	static public string Format (string key, params object[] parameters) { return string.Format(Get(key), parameters); }

	[System.Obsolete("Localization is now always active. You no longer need to check this property.")]
	static public bool isActive { get { return true; } }

	[System.Obsolete("Use Localization.Get instead")]
	static public string Localize (string key) { return Get(key); }

	/// <summary>
	/// Returns whether the specified key is present in the localization dictionary.
	/// </summary>

	static public bool Exists (string key)
	{
		// Ensure we have a language to work with
		if (!localizationHasBeenSet) language = PlayerPrefs.GetString("Language", "en");

#if UNITY_IPHONE || UNITY_ANDROID
		string mobKey = key + " Mobile";
		if (mDictionary.ContainsKey(mobKey)) return true;
		else if (mOldDictionary.ContainsKey(mobKey)) return true;
#endif
		return mDictionary.ContainsKey(key) || mOldDictionary.ContainsKey(key);
	}

	

	public static void LoadLanguageConfig()
	{
		byte[] bytes = null;

		if (loadFunction == null)
		{
			TextAsset asset = Resources.Load<TextAsset>("Localization");

			if (asset != null)
			{
				bytes = asset.bytes;
			}
		}
		else
		{
			bytes = loadFunction("Localization");
		}

//		ByteReader reader = new ByteReader(bytes);
//		
//		while (reader.canRead)
//		{
//			string str = reader.ReadLine();
//			if (!string.IsNullOrEmpty(str))
//			{
//                language = str;
//			}
//		}

		//为空时，默认使用简体中文
//		if (string.IsNullOrEmpty(language))
//		{
//			language = "zh-Hans";//默认为简体中文
//		}
			language = "zh-Hans";
	}

	public static void SwitchLanguage(string lan)
	{
        language = lan;
        mLanguage = lan;
        LoadAndSelect(lan);
        PlayerPrefs.SetString("Language", mLanguage);
        if(LocalizationManager.OnLocalize!=null)
        {
            LocalizationManager.OnLocalize();
        }
   
        /*
        DataSync.Instance.UpdateIDSText();
        UIRoot.Broadcast("OnLocalize");
        
        if (ConnectManager.Instance != null)
        {
            ConnectManager.Instance.RegisterHandler(
          () =>
          {
              TionManager.Instance.SettingSystemLanguageRequest(
            Localization.GetDefaultlanguage());
          },
          () =>
          {
              GameManager.RebootToTownLite();
              Time.timeScale = 1.0f;
          }
         );
        }*/

    }

    public static void CheatSwitchLanguage(string lan,Action rebootFunction)
    {
        language = lan;
        mLanguage = lan;
        LoadAndSelect(lan);
        PlayerPrefs.SetString("Language", mLanguage);
        if(rebootFunction!=null)
        {
            rebootFunction();
        }
        Time.timeScale = 1.0f;
    }

	

    public static string GetDefaultlanguage()
    {
		if (string.IsNullOrEmpty(mLanguage))
		{
			LoadLanguageConfig();
		}
		return mLanguage;
    }

	/// <summary>
	/// 是否为简体中文
	/// </summary>
	/// <returns></returns>
	public static bool IsChineseSimple()
	{
		return mLanguage == "zh-Hans";
	}


}
