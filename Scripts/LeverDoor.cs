using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverDoor : MonoBehaviour
{
    [SerializeField]
    private GameObject door;
    private bool isOpen;

    private void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "Player" && !isOpen)
        {
            Debug.Log("Door opening");
            StartCoroutine(OpenDoor());
        }
    }

    IEnumerator OpenDoor()
    {
        isOpen = true;
        for (int i=0 ; i< 30 ; i++)
        {
            door.transform.localPosition = new Vector3(door.transform.localPosition.x, door.transform.localPosition.y - 0.5f, door.transform.localPosition.z);
            yield return new WaitForFixedUpdate();
        }
        GameObject.FindGameObjectWithTag("Navmesh").GetComponent<NavMeshMasterScript>().UpdateNavMeshBake();
    }
}
