using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Car : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float speedGainSec = 0.2f;
    [SerializeField] private float turnSpeed = 100f;
    [SerializeField] private float maxSpeed = 50f;
    
    private int _steerValue;
    void Update()
    {
        //speed += speedGainSec * Time.deltaTime;
        speed = Mathf.Min(speed + (speedGainSec * Time.deltaTime), maxSpeed);

        transform.Rotate(0f, _steerValue * turnSpeed * Time.deltaTime, 0f);
        
        transform.Translate(Vector3.forward * speed * Time.deltaTime );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            SceneManager.LoadScene(0);
        }
    }

    public void Steer(int value)
    {
        _steerValue = value;
    }
}
