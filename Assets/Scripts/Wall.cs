using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        SnakeController SC = other.GetComponent<SnakeController>();
        if(SC){
            //SC.TriggerDeath();
        }
    }
}
