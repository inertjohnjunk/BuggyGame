using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSchmoovement : MonoBehaviour
{

    
    public Transform target;
    Vector3 startingDistance;


    //smooth factor smoothes camera rotation
    public float smoothFactor = 0.5f;


    //Logic for checking camera target acquisition
    public bool lookAtTarget = false;

    // Start is called before the first frame update
    void Start()
    {
        startingDistance = transform.position - target.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        FollowPlayer();
        if (lookAtTarget)
        {
            transform.LookAt(target);
        }
    } 
    void FollowPlayer()
    {

        Vector3 newPosition = target.transform.position + startingDistance;
        //transform.position = new Vector3(target.position.x + startingDistance.x, target.position.y/2 + startingDistance.y, target.position.z + startingDistance.z);

        transform.position = Vector3.Slerp(transform.position, newPosition, smoothFactor);
    }
    
}
