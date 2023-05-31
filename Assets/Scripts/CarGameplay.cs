using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarGameplay : MonoBehaviour
{

    [SerializeField] private ScoreSystem scoreSystem = null;
    [SerializeField] private GameOverHandler gameOverHandler = null;
    
    [SerializeField] private float topSpeedIncreaseValue;
    [SerializeField] private float topSpeedIncreaseInterval;

    [SerializeField] private int numberOfLives = 2;
    
    private Transform _lastCheckpoint;
    
    private Rigidbody _carRigidbody;
    private TestCarPhysics _carPhysicsParams;

    private float _topSpeedIncreaseTimer;
    private int _nLives;

    private bool isPlaying = true;
    private void Start()
    {
        //Ignoring collision with ground
        //Physics.IgnoreLayerCollision(layer1: LayerMask.NameToLayer("Default"), layer2:LayerMask.NameToLayer("Ignore Collision"), true);
        _nLives = numberOfLives;
        
        _carRigidbody = GetComponent<Rigidbody>();
        _carPhysicsParams = GetComponent<TestCarPhysics>();
    }

    private void Update()
    {
        if (!isPlaying) return;
        
        if (_topSpeedIncreaseTimer <= 0)
        {
            _carPhysicsParams.IncreaseMaxSpeed(topSpeedIncreaseValue);
            _topSpeedIncreaseTimer = topSpeedIncreaseInterval;
            Debug.Log("TopSpeed increased");
            Debug.Log(_carRigidbody.velocity.magnitude);
        }
        
        UpdateTimers();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Checkpoint"))
        {
            _lastCheckpoint = other.gameObject.transform;
            //Debug.Log("Passed checkpoint");
        }
    }

    //For gameOver
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            //Calculating if it was frontal hit or side hit
            Vector3 dirToHitPoint = other.GetContact(0).point - transform.position;
            Debug.Log("dirToHitPoint: " + dirToHitPoint);
            float dotForCarHit = Vector3.Dot(transform.forward, dirToHitPoint);
            Debug.Log("dotForCar: " + dotForCarHit);
    
            if (dotForCarHit > 1.5f)
            {
                Debug.Log("Should display game over display");
                if (_nLives >= 1)
                {
                    isPlaying = false;
                    _nLives -= 1;
                    RespawnAtLastCheckpoint();
                }
                else
                {
                    isPlaying = false;
                    scoreSystem.StopScoreSystem();
                    _carPhysicsParams.CarControlEnabled(false);
                    gameOverHandler.EndGame();
                }
            }
            else
            {
                Debug.Log("Minor hit, reduce strength points");
            }
        }
    }

    private void RespawnAtLastCheckpoint()
    {
        scoreSystem.StopScoreSystem();
        StartCoroutine(RespawnCar(1f));
        StartCoroutine(SpeedUpAfterRespawn(5f));
    }


    public void ContinueGame()
    {
        _nLives = numberOfLives;
        RespawnAtLastCheckpoint();
    }

    IEnumerator RespawnCar (float seconds)
    {
        _carPhysicsParams.CarReset();
        _carPhysicsParams.CarControlEnabled(false);
        transform.position = _lastCheckpoint.position + new Vector3(0f, 0.5f, 0f);
        transform.rotation = _lastCheckpoint.rotation;
        yield return new WaitForSeconds(seconds);
        isPlaying = true;
        _carPhysicsParams.CarControlEnabled(true);
        scoreSystem.StartScoreSystem();
    }

    IEnumerator SpeedUpAfterRespawn(float seconds)
    {
        for (int i = 0; i <= seconds; i++)
        {
            yield return new WaitForSeconds(1f);
            _carPhysicsParams.AccelerationInp((1 / seconds) * i);
        }
        
    }

    private void UpdateTimers()
    {
        _topSpeedIncreaseTimer -= Time.deltaTime;
    }
}