using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{

    //Repetition:
    /*
     * CRUD: Create.Read.Update.Delete
     * 
     * 
     *
     
     */
    Drive ds;

    //Sensitivity modifiers
    public float accelSensitivity = 0.3f;
    public float steeringSensitivity = 0.01f;
    public float brakingSensitivity = 3f;



    //navigation variables
    public Circuit circuit;
    public GameObject tracker;
    int currentTrackerWP = 0;
    public float lookAhead = 12;
    float totalDistanceToTarget;
    private float distPerc;
    Vector3 target;
    Vector3 nextTarget;
    int currentWP = 0;
    public float turnAboutTime = 500;

    //Reset Variables
    float lastTimeMoved = 0;


    // Start is called before the first frame update
    void Start()
    {





        ds = this.GetComponent<Drive>();
        target = circuit.waypoints[currentWP].transform.position;
        nextTarget = circuit.waypoints[currentWP+1].transform.position;
        totalDistanceToTarget = Vector3.Distance(target, ds.rb.gameObject.transform.position);

        tracker = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        DestroyImmediate(tracker.GetComponent<Collider>());
        tracker.GetComponent<MeshRenderer>().enabled = false;
        tracker.transform.position = ds.rb.gameObject.transform.position;
        tracker.transform.rotation = ds.rb.gameObject.transform.rotation;

        this.GetComponent<Ghost>().enabled = false;
    }

    void ProgressTracker()
    {
        Debug.DrawLine(ds.rb.gameObject.transform.position, tracker.transform.position);

        if (Vector3.Distance(ds.rb.gameObject.transform.position, tracker.transform.position) > lookAhead) return;
        tracker.transform.LookAt(circuit.waypoints[currentTrackerWP].transform.position);
        tracker.transform.Translate(0, 0, 1.0f); // tracker SPEEEED!!
        if (Vector3.Distance(tracker.transform.position, circuit.waypoints[currentTrackerWP].transform.position) < 1)
        {
            currentTrackerWP++;
            if (currentTrackerWP >= circuit.waypoints.Length)
            {
                currentTrackerWP = 0;
            }
        }
    }

    void ResetLayer()
    {
        ds.rb.gameObject.layer = 10;
        this.GetComponent<Ghost>().enabled = false;
    }



    // Update is called once per frame
    void Update()
    {
        //WIP field of view code, continuation not guaranteed
        /* if (Physics.SphereCast(transform.position, 40, transform.forward)
        {
            
        }
        */

        ProgressTracker();
        Vector3 localTarget;
        float targetAngle;

        if (ds.rb.velocity.magnitude > 1)
        {
            lastTimeMoved = Time.time;
        }


        //Resets the vehicle if stuck for too long
        if ((Time.time > lastTimeMoved + 4) || ds.rb.gameObject.transform.position.y < tracker.transform.position.y -10)
        {
            float xDisp = Random.Range(-0.5f, 0.5f);
            float zDisp = Random.Range(-0.5f, 0.5f);
            Vector3 newReset = new Vector3(tracker.transform.position.x + xDisp, tracker.transform.position.y +0.2f, tracker.transform.position.z + zDisp);
            ds.rb.gameObject.transform.position = newReset;
            ds.rb.gameObject.layer = 8;
            this.GetComponent<Ghost>().enabled = true;
            Invoke("ResetLayer", 4);

            //Obsolete, less customizable upwards reset displacement.
            /*
            ds.rb.gameObject.transform.position = tracker.transform.position += Vector3.up;
            */

            ds.rb.gameObject.transform.rotation = tracker.transform.rotation;
            tracker.transform.position = ds.rb.gameObject.transform.position;
        }



        if (Time.time < ds.rb.GetComponent<Avoidance>().avoidTime)
        {
            localTarget = tracker.transform.right * ds.rb.GetComponent<Avoidance>().avoidPath;
        }
        else
        {
            localTarget = ds.rb.gameObject.transform.InverseTransformPoint(tracker.transform.position);
        }
        targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

        float steer = Mathf.Clamp(targetAngle * steeringSensitivity, -1, 1) * Mathf.Sign(ds.currentSpeed);

        float speedFactor = ds.currentSpeed / ds.maxSpeed;

        float corner = Mathf.Clamp(Mathf.Abs(targetAngle), 0, 90);
        float cornerFactor = corner / 90.0f;

        float brake = 0;
        if (corner > 5 && speedFactor > 0.1f)
        {
            brake = Mathf.Lerp(0, 1 + speedFactor * brakingSensitivity, cornerFactor);
        }
        float accel = 1;
        if (corner > 20 && speedFactor > 0.2f)
        {
            accel = Mathf.Lerp(0, 1 * accelSensitivity, 1 - cornerFactor);
        }

        if (tracker.transform.position.y > ds.rb.gameObject.transform.position.y + 2f)
        {
            brake = 0;
            accel = 1f;
        }

        ds.Go(accel, steer, brake);

        ds.CheckForSkid();
        ds.CalculateEngineRev();
        
        
        //float distanceToTarget = Vector3.Distance(target, ds.rb.gameObject.transform.position);

        //Calculates the angle of attack to the targeted waypoint.
        //

        //steers accordingly with respect to the targets angle.

        //float distanceFactor = distanceToTarget / totalDistanceToTarget;
        //

        //distPerc = (1/distanceToTarget);


        //float accel = Mathf.Lerp(accelSensitivity, 1, distanceFactor);
        //




        /*
        //converts distanceToTarget into a usable percentage


        //Uses that percentage to increase steeringSensitivity inversely proportional to DistanceToTarget
        //Divided to avoid hypersensitive steering when too close to waypoints.
        if (targetAngle > 30)
        {
            steeringSensitivity = 1;
        }
        else
        {
            steeringSensitivity = (distPerc / 10)/10;
        }
        /*
        if (distanceToTarget < 25)
        {
            if (ds.currentSpeed > 80)
            {
                brake = 1;
                accel = 0;
            }
            else if (ds.currentSpeed > 30)
            {
                brake = 1;
                accel = 0.5f;
            }
            else if (ds.currentSpeed > 15)
            {
                brake = 0.5f;
                accel = 0.9f;
            }
            else if (ds.currentSpeed >5)
            {
                brake = 0;
                accel = 1;
            }

        }
        */


    }
    void TurnCheck(float x)
    {
        if (x > 20 || x < -20)
        {
            brakingSensitivity = 1;
        }
    }

}
