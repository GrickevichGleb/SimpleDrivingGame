using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    [SerializeField] private float scoreGainPerSecond = 1f;
    [SerializeField] private TMP_Text scoreText;
    
    //DebugValues
    [SerializeField] private GameObject carObj;

    [SerializeField] private TMP_Text speedText;
    [SerializeField] private TMP_Text topSpeedText;
    [SerializeField] private TMP_Text torqueText;
    [SerializeField] private float debugValuesUpdateRate = 0.5f;
    

    private TestCarPhysics _carPhysics = null;


    public const string HighScoreKey = "HighScore";
    private float _score;
    private bool _isCounting = true;
    
    private float _debugValuesUpdateTimer;

    private void Start()
    {
        _carPhysics = carObj.GetComponent<TestCarPhysics>();
    }

    void Update()
    {
        if (!_isCounting) return;
        
        _score += scoreGainPerSecond * Time.deltaTime; //one point every second 
        scoreText.text = Mathf.FloorToInt(_score).ToString();
        
        if (carObj == null) return;
        if (_debugValuesUpdateTimer <= 0)
        {
            speedText.text = $"Speed: {_carPhysics.GetVelocity():F1}";
            topSpeedText.text = $"TopSpeed: {_carPhysics.GetTopSpeed():F1}";
            torqueText.text = $"Torque: {_carPhysics.GetTorque():F1}";
            _debugValuesUpdateTimer = debugValuesUpdateRate;
        }

        _debugValuesUpdateTimer -= Time.deltaTime;
    }


    public void StopScoreSystem()
    {
        _isCounting = false;
        scoreText.enabled = false;
    }

    public void StartScoreSystem()
    {
        _isCounting = true;
        scoreText.enabled = true;
    }

    public int GetScore()
    {
        return Mathf.FloorToInt(_score);
    }
    
    private void OnDestroy()
    {
        int currentHighScore = PlayerPrefs.GetInt(HighScoreKey, 0);

        if (_score > currentHighScore)
        {
            PlayerPrefs.SetInt(HighScoreKey, Mathf.FloorToInt(_score));
        }
    }
}
