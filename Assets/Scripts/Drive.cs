using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour
{
    //Coding terminology for memorization:
    /*Field: variables defined by a class
     * Constructor: class able to take parameters. If not written manually, it is automatically provided by Visual Studio.
     * 
     * operand: 
     
     */
    //"General" Variables.
    [Header ("General")]
    public static Drive instance;
    public WheelCollider[] WCs;
    public GameObject[] Wheels;
    public float torque = 700;
    public float maxSteerAngle = 30;
    public float maxBrakeTorque = 500;
    public Rigidbody rb;
    public float minimumSpeed;
    Vector3 resetPosition;

    //Otherscript references
    Avoidance avo;
    AIController AI;

    public AudioSource skidSound;
    public Transform SkidTrailPrefab;
    Transform[] skidTrails = new Transform[4];

    public GameObject brakeLight;

    //Variables for calculating engine pitch, volume, etc.
    [Header("Engine Sounds")]
    public AudioSource highAccel;
    public float gearLength = 3;
    public float currentSpeed { get { return rb.velocity.magnitude * gearLength; } }
    public float lowPitch = 1f;
    public float highPitch = 6F;
    public int numGears = 5;
    float rpm;
    int currentGear = 1;
    float currentGearPerc;


    public float maxSpeed = 200;

    public void StartSkidTrail(int i)
    {
        if (skidTrails[i] == null)
        {
            skidTrails[i] = Instantiate(SkidTrailPrefab);


        }
        skidTrails[i].parent = WCs[i].transform;
        skidTrails[i].localRotation = Quaternion.Euler(90, 0, 0);
        skidTrails[i].localPosition = -Vector3.up * WCs[i].radius;
    }

    public void EndSkidTrail(int i)
    {
        if (skidTrails[i] == null) return;
        Transform holder = skidTrails[i];
        skidTrails[i] = null;
        holder.parent = null;
        holder.rotation = Quaternion.Euler(90, 0, 0);
        Destroy(holder.gameObject, 30);
    }

    // Start is called before the first frame update
    void Start()
    {
        brakeLight.SetActive(false);
    }

    public void CalculateEngineRev()
    {
        float gearPercentage = (1 / (float)numGears);
        float targetGearFactor = Mathf.InverseLerp(gearPercentage * currentGear, gearPercentage * (currentGear + 1),
                            Mathf.Abs(currentSpeed / maxSpeed));
        currentGearPerc = Mathf.Lerp(currentGearPerc, targetGearFactor, Time.deltaTime * 5f);

        var gearNumFactor = currentGear/ (float)numGears;
        rpm = Mathf.Lerp(gearNumFactor, 1, currentGearPerc);

        float speedPercentage = Mathf.Abs(currentSpeed / maxSpeed);
        float upperGearMax = (1 / (float)numGears) * (currentGear + 1);
        float downGearMax = (1 / (float)numGears) * currentGear;
        if (currentGear > 0 && speedPercentage < downGearMax)

        {
            currentGear--;
        }
        if (speedPercentage > upperGearMax && (currentGear < (numGears - 1)))
        {
            currentGear++;
        }
        float pitch = Mathf.Lerp(lowPitch, highPitch, rpm);
        highAccel.pitch = Mathf.Min(highPitch, pitch) * 0.25f;
    }


    //Go Handles everything to do with acceleration, braking, and turning.
    public void Go(float accel, float steer, float brake)
    {

            accel = Mathf.Clamp(accel, -1, 1);
            steer = Mathf.Clamp(steer, -1, 1) * maxSteerAngle;
            brake = Mathf.Clamp(brake, 0, 1) * maxBrakeTorque;

        if (brake != 0)
        {
            brakeLight.SetActive(true);
        }
        else
        {
            brakeLight.SetActive(false);
        }
        float thrustTorque = 0;
        if (currentSpeed < maxSpeed)
        {
            thrustTorque = accel * torque;
           
        }

        //sets/normalizes torque between wheels and aligns their mesh-renders with the colliders
        for (int i = 0; i < 4; i++)
        {
            if (i > 1)
            {
                    WCs[i].motorTorque = thrustTorque;

            }


            if (i < 2)
            {
                WCs[i].steerAngle = steer;
            }
            else
            {
                WCs[i].brakeTorque = brake;
            }
            Quaternion quat;
            Vector3 position;
            WCs[i].GetWorldPose(out position, out quat);
            Wheels[i].transform.position = position;
            Wheels[i].transform.rotation = quat;
        }
    }

    public void CheckForSkid()
    {
        int numSkidding = 0;
        for (int i = 0; i < 4; i++)
        {
            WheelHit wheelHit;
            WCs[i].GetGroundHit(out wheelHit);

            if (Mathf.Abs(wheelHit.forwardSlip) >= 0.25f || Mathf.Abs(wheelHit.sidewaysSlip) >= 0.25f)
            {

                numSkidding++;
                if (!skidSound.isPlaying)
                {
                    skidSound.Play();
                }
                //StartSkidTrail(i);

            }
            else
            {
                //EndSkidTrail(i);
            }
            skidSound.volume = Mathf.Clamp((Mathf.Abs(wheelHit.sidewaysSlip) + Mathf.Abs(wheelHit.forwardSlip)), 0, 1);
        }
        if (numSkidding == 0 && skidSound.isPlaying)
        {
            skidSound.Stop();
        }
    }


}
