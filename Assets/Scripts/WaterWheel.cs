using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterWheel : MonoBehaviour
{
    public float radius = 1.5f; // The radius of the wheel (homogeneous cylinder)
    public float waterVelocity = 2f; // The velocity of the water striking the paddles of the wheel.

    private float simTime = 0f; // Defining the time the simulation is running, starting at 0
    private float stepSize = 0f; // The stepsize (h) used in euler approximation. Here it's 1/50 = 0.02

    [SerializeField]
    private float angularPos; // The angular velocity of the wheel. d(theta)/d(time).

    private void Start() {
        stepSize = Time.fixedDeltaTime;
    }

    // Fixed Update is called a fixed amount of times each second (50/sec)
    void FixedUpdate()
    {

        simTime += stepSize; // Adds the stepsize each FixedUpdate which corresponds to 50 times per second.
        angularPos += (stepSize/radius) * waterVelocity; 
    }

    private void Update() {
        if(angularPos >= 2*Mathf.PI) {
            angularPos = 0f;
        }
        
        Vector3 wheelRot = new Vector3(0, 0, angularPos);
        //Transform.Rotate();
    } 
}
