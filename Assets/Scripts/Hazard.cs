using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Lava!");
        if (collision.gameObject.tag == "Player")
        {
            Game.singleton.Lose();
        }
    }
}
