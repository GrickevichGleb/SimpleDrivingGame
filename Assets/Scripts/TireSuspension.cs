using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TireSuspension : MonoBehaviour
{
    [SerializeField] private float suspensionRestDist = 0.5f;
    [SerializeField] private float springStrength = 1f;
    [SerializeField] private float springDamp = 1f;


    private Rigidbody _carBodyRb;
    // Start is called before the first frame update
    void Start()
    {
        _carBodyRb = GetComponentInParent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Tire(this.gameObject);
    }

    private void Tire(GameObject wheel)
    {
        RaycastHit hit;
        Ray downRay = new Ray(wheel.transform.position, Vector3.down);
        if (Physics.Raycast(downRay, out hit))
        {
            Vector3 springDir = wheel.transform.up;

            Vector3 wheelWorldVelocity = _carBodyRb.GetPointVelocity(wheel.transform.position);

            float offset = suspensionRestDist - hit.distance;
    
            float velocity = Vector3.Dot(springDir, wheelWorldVelocity);

            float force = (offset * springStrength) - (velocity * springDamp);


           
            //Applying force
            _carBodyRb.AddForceAtPosition(springDir * force, wheel.transform.position);
            
        }
    }

}
