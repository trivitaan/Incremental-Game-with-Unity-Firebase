using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;

public class AnalyticsManager
{
    private static void LogEvent(string eventName, params Parameter[] parameters)
    {
        //method utama untuk menembakkan firebase
        FirebaseAnalytics.LogEvent(eventName, parameters);
    }

    public static void LogUpgradeEvent(int resourceIndex, int level)
    {
        //kita memiliki event dan parameter yang tersedia di firebase (tidak memakai yang custom)
        //agar dapat muncul sebagai report data di analytics firebase
        LogEvent(
            FirebaseAnalytics.EventLevelUp,
            new Parameter(FirebaseAnalytics.ParameterIndex, resourceIndex.ToString()),
            new Parameter(FirebaseAnalytics.ParameterLevel, level)
        );
        //karena resourceIndex digunakan sebagai ID, maka seharusnya kita simpan sebagai string bukan integer
    }

    public static void LogUnlockEvent(int resourceIndex)
    {
        LogEvent(
            FirebaseAnalytics.EventUnlockAchievement, 
            new Parameter(FirebaseAnalytics.ParameterIndex, resourceIndex.ToString())
        );
    }

    public static void SetUserProperties(string name, string value)
    {
        FirebaseAnalytics.SetUserProperty(name, value);
    }
}
