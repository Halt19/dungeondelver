using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class PickupScript : MonoBehaviour
{
    public int points = 100;
    public int health = 20;

    public void SetPointsAndHealth(int incPoints, int incHealth)
    {
        points = incPoints;
        health = incHealth;
    }

   void OnTriggerEnter(Collider c)
   {
       if (c.gameObject.tag == "Player")
       {
        if (c.gameObject.TryGetComponent<FirstPersonController>(out FirstPersonController controller))
        {
            //c.gameObject.transform.parent.GetComponent<FirstPersonController>().AddPointsToPlayer(points);
            //c.gameObject.transform.parent.GetComponent<FirstPersonController>().HealPlayer(health);
            controller.AddPointsToPlayer(points);
            controller.HealPlayer(health);
            Destroy(this.gameObject);
        }
       }
   }
}
