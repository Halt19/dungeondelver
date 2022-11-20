using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitScript : MonoBehaviour
{
    private GameMasterScript masterScript;

    private void Start()
    {
        masterScript = GameObject.FindGameObjectWithTag("Master").GetComponent<GameMasterScript>();
    }

    // If the player enters, end the game 
    void OnTriggerEnter(Collider c)
    {
       if (c.gameObject.tag == "Player")
        {   
            masterScript.EndGame();
        }
    }
}
