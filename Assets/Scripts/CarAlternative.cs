using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAlternative : MonoBehaviour
{
    [SerializeField] private float acceleration = 11f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float torqueTurn = 3f;
    
    private Rigidbody _carRb = null;
    private int _steerValue;
    // Start is called before the first frame update
    void Start()
    {
        _carRb = GetComponent<Rigidbody>();
        _carRb.isKinematic = false;
        _carRb.useGravity = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        ForwardMovement();
        Steering();
    }
    
    public void Steer(int value)
    {
        _steerValue = value;
    }

    private void ForwardMovement()
    {
        if (_carRb.velocity.magnitude < maxSpeed)
        {
            Vector3 forceVector3 = _carRb.rotation * Vector3.forward * acceleration;
            _carRb.AddForce(forceVector3, ForceMode.Force);
        }

        Debug.Log("Current velocity: " + _carRb.velocity.magnitude);
    }

    private void Steering()
    {
        Vector3 steerVector3 = Vector3.up * torqueTurn * _steerValue;
        _carRb.AddTorque( steerVector3, ForceMode.Acceleration);
    }
}
