using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    Drive ds;
    // Start is called before the first frame update
    void OnEnable()
    {
        instance = this;
        ds = this.GetComponent <Drive> ();
        this.GetComponent<Ghost>().enabled = false;
    }
    void Update()
    {
        float a = Input.GetAxis("Vertical");
        float s = Input.GetAxis("Horizontal");
        float b = Input.GetAxis("Jump");
        ds.Go(a, s, b);

        ds.CheckForSkid();
        ds.CalculateEngineRev();

    }
}
