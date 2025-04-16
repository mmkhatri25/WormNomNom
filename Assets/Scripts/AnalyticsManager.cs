using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void SendEvent_LoadingGame()
    {
        Analytics.CustomEvent("Loading game");

        Debug.Log("Analytics: Player Die event sent.");
    }

    
    // Player die event
    public void SendEvent_PlayerDie(int levelNumber = 0, int score = 0)
    {
        Analytics.CustomEvent("player_die", new Dictionary<string, object>
        {
            { "level_number", levelNumber },
            { "score", score }
        });
        Debug.Log("Analytics: Player Die event sent.");
    }

    // Player revive event
    public void SendEvent_PlayerRevive(int levelNumber = 0, string reviveMethod = "")
    {
        Analytics.CustomEvent("player_revive", new Dictionary<string, object>
        {
            { "level_number", levelNumber },
            { "revive_method", reviveMethod }
        });
        Debug.Log("Analytics: Player Revive event sent.");
    }

    // Home screen view event
    public void SendEvent_HomeScreenView()
    {
        Analytics.CustomEvent("home_screen_view");
        Debug.Log("Analytics: Home screen viewed.");
    }
    public void SendEvent_PlayAgain()
    {
        Analytics.CustomEvent("User PlayAgain");
        Debug.Log("Analytics: Home screen viewed.");
    }
    // Shop screen view event
    public void SendEvent_ShopScreenView()
    {
        Analytics.CustomEvent("coin shop_screen_view");
        Debug.Log("Analytics: Shop screen viewed.");
    }

    // Rate app clicked event
    public void SendEvent_RateAppClicked()
    {
        Analytics.CustomEvent("rate_app_clicked");
        Debug.Log("Analytics: Rate app clicked.");
    }

    // App crash event
    public void SendEvent_AppCrash(string errorMessage)
    {
        Analytics.CustomEvent("app_crash", new Dictionary<string, object>
        {
            { "error_message", errorMessage }
        });
        Debug.Log("Analytics: App crash event sent.");
    }

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Exception)
        {
            SendEvent_AppCrash(logString + "\n" + stackTrace);
        }
    }
}
