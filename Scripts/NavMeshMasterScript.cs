using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class NavMeshMasterScript : MonoBehaviour
{
    public GameObject[] floors;
    public GameObject navMaster;
    public NavMeshSurface[] surfaces;
    public NavMeshSurface surface;

    public static NavMeshMasterScript instance;
    // Start is called before the first frame update
    void Start()
    {
        UpdateFloors();
    }

    public void UpdateFloors()
    {
        floors = GameObject.FindGameObjectsWithTag("Floor");
      //  surfaces = new NavMeshSurface[floors.Length];
        for (int i=0; i<floors.Length;i++)
        {
        //    surfaces[i] = floors[i].GetComponent<NavMeshSurface>();
        }
    }

    public void UpdateNavMeshBake()
    {
        Debug.Log("Updating NavMesh Bake");
        this.GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    public NavMeshMasterScript GetInstance()
    {
        if (instance == null) instance = this;
        return instance;
    }
    

    
}
