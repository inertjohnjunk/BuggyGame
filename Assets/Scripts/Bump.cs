using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bump : MonoBehaviour
{
    public float delayTime = 5;
    public int money = 0;
    public float timer;
    public float timerEnd;
    private GameObject bumped;


    private void FixedUpdate()
    {
        timer = timer + 1;
        if (timer >= 5000)
        {
            timer = 0;
        }
        PickUpRespawn(bumped);
    }
    private void PickUpRespawn(GameObject x)
    {
        if(timer == timerEnd)
        {
            x.SetActive(true);
        }
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "MoneyUP")
        {
            bumped = collider.gameObject;
            timerEnd = timer;
            timer = timer - 100;
            money = money + 10;
            Debug.Log($"{money}");
            collider.gameObject.SetActive(false);

        }

    }
}
