using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class TestCarPhysics : MonoBehaviour
{
    [SerializeField] private GameObject FRSuspension;
    [SerializeField] private GameObject FLSuspension;
    [SerializeField] private GameObject RLSuspension;
    [SerializeField] private GameObject RRSuspension;
    
    [SerializeField] private GameObject FRWheel;
    [SerializeField] private GameObject FLWheel;
    [SerializeField] private GameObject RLWheel;
    [SerializeField] private GameObject RRWheel;
    
    [SerializeField] private float suspensionRestDist = 0.5f;
    [SerializeField] private float springStrength = 1f;
    [SerializeField] private float springDamp = 1f;
    [SerializeField] private float springTravel = 0.3f;

    [SerializeField] private float tireGrip;
    [SerializeField] private float tireMass = 1f;

    [SerializeField] private float accelInput;
    [SerializeField] private float topSpeed;
    [SerializeField] private float topSpeedCap = 50f;
    [SerializeField] private float torque;
    [SerializeField] private float torqueCap = 2000f;
    
    [SerializeField] private AnimationCurve steerIntensity;
    [SerializeField, Range(0,1)]private float steerInterpolation = 0.1f;

    private CarEffects _carEffects;
    
    private Rigidbody _carBodyRb;
    private int _steerValue = 0;
    private float _wheelSteerAngle = 45f;
    private float curvePoint = 0f;
    
    private float _wheelRadius;
    private bool _carControlEnabled = true;
    
    
    //debug
    private float yMax = 0f;

    // Start is called before the first frame update
    void Start()
    {
        _carBodyRb = GetComponent<Rigidbody>();
        _carEffects = GetComponent<CarEffects>();
        
        RaycastHit hit;
        
        Ray downRay = new Ray(RRWheel.transform.position, Vector3.down);
        Physics.Raycast(downRay, out hit);

        _wheelRadius = RRWheel.GetComponent<MeshRenderer>().localBounds.max[2];
        suspensionRestDist = _wheelRadius + 0.01f;
    }
    
    private void FixedUpdate()
    {
        SuspensionWork(FRSuspension, FRWheel);
        SuspensionWork(FLSuspension, FLWheel);
        SuspensionWork(RRSuspension, RRWheel);
        SuspensionWork(RLSuspension, RLWheel);
        
        if (!_carControlEnabled) return;
        
        SteeringWork(FRSuspension, FRWheel);
        SteeringWork(FLSuspension, FLWheel);
        SteeringWork(RRSuspension, RRWheel);
        SteeringWork(RLSuspension, RLWheel);

        Acceleration(RRSuspension);
        Acceleration(RLSuspension);
        
        // Acceleration(FRSuspension);
        // Acceleration(FLSuspension);

        SteerWheel(FLSuspension);
        SteerWheel(FRSuspension);
        
        SpinWheels();//Adds spinning motion to a wheels 
    }
    
    //Used for touch inputs
    public void Steering(int steer)
    {
        _steerValue = steer;
    }

    public void AccelerationInp(float accel)
    {
        accelInput = accel;
    }

    public void IncreaseMaxSpeed(float maxSpeedIncreaseValue)
    {
        if ((topSpeed + maxSpeedIncreaseValue) <= topSpeedCap)
        {
            topSpeed += maxSpeedIncreaseValue;

            if (torque + (maxSpeedIncreaseValue * 20f) <= torqueCap)
            {
                torque += maxSpeedIncreaseValue * 200f;
            }
        }
 
    }

    public void SetAcceleration(float torqueValue)
    {
        torque = torqueValue;
    }

    public void CarReset()
    {
        _carEffects.EmitTireMarks(false);
        
        _steerValue = 0;
        StraightenWheels(FLSuspension);
        StraightenWheels(FRSuspension);

        
        _carBodyRb.velocity = Vector3.zero;
        _carBodyRb.angularVelocity = Vector3.zero;
    }

    public void CarControlEnabled(bool flag)
    {
        _carControlEnabled = flag;
    }
    
    //Debug purposes
    public float GetTopSpeed()
    {
        return topSpeed;
    }

    public float GetVelocity()
    {
        return _carBodyRb.velocity.magnitude;
    }

    public float GetTorque()
    {
        return torque;
    }

    private void SuspensionWork(GameObject suspensionPoint, GameObject tire)
    {
        // If to far from the ground just place wheels in zero position
        if (!Physics.Raycast(suspensionPoint.transform.position, Vector3.down, suspensionRestDist))
        {
            tire.transform.position = suspensionPoint.transform.position;
            return;
        }
        
        
        RaycastHit hit;
        Ray downRay = new Ray(suspensionPoint.transform.position, Vector3.down);
        if (Physics.Raycast(downRay, out hit))
        {
            // Calculating and applying forces to car rigidbody
            Vector3 springDir = suspensionPoint.transform.up;
            Vector3 wheelWorldVelocity = _carBodyRb.GetPointVelocity(suspensionPoint.transform.position);
            float offset = suspensionRestDist - hit.distance;
            
            //Resist only against ground directed forces
            if (offset > 0)
            {
                float velocity = Vector3.Dot(springDir, wheelWorldVelocity);
                float force = (offset * springStrength) - (velocity * springDamp);
                _carBodyRb.AddForceAtPosition(springDir * force, suspensionPoint.transform.position);
            }
          
            
            // Adjusting wheel position 
            Vector3 wheelAdjustedPosition = Vector3.zero;
            Vector3 wheelAdjustDirection = suspensionPoint.transform.position - hit.point;
            wheelAdjustDirection.Normalize();

            if (Mathf.Abs(offset) < springTravel)
            {
                wheelAdjustedPosition = suspensionPoint.transform.position + (wheelAdjustDirection * offset);
            }
            else
            {
                wheelAdjustedPosition = suspensionPoint.transform.position + (wheelAdjustDirection * springTravel);
            }

            tire.transform.position = wheelAdjustedPosition;
            
        }
    }

    private void SteeringWork(GameObject suspensionPoint, GameObject tire)
    {
        if (Physics.Raycast(suspensionPoint.transform.position, Vector3.down, suspensionRestDist+springTravel))
        {
            Vector3 steeringDir = suspensionPoint.transform.right;
            
            Vector3 wheelWorldVelocity = _carBodyRb.GetPointVelocity(suspensionPoint.transform.position);
        
            float steeringVelocity = Vector3.Dot(steeringDir, wheelWorldVelocity);
            float desiredVelChange = -steeringVelocity * tireGrip;
            float desiredAccel = desiredVelChange / Time.fixedDeltaTime;

            _carBodyRb.AddForceAtPosition(steeringDir * (tireMass * desiredAccel), suspensionPoint.transform.position);

            //TireMarks
            float steerResistance = Vector3.Dot(steeringDir.normalized, wheelWorldVelocity.normalized);
            if (Mathf.Abs(steerResistance) > 0.25f)
            {
                RaycastHit hit;
                Ray downRay = new Ray(suspensionPoint.transform.position, Vector3.down);
                if (Physics.Raycast(downRay, out hit, suspensionRestDist + springTravel))
                {
                    _carEffects.EmitTireMarks(true);
                }
            }
            else
            {
                _carEffects.EmitTireMarks(false);
            }
        }
        else
        {
            _carEffects.EmitTireMarks(false);
        }
        
        // Always applying gravity to a tire
        Vector3 gravityDir = -suspensionPoint.transform.up;
        //Debug.Log("gravityDir: " + gravityDir);
        _carBodyRb.AddForceAtPosition((tireMass*gravityDir)/Time.fixedDeltaTime, suspensionPoint.transform.position);

    }

    private void StraightenWheels(GameObject steeringPoint)
    {
        //Vector3 wheelTurnAngle = new Vector3(0f, _wheelSteerAngle, 0f) * _steerValue;
        
        Quaternion wheelRot = _carBodyRb.transform.rotation * Quaternion.Euler(Vector3.zero);
        steeringPoint.transform.rotation = Quaternion.Lerp(steeringPoint.transform.rotation, wheelRot, 1f);
    }

    private void Acceleration(GameObject forcePoint)
    {
        Vector3 accelDir = forcePoint.transform.forward;
        
        if (accelInput > 0.0f)
        {
            if (Physics.Raycast(forcePoint.transform.position, Vector3.down, suspensionRestDist))
            {
                float carSpeed = Vector3.Dot(transform.forward, _carBodyRb.velocity);
                float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / topSpeed);

                //To use with curves
                float availableTorque = torque * accelInput;

                if (carSpeed < topSpeed)
                {
                    _carBodyRb.AddForceAtPosition(accelDir*availableTorque, forcePoint.transform.position, ForceMode.Force);
                }
            }
        }
    }

    private void SteerWheel( GameObject steeringPoint)
    {
        if (_steerValue != 0)
        {
            curvePoint += Time.fixedDeltaTime;
            if (curvePoint >= 1f) curvePoint = 1f;
        }
        else
        {
            curvePoint = 0f;
        }
        
        float t = steerIntensity.Evaluate(curvePoint);
        //Debug.Log("CurvePoint: "+ curvePoint + " t: "+ t);
        
        
        Vector3 wheelTurnAngle = new Vector3(0f, _wheelSteerAngle, 0f) * _steerValue;
        Quaternion wheelRot = _carBodyRb.transform.rotation * Quaternion.Euler(wheelTurnAngle);
        
        if(_steerValue == 0)
        {
            steeringPoint.transform.rotation = Quaternion.Lerp(steeringPoint.transform.rotation, wheelRot, steerInterpolation);
        }
        
        steeringPoint.transform.rotation = Quaternion.Lerp(steeringPoint.transform.rotation, wheelRot, t);
    }

    private void SpinWheels()
    {
        float carSpeed = Vector3.Dot(transform.forward, _carBodyRb.velocity);

        float wheelOuterLength = 2 * Mathf.PI * _wheelRadius;
        float xRotation = 360 * carSpeed / wheelOuterLength;
        RRWheel.transform.Rotate(xRotation*Time.fixedDeltaTime, 0f, 0f);
        RLWheel.transform.Rotate(xRotation*Time.fixedDeltaTime, 0f, 0f);
        
        FRWheel.transform.Rotate(xRotation*Time.fixedDeltaTime, 0f, 0f);
        FLWheel.transform.Rotate(xRotation*Time.fixedDeltaTime, 0f, 0f);

    }
}