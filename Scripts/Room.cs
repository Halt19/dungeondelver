using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public bool DoorNorth;
    public bool DoorEast;
    public bool DoorSouth;
    public bool DoorWest;

    public Room()
    {
        DoorNorth = false;
        DoorEast = false;
        DoorSouth = false;
        DoorWest = false;
    }
}
