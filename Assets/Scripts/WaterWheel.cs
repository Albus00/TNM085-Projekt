using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaterWheel : MonoBehaviour
{
    public GameObject waterBody;
    private float swellIncrement;
    private WaterSystem.Water waterScript;

    public float radius = 1.5f; // The radius of the wheel (homogeneous cylinder)
    public float innerRadius = 1.3f;
    public float wheelDepth = 0.8f;
    private float wheelCylinderVolume; 
    public float wetWoodDensity = 420.0f;
    public float numberOfPaddles = 8.0f;
    private float paddlesVolume;
    private float wheelMass;
    private float momentOfInertia;
    private float efficiency = 0.20f;
    private float kineticEnergy;
    [SerializeField] private float rotationalEnergy;
    [SerializeField] private float wattOutput;
    public float maxWatt = 335;

    public Slider velocitySlider;
    public TextMeshProUGUI wattageText;


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

        wheelCylinderVolume = (Mathf.Pow(radius, 2.0f)*Mathf.PI*wheelDepth) - (Mathf.Pow(innerRadius, 2.0f)*Mathf.PI*wheelDepth); 
        paddlesVolume = numberOfPaddles*0.4f*0.1f*0.8f;
        wheelMass = (wheelCylinderVolume + paddlesVolume)*wetWoodDensity;
        momentOfInertia = (wheelMass*Mathf.Pow(radius, 2.0f))/2.0f;
    }

    // Fixed Update is called a fixed amount of times each second (50/sec)
    void FixedUpdate()
    {
        // Get velocity from UI slider
        waterVelocity = velocitySlider.value;

        // Converting kinetic energy to rotational energy.
        kineticEnergy = (wheelMass * Mathf.Pow(waterVelocity, 2.0f));
        float oldRot = rotationalEnergy;
        rotationalEnergy = (efficiency * momentOfInertia * Mathf.Pow(angularVel, 2.0f))/2.0f;


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

            wattOutput = rotationalEnergy;
            wattageText.text = Mathf.Round(wattOutput * 100f) / 100f + "W";
        }

        // Change emission intensity in all the windows, based on the wheel velocity
        foreach (var window in emissiveObjects)
        {
            window.GetComponent<Renderer>().material.SetColor("_EmissionColor", emissiveColor * wattOutput/maxWatt);
        }

        // Move the wheel
        Vector3 wheelRot = new Vector3(0, 0, Mathf.Rad2Deg * -(angularPos - lastPos)); // Converting to degrees because of Unity.
        transform.Rotate(wheelRot, Space.Self);
    }
}
