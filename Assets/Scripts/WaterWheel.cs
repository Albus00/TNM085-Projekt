using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterWheel : MonoBehaviour
{
    public GameObject waterBody;
    private float swellIncrement;
    private WaterSystem.Water waterScript;

    public float radius = 1.5f; // The radius of the wheel (homogeneous cylinder)
    [Range(0.0f, 3.1f)] public float waterVelocity = 2f; // The velocity of the water striking the paddles of the wheel.
    public float startupTime = 50; // The time it takes for starting the wheel up.  

    private float simTime = 0f; // Defining the time the simulation is running, starting at 0
    private float stepSize = 0f; // The stepsize (h) used in euler approximation. Here it's 1/50 = 0.02
    private float frictionTimer = 0; // Ticking friction in the loop.
    private float frictionStep;

    [SerializeField] private float angularPos;  
    [SerializeField] private float angularVel;  // The angular velocity of the wheel. d(theta)/d(time).
    [SerializeField] private float angularAcc = 0;
    public float[] angularPosArray;             // Saves previous velocity to check for acceleration

    public GameObject house;
    private Color emissiveColor;
    private GameObject[] emissiveObjects;

    private void Start() {
        stepSize = Time.fixedDeltaTime; // Stepsize used in Euler approximation in FixedUpdate().
        frictionStep = 1/startupTime;
        frictionTimer = startupTime;

        swellIncrement = 1f/3.1f;
        waterScript = waterBody.GetComponent<WaterSystem.Water>();
        waterScript.surfaceData._basicWaveSettings.amplitude = 0;

        emissiveObjects = house.GetComponent<Glow>().windows;
        emissiveColor = emissiveObjects[0].GetComponent<Renderer>().material.GetColor("_EmissionColor");

        emissiveObjects[0].GetComponent<Renderer>().material.SetColor("_EmissionColor", emissiveColor * 1.0f);
    }

    // Fixed Update is called a fixed amount of times each second (50/sec)
    void FixedUpdate()
    {
        // Adjust wave height according to water velocity
        waterScript.surfaceData._basicWaveSettings.amplitude = waterVelocity*swellIncrement;

        float lastPos = angularPos;
        float lastVel = angularVel;

        emissiveColor[3] = 0.0f;

        // Friction
        float frictionMultiplier = 1;
        if (frictionTimer <= startupTime / Time.fixedDeltaTime) {
            frictionMultiplier = frictionStep * frictionTimer * Time.fixedDeltaTime * frictionStep * frictionTimer * Time.fixedDeltaTime;
            frictionTimer++; // Counts up from 0 to startupTime.
        }

        // Reset when the wheel have turned 360 degrees to count each turn of the wheel.
        if(angularPos >= 2*Mathf.PI)
        {
            angularPos = 0f;
        }
        else
        {
            simTime += stepSize; // Adds the stepsize each FixedUpdate which corresponds to 50 times per second.
            angularPos += (stepSize/radius) * waterVelocity * frictionMultiplier;
            angularVel = (angularPos - lastPos) / stepSize;
            angularAcc = (angularVel - lastVel) / stepSize;        
        }

        // Change emission intensity in all the windows, based on the wheel velocity
        foreach (var window in emissiveObjects)
        {
            
        }

        // Move the wheel
        Vector3 wheelRot = new Vector3(0, 0, Mathf.Rad2Deg * -(angularPos - lastPos)); // Converting to degrees because of Unity.
        transform.Rotate(wheelRot, Space.Self);
    }
}
