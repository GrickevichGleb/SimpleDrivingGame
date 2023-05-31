using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text energyText;
    [SerializeField] private int maxEnergy;
    [SerializeField] private int energyRechargeSec;
    [SerializeField] private Button playButton;
    [SerializeField] private AndroidNotificationHandler androidNotificationHandler;

    private int _energy;
    
    private const string EnergyKey = "Energy";
    private const string EnergyReadyKey = "EnergyReady";

    private void Start()
    {
        OnApplicationFocus(true);
    }

    private void OnApplicationFocus(bool hasFocused)
    {
        if (!hasFocused) { return;}
        
        CancelInvoke();
        androidNotificationHandler.CancelDisplayedNotifications();//Clearing up notifications
        
        //Loading Highscore
        int highScore = PlayerPrefs.GetInt(ScoreSystem.HighScoreKey, 0);
        highScoreText.text = $"HighScore: {highScore}";
        
        //Loading Energy
        _energy = PlayerPrefs.GetInt(EnergyKey, maxEnergy);//By default energy is max

        if (_energy == 0)
        {
            string energyReadyString = PlayerPrefs.GetString(EnergyReadyKey, string.Empty);

            if (energyReadyString == String.Empty) return;

            DateTime energyReadyDT = DateTime.Parse(energyReadyString);
            
            if (DateTime.Now > energyReadyDT)
            {
                _energy = maxEnergy;
                PlayerPrefs.SetInt(EnergyKey, maxEnergy);
            }
            else
            {
                playButton.interactable = false;
                Invoke(nameof(EnergyRecharged) ,(energyReadyDT - DateTime.Now).Seconds);
            }
        }

        energyText.text = $"PLAY ({_energy})";
    }

    public void Play()
    {
        if (_energy > 0)
        {
            _energy -= 1;
            PlayerPrefs.SetInt(EnergyKey, _energy);
            if (_energy == 0)//Consumed all energy
            {
                DateTime energyRestoreTime = DateTime.Now.AddSeconds(energyRechargeSec);
                PlayerPrefs.SetString(EnergyReadyKey, energyRestoreTime.ToString());
                
                //Activating notification but only in build for android
#if UNITY_ANDROID
                androidNotificationHandler.ScheduleNotification(energyRestoreTime);
#endif
            }
            SceneManager.LoadScene(1);
        }
        else
        {
            Debug.Log("Out of energy");
            return;
        }
        
    }

    private void EnergyRecharged()
    {
        playButton.interactable = true;
        _energy = maxEnergy;
        PlayerPrefs.SetInt(EnergyKey, maxEnergy);
        energyText.text = $"PLAY ({_energy})";
    }
}
