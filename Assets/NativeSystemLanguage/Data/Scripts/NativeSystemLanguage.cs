using System;
using System.Globalization;
using System.Runtime.InteropServices;
using UnityEngine;

public class NativeSystemLanguage
{
    public static string GetSystemLanguag()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                return WindowsLanguage();
            case RuntimePlatform.IPhonePlayer:
                return iOSLanguage();
            case RuntimePlatform.Android:
                return AndroidLanguage();
            default:
                return Application.systemLanguage.ToString();
        }
    }

    #region iOS
    [DllImport("__Internal")]
    private static extern string CurIOSLang();

    private static string iOSLanguage()
    {
        return GetNativeSystemLanguage(CurIOSLang);
    }
    #endregion
    #region Android
    private static string AndroidLanguage()
    {
        return GetNativeSystemLanguage(CurrentAndroidLanguage);
    }

    private static string CurrentAndroidLanguage()
    {
        string result = "";
        using (AndroidJavaClass cls = new AndroidJavaClass("java.util.Locale"))
        {
            if (cls != null)
            {
                using (AndroidJavaObject locale = cls.CallStatic<AndroidJavaObject>("getDefault"))
                {
                    if (locale != null)
                    {
                        result = locale.Call<string>("getLanguage") + "_" + locale.Call<string>("getDefault");
                        Debug.Log("Android lang: " + result);
                    }
                    else
                    {
                        Debug.Log("locale null");
                    }
                }
            }
            else
            {
                Debug.Log("cls null");
            }
        }
        return result;
    }
    #endregion
    #region Windows
    private static string WindowsLanguage()
    {
        return GetNativeSystemLanguage(GetWindowsCultureInfoName);
    }

    [DllImport("kernel32.dll")]
    private static extern int GetSystemDefaultLCID();

    private static string GetWindowsCultureInfoName()
    {
        return CultureInfo.GetCultureInfo(GetSystemDefaultLCID()).Name;
    }
    #endregion

    private static string GetNativeSystemLanguage(Func<string> GetNativeLanguageMethod)
    {
        try
        {
            return GetTransformLanguage(GetNativeLanguageMethod());
        }
        catch
        {
            return "zh-TW";
        }
    }

    private static string GetTransformLanguage(string systemLanguage)
    {
        string LangLower = systemLanguage.ToLower();

        switch (LangLower)
        {
            case "zh-cn":
            case "zh-sg":
            case "zh-chs":
            case "zh-hans":
            case "zh-hans-hk":
            case "zh-hans-mo":
            case "zh-hans-tw":
            case "chinese":
            case "chinesesimplified":
                return "CN";

            case "zh-hk":
            case "zh-mo":
            case "zh-cht":
            case "zh-tw":
            case "zh-hant":
            case "zh-hant-cn":
            case "zh-hant-hk":
            case "zh-hant-mo":
            case "zh-hant-sg":
            case "zh-hant-tw":
            case "chinesetraditional":
                return "TW";

            case "ja":
            case "ja-JP":
            case "Japanese":
                return "JP";
            default:
                return GetLanguageFamily(LangLower);
        }

    }

    private static string GetLanguageFamily(string systemLanguage)
    {
        if (systemLanguage.Contains("zh"))
            return "CN";
        else if (systemLanguage.Contains("jp"))
            return "JP";
        else
            return "EN";
    }
}
