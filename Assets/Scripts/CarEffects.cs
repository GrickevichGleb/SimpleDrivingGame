using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEffects : MonoBehaviour
{
    [SerializeField] private TrailRenderer[] tireMarksRenderers;


    public void EmitTireMarks(bool isEmitting)
    {
        foreach (TrailRenderer trail in tireMarksRenderers)
        {
            trail.transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
            trail.emitting = isEmitting;
        }
    }
}