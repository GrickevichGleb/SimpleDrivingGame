using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;

public class AddManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public string debugText;


    [SerializeField] private bool testMode = true;

    private bool adIsLoaded = false;
    public bool AdIsLoaded => adIsLoaded;
    
    public static AddManager Instance;

#if UNITY_ANDROID
    private string gameId = "5255689";
#elif UNITY_IOS
    private string gameId = "5255688";
#endif
    
    private GameOverHandler _gameOverHandler;
    private void Awake()
    {
        //We need one and only one instance of AddManager 
        //available from everywhere 
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            //Advertisement.Initialize(gameId,testMode, this);
        }
        
        //Advertisement.Initialize(gameId,testMode, this);

        StartCoroutine(InitCoroutineAdd());
        
        debugText = "Add debug: ";
    }

    public void ShowAdd(GameOverHandler gameOverHandler)
    {
        debugText += " is supported: " + Advertisement.isSupported + "| ";
        debugText += " is initialized: " + Advertisement.isInitialized + " | ";

        this._gameOverHandler = gameOverHandler;

        if (!adIsLoaded)
        { 
            debugText += "No loaded Add |";
           Debug.Log("No loaded Add");
           return;
        }

        adIsLoaded = false;
        Advertisement.Show("Rewarded_Android", this);

    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        debugText += " UnityAddsShowFailure message| ";

        Debug.LogError("UnityAddsShowFailure message");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        debugText += " Unity add show started| ";
        
        Debug.Log("Unity add show started");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log("Unity add clicked");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Advertisement.Load("Rewarded_Android", this);

        switch (showCompletionState)
        {
            case UnityAdsShowCompletionState.COMPLETED:
                
                debugText += " Add show completed | ";

                Debug.Log("Add showed");
                _gameOverHandler.ContinueGame();
                break;
            
            case UnityAdsShowCompletionState.SKIPPED:
                
                debugText += " Add show skipped | ";

                Debug.Log("Add skipped");
                break;
            
            case UnityAdsShowCompletionState.UNKNOWN:
                
                debugText += " Add show unknown | ";

                Debug.LogWarning("Add failed unknown");
                break;
        }
    }

    public void OnInitializationComplete()
    {
        Advertisement.Load("Rewarded_Android", this);
        
        debugText += " Add Initialization complete | ";

        Debug.Log("Adds initialization complete");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        debugText += " Add Initialization failed | ";

        Debug.LogError($"Adds initialization failed {error} - {message}");
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        debugText += " Add loaded | ";

        Debug.Log($"Add loaded: {placementId}");
        adIsLoaded = true;
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        debugText += " Add failed to loaded | ";

        Debug.LogError($"Add failed to load {error} - {message}");
    }

    IEnumerator InitCoroutineAdd()
    {
        Advertisement.Initialize(gameId,testMode, this);
        yield return new WaitForSeconds(3f);

        if (!Advertisement.isInitialized)
        {
            StartCoroutine(InitCoroutineAdd());
        }
    }
    
}

