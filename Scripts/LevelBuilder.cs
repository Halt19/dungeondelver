using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using StarterAssets;

public class LevelBuilder : MonoBehaviour
{

    private float sizeOfTile = 50f;

    public TextAsset file;

    private GameObject[][] rooms;
    private int[][] rotations;
    private GameObject room1;
    private GameObject room2;
    private GameObject room3;
    private GameObject room4;
    private GameObject room5;
    private GameObject room6;
    private GameObject room7;
    private GameObject room8;
    private GameObject room9;
    private GameObject exitPrefab;

    public Room[][] room;


    public int seed = 1834784;
    public GameObject pickup;
    public GameObject enemy;
    [SerializeField]
    private GameObject player;
    public int nothingWeight = 100;
    public int pickupWeight = 10;
    public int enemyWeight = 15;
    private bool[][] connected;
    private NavMeshPath path;
    private GameObject[] spawnPoints; // one dimensional
    private float mutationRate = 5f; // set to 100 for fully random

    private int enemyMinimumHealth = 60;
    private int enemyMaximumHealth = 120;
    private int enemyMinimumDamage = 2;
    private int enemyMaximumDamage = 20;
    private int pickupMinimumHealth = 0;
    private int pickupMaximumHealth = 30;
    private int pickupMinimumPoints = 50;
    private int pickupMaximumPoints = 200;


    private int targetEnemyNumbers = 140;
    private int targetEnemyHealth = 11200;
    private int targetEnemyDamage = 1400;

    private int targetPickupNumbers = 140;
    private int targetPickupHealth = 2100;
    private int targetPickupPoints = 17500;

    private float enemyGunRate = 0.5f;

    public int loadProgress = 0;

    private float mismatchDoorCloseRate = 0.75f;

    private float bridgeRate = 0.75f;

    private int populationSize = 40; // 80
    private int NumElites = 7; // 5
    public int NumGenerations = 80; // 150

    private int TargetLength = 100;

    private string[] generation;
    private string[] elites;

    private int pathPossibleTolerance = 80;

    private string string2 = "R8;R8;R8;R8;R8;R8;R8;R8;R8;R8_R7;R7;R7;R7;R7;R7;R7;R7;R7;R7_R6;R6;R6;R6;R6;R6;R6;R6;R6;R6_R5;R5;R5;R5;R5;R5;R5;R5;R5;R5_R4;R4;R4;R4;R4;R4;R4;R4;R4;R4_R3;R3;R3;R3;R3;R3;R3;R3;R3;R3_R2;R2;R2;R2;R2;R2;R2;R2;R2;R2_R1;R1;R1;R1;R1;R1;R1;R1;R1;R1_R1;R1;R1;R1;R1;R1;R1;R1;R1;R1_R1;R1;R1;R1;R1;R1;R1;R1;R1;R1";

    public GeneticAlgorithm<String> geneticAlgorithm;

    // Start is called before the first frame update
    void Start()
    {
        path = new NavMeshPath();
        room1 = (GameObject)Resources.Load("Rooms/Room1_ +");
        room2 = (GameObject)Resources.Load("Rooms/Room2_ DeadEnd");
        room3 = (GameObject)Resources.Load("Rooms/Room3_ L");
        room4 = (GameObject)Resources.Load("Rooms/Room4_ I");
        room5 = (GameObject)Resources.Load("Rooms/Room5_ I Hall");
        room6 = (GameObject)Resources.Load("Rooms/Room6_ + Platform");
        room7 = (GameObject)Resources.Load("Rooms/Room7_ I Platform");
        room8 = (GameObject)Resources.Load("Rooms/Room8_ 3-Exit");
        room9 = (GameObject)Resources.Load("Rooms/Room9_ Empty");
        exitPrefab = (GameObject)Resources.Load("Exit");
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    /*
    I hated making this one

    Provided with all the relevant values from the master script, sets this script's values.
    */
    public void SetGenerationInfo(int nothingWeightInc, int pickupWeightInc, int enemyWeightInc, int enemyMinimumHealthInc, int enemyMaximumHealthInc, 
    int enemyMinimumDamageInc, int enemyMaximumDamageInc, int pickupMinimumHealthInc, int pickupMaximumHealthInc, int pickupMinimumPointsInc, 
    int pickupMaximumPointsInc, int targetEnemyNumbersInc, int targetEnemyHealthInc, int targetEnemyDamageInc, int targetPickupNumbersInc, 
    int targetPickupHealthInc, int targetPickupPointsInc, float mutationRateInc, float mismatchDoorCloseRateInc, float bridgeRateInc, int populationSizeInc,
    int NumElitesInc, int NumGenerationsInc, int targetLengthInc, float enemyGunRateInc)
    {
        nothingWeight = nothingWeightInc;
        pickupWeight = pickupWeightInc;
        enemyWeight = enemyWeightInc;
        enemyMinimumHealth = enemyMinimumHealthInc;
        enemyMaximumHealth = enemyMaximumHealthInc;
        enemyMinimumDamage = enemyMinimumDamageInc;
        enemyMaximumDamage = enemyMaximumDamageInc;
        pickupMinimumHealth = pickupMinimumHealthInc;
        pickupMaximumHealth = pickupMaximumHealthInc;
        pickupMinimumPoints = pickupMinimumPointsInc;
        pickupMaximumPoints = pickupMaximumPointsInc;
        targetEnemyNumbers = targetEnemyNumbersInc;
        targetEnemyHealth = targetEnemyHealthInc;
        targetEnemyDamage = targetEnemyDamageInc;
        targetPickupNumbers = targetPickupNumbersInc;
        targetPickupHealth = targetPickupHealthInc;
        targetPickupPoints = targetPickupPointsInc;
        mutationRate = mutationRateInc;
        mismatchDoorCloseRate = mismatchDoorCloseRateInc;
        bridgeRate = bridgeRateInc;
        populationSize = populationSizeInc;
        NumElites = NumElitesInc;
        NumGenerations = NumGenerationsInc;
        TargetLength = targetLengthInc;
        enemyGunRate = enemyGunRateInc;
    } // SetGenerationInfo

    /*
    I don't know what to put here - this one literally only exists so the GameMasterScript has something public to call, I probably should have just made RunGeneticAlgorithm public instead.
    */
    public void Generate()
    {
        RunGeneticAlgorithm();
    } // Generate

    /*
    Finishes the generation - makes the final Room, then actually builds out the dungeon, and finally updates the NavMesh bake.
    */
    private void FinishGeneration(string[][] inputData)
    {
        room = MainStringToRoom(inputData);
        room = HandleConflictingDoors(room); // THIS is what's causing the drastic difference before/after closure, and was responsible for the difference in lengths
        room = CloseOffEdgeDoors(room); // closes off edges so you can't walk out the side of the dungeon into the void
        inputData = RoomToMainString(room, inputData);
        inputData = PlaceExit(inputData, RoomsDistances(room, inputData));

        rooms = new GameObject[inputData.Length][];
        bool[][] isConnected = RoomsAreReachable(room, inputData);
        for (int i=0 ; i<inputData.Length; i++)
        {
            rooms[i] = new GameObject[inputData[i].Length];
            for (int j=0 ; j<inputData[i].Length; j++)
            {
                if (isConnected[i][j]) // only make rooms which are connected to the main dungeon, to save resources
                {
                    rooms[i][j] = StringToRoom(inputData[i][j], i, j, (inputData.Length/2), (inputData[i].Length/2));
                }
            }
        }
        UpdateNavMeshBake();
    } // FinishGeneration

    /*
    Generates a string[][] based on a predetermined file on disk.
    */
    private string[][] RunFromFile()
    {
        string temp = file.text.Replace("\r\n", "_");
        string[] content = temp.Split('_');
        string[][] inputData = new string[content.Length][];
        for (int i=0 ; i<content.Length; i++)
        {
            inputData[i] = content[i].Split(';');
        }
        return inputData;
    } // RunFromFile

    /*
    3 guesses what this one does

    Used to be the main method, but now only exists to activate the coroutine which handles everything this one used to, but independently.
    */
    private void RunGeneticAlgorithm()
    {
        StartCoroutine(RunGAIndependently());
    } // RunGeneticAlgorithm

    /*
    Used to be a method, but now is an IEnumerator so it can run independently. Before, it would freeze the application until generation was complete,
    but this way allows it to have a progress bar. One concern is that it doesn't check the time within the frame, so it will max out at one generation
    per frame even if the system is more efficient than that, but a) I don't think the system is efficient enough where it actually handles one generation
    per frame, and b) even if it did, it's a tiny difference, and doesn't get noticed even on high GA values. Also, it gives time to show off the fancy load
    screen.
    */
    private IEnumerator RunGAIndependently()
    {
        System.Random random = new System.Random();
		GeneticAlgorithm<string[][]> ga = new GeneticAlgorithm<string[][]>(populationSize, random, NumElites, this, mutationRate);
        for (loadProgress=0 ; loadProgress<NumGenerations ; loadProgress++)
        {
            ga.NewGeneration();
            yield return null;
        }
        FinishGeneration(ga.BestLayout);
    } // RunGAIndependently

    /*
    Provided a Room[][], returns the equivalent in string[][] form, based on each Room's door configuration.
    */
    public string[][] RoomToMainString(Room[][] incRoom, string[][] incString)
    {
        int index;
        int oldRoomNum;
        for (int i=0 ; i<incRoom.Length ; i++)
        {
            for (int j=0 ; j<incRoom[i].Length ; j++)
            {
                if (incString[i][j].IndexOf("R") > -1)
                {
                    if (incString[i][j].IndexOf("Q") > -1)
                    {
                        incString[i][j] = incString[i][j].Remove(incString[i][j].IndexOf("Q"), 2); // remove the rotation value so it can be added in recalculation
                    }
                    oldRoomNum = int.Parse(incString[i][j].Substring(incString[i][j].IndexOf("R")+1, 1));
                    index = incString[i][j].IndexOf("R")+1; // doesn't remove the old room number, but doesn't matter
                    incString[i][j] = incString[i][j].Insert(index, IdentifyRoomTypeToString(incRoom[i][j]));
                }
            }
        }
        return incString;
    } // RoomToMainString

    /*
    Uses recursion to return an int[][] the size of the dungeon - each int represents how far each room is from the spawn point of the player.
    */
    private int[][] DistanceFromSpawnpoint(Room[][] inc, int i, int j, int distance, int[][] output)
    {
        if (distance < output[i][j])
        {
            output[i][j] = distance;
        } else
        {
            distance = output[i][j];
        }

        if (inc[i][j].DoorNorth && inc[i].Length > j+1 && inc[i][j+1].DoorSouth && output[i][j+1] > output[i][j]+1)
        {
            output = DistanceFromSpawnpoint(inc, i, j+1, distance+1, output);
        }

        if (inc[i][j].DoorEast && inc.Length > i+1 && inc[i+1][j].DoorWest && output[i+1][j] > output[i][j]+1)
        {
            output = DistanceFromSpawnpoint(inc, i+1, j, distance+1, output);
        }

        if (inc[i][j].DoorSouth && j>0 && inc[i][j-1].DoorNorth && output[i][j-1] > output[i][j]+1)
        {
            output = DistanceFromSpawnpoint(inc, i, j-1, distance+1, output);
        }

        if (inc[i][j].DoorWest && i > 0 && inc[i-1][j].DoorEast && output[i-1][j] > output[i][j]+1)
        {
            output = DistanceFromSpawnpoint(inc, i-1, j, distance+1, output);
        }
        return output;
    } // DistanceFromSpawnpoint

    /*
    Calculates the fitness for a dungeon, provided its string[][] and Room[][] representations.
    */
    public float CalculateFitness(string[][] inc, Room[][] incRoom)
    {
        float output = 0;
        output += DistanceFromTarget(inc, incRoom, 0); // num pickups
        output += DistanceFromTarget(inc, incRoom, 1); // puckup point values
        output += DistanceFromTarget(inc, incRoom, 2); // pickup health provided
        output += DistanceFromTarget(inc, incRoom, 3); // num enemies
        output += DistanceFromTarget(inc, incRoom, 4); // enemy health
        output += DistanceFromTarget(inc, incRoom, 5); // enemy damage
        output += DistanceFromTarget(inc, incRoom, 6); // length
        return output;
    } // CalculateFitness

    /*
    Holds all the fitness function values - each operation corresponds to an integer provided with method call.
    IE passing 0 will make it evaluate the string based on the number of pickups, compared to the global goal,
    6 will compare the overall length of the dungeon against the provided requested length, etc. Basically a
    glorified "how far is this dungeon's value from the target value" function, that searches the string, which
    stores the values for this purpose - now it doesn't need to manually count up each enemy and pickup in the 
    string, because when those are added to the string, the string's internal count of total values is updated,
    and here is where that is used.
    */
 // 0 num pickups 1 pickup values 2 pickup healths 3 num enemies 4 enemy healths 5 enemy damages 6 overall length
    private float DistanceFromTarget(string[][] inc, Room[][] incRoom, int op)
    {
        float output = 0;
        int val = 999999;
        if (op == 0)
        {
            int index1 = inc[0][0].IndexOf("?");
            int index2 = inc[0][0].IndexOf("?", index1+1);
            if (index1 > -1 && index2 > -1)
            {
                val = int.Parse(inc[0][0].Substring(index1+1, index2-(index1+1)));
            }
            output = 500 - Mathf.Abs(targetPickupNumbers - val)/500;
            return output;
        }
        if (op == 1)
        {
            int index1 = inc[0][0].IndexOf("?");
            int index2 = inc[0][0].IndexOf("?", index1+1);
            index1 = index2;
            index2 = inc[0][0].IndexOf("?", index1+1);
            if (index1 > -1 && index2 > -1)
            {
                val = int.Parse(inc[0][0].Substring(index1+1, index2-(index1+1)));
            }
            output = 500 - Mathf.Abs(targetPickupHealth - val)/500;
            return output;
        }
        if (op == 2)
        {
            int index1 = inc[0][0].IndexOf("?");
            int index2 = inc[0][0].IndexOf("?", index1+1);
            index1 = index2;
            index2 = inc[0][0].IndexOf("?", index1+1);
            index1 = index2;
            index2 = inc[0][0].IndexOf("?", index1+1);
            if (index1 > -1 && index2 > -1)
            {
                val = int.Parse(inc[0][0].Substring(index1+1, index2-(index1+1)));
            }
            output = 500 - Mathf.Abs(targetPickupPoints - val)/500;
            return output;
        }
        if (op == 3)
        {
            int index1 = inc[0][0].IndexOf(",");
            int index2 = inc[0][0].IndexOf(",", index1+1);
            if (index1 > -1 && index2 > -1)
            {
                val = int.Parse(inc[0][0].Substring(index1+1, index2-(index1+1)));
            }
            output = 500 - Mathf.Abs(targetEnemyNumbers - val)/500;
            return output;
        }
        if (op == 4)
        {
            int index1 = inc[0][0].IndexOf(",");
            int index2 = inc[0][0].IndexOf(",", index1+1);
            index1 = index2;
            index2 = inc[0][0].IndexOf(",", index1+1);
            if (index1 > -1 && index2 > -1)
            {
                val = int.Parse(inc[0][0].Substring(index1+1, index2-(index1+1)));
            }
            output = 500 - Mathf.Abs(targetEnemyHealth - val)/500;
            return output;
        }

        if (op == 5)
        {
            int index1 = inc[0][0].IndexOf(",");
            int index2 = inc[0][0].IndexOf(",", index1+1);
            index1 = index2;
            index2 = inc[0][0].IndexOf(",", index1+1);
            index1 = index2;
            index2 = inc[0][0].IndexOf(",", index1+1);
            if (index1 > -1 && index2 > -1)
            {
                val = int.Parse(inc[0][0].Substring(index1+1, index2-(index1+1)));
            }
            output = 500 - Mathf.Abs(targetEnemyDamage - val)/500;
            return output;
        }

        if (op == 6)
        { // length comparison
            output = 1000 - Mathf.Abs(TargetLength - MaxDistance(RoomsDistances(incRoom, inc)));
            return output;
        }
        Debug.LogError("Critical error in fitness function"); // if we get here something's VERY wrong, so return a very large negative number to ensure it dies in the genetic algorithm and isn't picked
        return -99999999;
    } // DistanceFromTarget

    /*
    Given an int[][], returns the value of the largest individual int in it that is not MaxValue.
    */
    private int MaxDistance(int[][] inc)
    {
        int output = 0;
        for (int i=0 ; i<inc.Length ; i++)
        {
            for (int j=0 ; j<inc[i].Length ; j++)
            {
                if (inc[i][j] < int.MaxValue && inc[i][j]>output)
                {
                    output = inc[i][j];
                }
            }
        }
        return output;
    } // MaxDistance

    /*
    Returns an int[][] the size of the dungeon - each int represents the number of rooms it is away from where the player starts. 
    MaxValue represents unreachable rooms.
    Calls DistanceFromSpawnpoint, a recursive method, with initialized values.
    */
    private int[][] RoomsDistances(Room[][] inc, string[][] incString)
    {
        int[][] output = new int[inc.Length][];
        for (int i=0 ; i<inc.Length ; i++)
        {
            output[i] = new int[inc[i].Length];
            for (int j=0 ; j<inc[i].Length ; j++)
            {
                output[i][j] = int.MaxValue;
            }
        }

        for (int i=0 ; i<incString.Length ; i++)
        {
            for (int j=0 ; j<incString[i].Length ; j++)
            {
                if (incString[i][j].Contains("P"))
                {
                    int lifetime = output.Length + output[0].Length;
                    output = DistanceFromSpawnpoint(inc, i, j, 0, output);
                    return output;
                }
            }
        }
        return output;
    } // RoomsDIstances
 
    /*
    Parent of DetermineReachability recursive method - calls it with initialized values. End result
    is a bool[][] where each bool represents whether or not that room can be reached by the player's 
    starting point (true) or not (false). I considered using a lifetime to prevent overload, but found
    it wasn't needed, as I instead implemented a direct check into the recursive method which only triggers
    the recursion if the room being checked has not already been evaluated as having been reachable. This
    means that the recursion will not run for the same room multiple times AFTER IT HAS BEEN FLAGGED AS POSSIBLE
    TO REACH, so the process dies out much more quickly and doesn't loop forever on itself.
    */
    private bool[][] RoomsAreReachable(Room[][] inc, string[][] incString)
    {
        bool[][] output = new bool[inc.Length][];
        for (int i=0 ; i<inc.Length ; i++)
        {
            output[i] = new bool[inc[i].Length];
            for (int j=0 ; j<inc[i].Length ; j++)
            {
                output[i][j] = false; // initialize the output bool to match the size of the input Room[][] dungeon
            }
        }

        for (int i=0 ; i<incString.Length ; i++)
        {
            for (int j=0 ; j<incString[i].Length ; j++)
            {
                if (incString[i][j].Contains("P"))
                {
                    output = DetermineReachability(inc, i, j, output); // the DetermineReachability function starts at the player's start location
                    return output;
                }
            }
        }
        return output;
    } // RoomsAreReachable

    /*
    Returns a bool[][]; each bool represents whether that room is reachable from the starting room, recursively.
    */
    private bool[][] DetermineReachability(Room[][] inc, int i, int j, bool[][] output)
    { // RECURSIVE
        output[i][j] = true; // if this method is being called, then this room (represented by i/j coords) is reachable
        // REMEMBER: I IS HORIZONTAL, J IS NORTH/SOUTH
        if (inc[i][j].DoorNorth && inc[i].Length > j+1 && inc[i][j+1].DoorSouth && !output[i][j+1]) // the "!output[i][j+1]" is checking the next room's value, to see if it has already been evaluated as true. This is the exit condition for the recursive function. 
        {
            output = DetermineReachability(inc, i, j+1, output);
        }

        if (inc[i][j].DoorEast && inc.Length > i+1 && inc[i+1][j].DoorWest && !output[i+1][j])
        {
            output = DetermineReachability(inc, i+1, j, output);
        }

        if (inc[i][j].DoorSouth && j>0 && inc[i][j-1].DoorNorth && !output[i][j-1])
        {
            output = DetermineReachability(inc, i, j-1, output);
        }

        if (inc[i][j].DoorWest && i > 0 && inc[i-1][j].DoorEast && !output[i-1][j])
        {
            output = DetermineReachability(inc, i-1, j, output);
        }

        return output;
    } // DetermineReachability

    /*
    Converts the provided Room into a string, based on door configuration - each string represents one of the pre-made room types in the dungeon. String representation
    allows for the room to be generated.
    */
    private string IdentifyRoomTypeToString(Room inc)
    {
        int num = 0; // the number of doors the room has will narrow down which room type it is - for instance, a room with 2 doors open and 2 closed rules out rooms 1, 6, 8, and 9
        if (inc.DoorNorth) 
        {
            num++;
        }
        if (inc.DoorEast) 
        {
            num++;
        }
        if (inc.DoorSouth) 
        {
            num++;
        }
        if (inc.DoorWest) 
        {
            num++;
        }
        if (num == 4)
        { 
            // Rooms 1 & 6
            if (UnityEngine.Random.value < bridgeRate)
            {
                // Room 1
                return "1";
            } else
            {
                // Room 6
                if (UnityEngine.Random.value < 0.5f)
                {
                    return "6";
                }
                else
                {
                    return "6Q1";
                }
            }
        }

        if (num == 3)
        {
            // Room 8
            if (!inc.DoorNorth)
            {
                return "8Q2";
            }
            if (!inc.DoorEast)
            {   
                return "8Q3";
            }
            if (!inc.DoorSouth)
            {
                return "8";
            }
            if (!inc.DoorWest)
            {
                return "8Q1";
            }
        }

        if (num == 2)
        {
            // Room 3
            if (!inc.DoorNorth && !inc.DoorEast && inc.DoorSouth && inc.DoorWest)
            {
                return "3";
            }
            if (inc.DoorNorth && !inc.DoorEast && !inc.DoorSouth && inc.DoorWest)
            {
                return "3Q1";
            }
            if (inc.DoorNorth && inc.DoorEast && !inc.DoorSouth && !inc.DoorWest)
            {
                return "3Q2";
            }
            if (!inc.DoorNorth && inc.DoorEast && inc.DoorSouth && !inc.DoorWest)
            {
                return "3Q3";
            }

            // Rooms 4, 5, & 7
            if (!inc.DoorNorth && inc.DoorEast && !inc.DoorSouth && inc.DoorWest)
            {
                if (UnityEngine.Random.value > bridgeRate)
                {
                    return "7";
                } 
                else
                {
                    if (UnityEngine.Random.value < 0.5f)
                    {
                        return "4";
                    } else
                    {
                        return "5";
                    }
                }
            }
            if (inc.DoorNorth && !inc.DoorEast && inc.DoorSouth && !inc.DoorWest)
            {
                if (UnityEngine.Random.value > bridgeRate)
                {
                    return "7Q1";
                } 
                else
                {
                    if (UnityEngine.Random.value < 0.5f)
                    {
                        return "4Q1";
                    } else
                    {
                        return "5Q1";
                    }
                }
            }
        }

        if (num == 1)
        {
            // Room 2
            if (inc.DoorNorth)
            {
                return "2";
            }
            if (inc.DoorEast)
            {
                return "2Q1";
            }
            if (inc.DoorSouth)
            {
                return "2Q2";
            }
            if (inc.DoorWest)
            {
                return "2Q3";
            }
        }
        return "9";
    } // IdentifyRoomTypeToString

    /*
    Removes door-wall interactions in the Room[][] provided - instead each door-wall becomes door-door or wall-wall.
    */
    public Room[][] HandleConflictingDoors(Room[][] inc)
    {
        for (int i=0 ; i<inc.Length ; i++)
        {
            for (int j=0 ; j<inc[i].Length ; j++)
            {
                if (i > 0)
                {
                    if (inc[i][j].DoorWest && !inc[i-1][j].DoorEast || !inc[i][j].DoorWest && inc[i-1][j].DoorEast)
                    {
                        if (UnityEngine.Random.value < mismatchDoorCloseRate)
                        {
                            inc[i][j].DoorWest = false;
                            inc[i-1][j].DoorEast = false;
                        } else
                        {
                            inc[i][j].DoorWest = true;
                            inc[i-1][j].DoorEast = true;
                        }
                    }
                }
                if (i < inc.Length - 1)
                {
                    if (inc[i][j].DoorEast && !inc[i+1][j].DoorWest || !inc[i][j].DoorEast && inc[i+1][j].DoorWest)
                    {
                        if (UnityEngine.Random.value < mismatchDoorCloseRate)
                        {
                            inc[i][j].DoorEast = false;
                            inc[i+1][j].DoorWest = false;
                        } else
                        {
                            inc[i][j].DoorEast = true;
                            inc[i+1][j].DoorWest = true;
                        }
                    }
                }
                if (j > 0)
                {
                    if (inc[i][j].DoorSouth && !inc[i][j-1].DoorNorth || !inc[i][j].DoorSouth && inc[i][j-1].DoorNorth)
                    {
                        if (UnityEngine.Random.value < mismatchDoorCloseRate)
                        {
                            inc[i][j].DoorSouth = false;
                            inc[i][j-1].DoorNorth = false;
                        } else
                        {
                            inc[i][j].DoorSouth = true;
                            inc[i][j-1].DoorNorth = true;
                        }
                    }
                }
                if (j < inc[i].Length - 1)
                {
                    if (inc[i][j].DoorNorth && !inc[i][j+1].DoorSouth || !inc[i][j].DoorNorth && inc[i][j+1].DoorSouth)
                    {
                        if (UnityEngine.Random.value < mismatchDoorCloseRate)
                        {
                            inc[i][j].DoorNorth = false;
                            inc[i][j+1].DoorSouth = false;
                        } else
                        {
                            inc[i][j].DoorNorth = true;
                            inc[i][j+1].DoorSouth = true;
                        }
                    }
                }
            }
        }
        return inc;
    } // HandleConflictingDoors
 
    /* 
    Closes each door that would lead outside the outer edge of the dungeon. 
    */
    public Room[][] CloseOffEdgeDoors(Room[][] inc) // Only works on square-BASED dungeons
    {
        for (int j=0 ; j<inc[0].Length ; j++)
        { 
            inc[0][j].DoorWest = false;
            inc[inc.Length-1][j].DoorEast = false;
        }
        for (int i=0 ; i<inc.Length ; i++)
        {
            inc[i][0].DoorSouth = false;
            inc[i][inc[i].Length-1].DoorNorth = false;
        }
        return inc;
    } // CloseOffEdgeDoors

    /*
    Debug.Log stuff - prints the door status for each Room in the provided Room[][].
    */
    private void PrintRoomDoors(Room[][] inc)
    {
        for (int i=0 ; i<inc.Length ; i++)
        {
            for (int j=0 ; j<inc[i].Length ; j++)
            {
                Debug.Log(i + ", " + j + ", North: " + inc[i][j].DoorNorth + ", East: " + inc[i][j].DoorEast + ", South: " + inc[i][j].DoorSouth + ", West: " + inc[i][j].DoorWest);
            }
        }
    } // PrintRoomDoors

    /* 
    Converts the provided string[][] into a Room[][] (the dungeon is represented by this Room[][], each Room is a representation 
    of four doors, each with a true/false value for whether or not it exists). 
    */
    public Room[][] MainStringToRoom(string[][] iData)
    {
        Room[][] output = new Room[iData.Length][];
        for (int i=0 ; i<iData.Length ; i++)
        {
            output[i] = new Room[iData[i].Length];
            for (int j=0 ; j<iData[i].Length ; j++)
            {
                output[i][j] = new Room();
                // Translate each room + rotation into true/false
                int rot = 0;
                if (iData[i][j].IndexOf("Q") > -1)
                {
                    rot = int.Parse(iData[i][j].Substring(iData[i][j].IndexOf("Q")+1, 1));
                    while (rot > 3)
                    {
                        rot -= 4;
                    }
                }

                if (iData[i][j].Contains("R1"))
                {
                    output[i][j].DoorNorth = true;
                    output[i][j].DoorEast = true;
                    output[i][j].DoorSouth = true;
                    output[i][j].DoorWest = true;

                }
                if (iData[i][j].Contains("R2"))
                {
                    switch(rot)
                    {
                        case 0: 
                        output[i][j].DoorNorth = true;
                        output[i][j].DoorEast = false;
                        output[i][j].DoorSouth = false;
                        output[i][j].DoorWest = false;
                        break;
                        case 1:
                        output[i][j].DoorNorth = false;
                        output[i][j].DoorEast = true;
                        output[i][j].DoorSouth = false;
                        output[i][j].DoorWest = false;
                        break;
                        case 2:
                        output[i][j].DoorNorth = false;
                        output[i][j].DoorEast = false;
                        output[i][j].DoorSouth = true;
                        output[i][j].DoorWest = false;
                        break;
                        case 3:
                        output[i][j].DoorNorth = false;
                        output[i][j].DoorEast = false;
                        output[i][j].DoorSouth = false;
                        output[i][j].DoorWest = true;
                        break;
                    }
                }
                if (iData[i][j].Contains("R3"))
                {
                    switch(rot)
                    {
                        case 0: 
                        output[i][j].DoorNorth = false;
                        output[i][j].DoorEast = false;
                        output[i][j].DoorSouth = true;
                        output[i][j].DoorWest = true;
                        break;
                        case 1:
                        output[i][j].DoorNorth = true;
                        output[i][j].DoorEast = false;
                        output[i][j].DoorSouth = false;
                        output[i][j].DoorWest = true;
                        break;
                        case 2:
                        output[i][j].DoorNorth = true;
                        output[i][j].DoorEast = true;
                        output[i][j].DoorSouth = false;
                        output[i][j].DoorWest = false;
                        break;
                        case 3:
                        output[i][j].DoorNorth = false;
                        output[i][j].DoorEast = true;
                        output[i][j].DoorSouth = true;
                        output[i][j].DoorWest = false;
                        break;
                    }
                }
                if (iData[i][j].Contains("R4"))
                {
                    switch(rot)
                    {
                        case 0: 
                        output[i][j].DoorNorth = false;
                        output[i][j].DoorEast = true;
                        output[i][j].DoorSouth = false;
                        output[i][j].DoorWest = true;
                        break;
                        case 1:
                        output[i][j].DoorNorth = true;
                        output[i][j].DoorEast = false;
                        output[i][j].DoorSouth = true;
                        output[i][j].DoorWest = false;
                        break;
                        case 2:
                        output[i][j].DoorNorth = false;
                        output[i][j].DoorEast = true;
                        output[i][j].DoorSouth = false;
                        output[i][j].DoorWest = true;
                        break;
                        case 3:
                        output[i][j].DoorNorth = true;
                        output[i][j].DoorEast = false;
                        output[i][j].DoorSouth = true;
                        output[i][j].DoorWest = false;
                        break;
                    }
                }
                if (iData[i][j].Contains("R5"))
                {
                    switch(rot)
                    {
                        case 0: 
                        output[i][j].DoorNorth = false;
                        output[i][j].DoorEast = true;
                        output[i][j].DoorSouth = false;
                        output[i][j].DoorWest = true;
                        break;
                        case 1:
                        output[i][j].DoorNorth = true;
                        output[i][j].DoorEast = false;
                        output[i][j].DoorSouth = true;
                        output[i][j].DoorWest = false;
                        break;
                        case 2:
                        output[i][j].DoorNorth = false;
                        output[i][j].DoorEast = true;
                        output[i][j].DoorSouth = false;
                        output[i][j].DoorWest = true;
                        break;
                        case 3:
                        output[i][j].DoorNorth = true;
                        output[i][j].DoorEast = false;
                        output[i][j].DoorSouth = true;
                        output[i][j].DoorWest = false;
                        break;
                    }
                }
                if (iData[i][j].Contains("R6"))
                {
                    switch(rot)
                    {
                        case 0: 
                        output[i][j].DoorNorth = true;
                        output[i][j].DoorEast = true;
                        output[i][j].DoorSouth = true;
                        output[i][j].DoorWest = true;
                        break;
                        case 1:
                        output[i][j].DoorNorth = true;
                        output[i][j].DoorEast = true;
                        output[i][j].DoorSouth = true;
                        output[i][j].DoorWest = true;
                        break;
                        case 2:
                        output[i][j].DoorNorth = true;
                        output[i][j].DoorEast = true;
                        output[i][j].DoorSouth = true;
                        output[i][j].DoorWest = true;
                        break;
                        case 3:
                        output[i][j].DoorNorth = true;
                        output[i][j].DoorEast = true;
                        output[i][j].DoorSouth = true;
                        output[i][j].DoorWest = true;
                        break;
                    }
                }
                if (iData[i][j].Contains("R7"))
                {
                    switch(rot)
                    {
                        case 0: 
                        output[i][j].DoorNorth = false;
                        output[i][j].DoorEast = true;
                        output[i][j].DoorSouth = false;
                        output[i][j].DoorWest = true;
                        break;
                        case 1:
                        output[i][j].DoorNorth = true;
                        output[i][j].DoorEast = false;
                        output[i][j].DoorSouth = true;
                        output[i][j].DoorWest = false;
                        break;
                        case 2:
                        output[i][j].DoorNorth = false;
                        output[i][j].DoorEast = true;
                        output[i][j].DoorSouth = false;
                        output[i][j].DoorWest = true;
                        break;
                        case 3:
                        output[i][j].DoorNorth = true;
                        output[i][j].DoorEast = false;
                        output[i][j].DoorSouth = true;
                        output[i][j].DoorWest = false;
                        break;
                    }
                }
                if (iData[i][j].Contains("R8"))
                {
                    switch(rot)
                    {
                        case 0: 
                        output[i][j].DoorNorth = true;
                        output[i][j].DoorEast = true;
                        output[i][j].DoorSouth = false;
                        output[i][j].DoorWest = true;
                        break;
                        case 1:
                        output[i][j].DoorNorth = true;
                        output[i][j].DoorEast = true;
                        output[i][j].DoorSouth = true;
                        output[i][j].DoorWest = false;
                        break;
                        case 2:
                        output[i][j].DoorNorth = false;
                        output[i][j].DoorEast = true;
                        output[i][j].DoorSouth = true;
                        output[i][j].DoorWest = true;
                        break;
                        case 3:
                        output[i][j].DoorNorth = true;
                        output[i][j].DoorEast = false;
                        output[i][j].DoorSouth = true;
                        output[i][j].DoorWest = true;
                        break;
                    }
                }
            }
        }
        return output;
    } // MainStringToRoom

    /*
    Formats the file string into a string[][] form.
    */
    public string[][] GetDataString()
    {
        string temp = file.text.Replace("\r\n", "_");
        string[] content = temp.Split('_');
        string[][] output = new string[content.Length][];
         for (int i=0 ; i<content.Length; i++)
        {
            output[i] = content[i].Split(';');
        }
        return output;
    } // GetDataString

    private GameObject MainStringToGameObject(string[][] iData)
    { // Deprecated - used old NavMesh to calculate traversal rate. 
        rooms = new GameObject[iData.Length][];
        ClearRooms();

        for (int i=0 ; i<iData.Length; i++)
        {
            rooms[i] = new GameObject[iData[i].Length];
            for (int j=0 ; j<iData[i].Length; j++)
            {
                rooms[i][j] = StringToRoom(iData[i][j], i, j, iData.Length/2, iData[i].Length/2);
            }
        }
        UpdateNavMeshBake();
        spawnPoints = GetSpawnpoints();
        Debug.Log("Number of spawnpoints found: " + spawnPoints.Length);
        Debug.Log(spawnPoints[0].transform.position);
        connected = CheckIfConnectedToPlayer(connected);
        PrintConnections(connected);
        Debug.Log("Traversal rate: " + CalculateTraversalRate(connected));
        return this.gameObject;
    } // MainStringToGameObject

    /*
    Updates the NavMesh bake.
    Shocking.
    */
    private void UpdateNavMeshBake()
    {
        GameObject.FindGameObjectWithTag("Navmesh").GetComponent<NavMeshMasterScript>().UpdateNavMeshBake();  
    } // UpdateNavMeshBake

    private GameObject[] GetSpawnpoints()
    { // Deprecated - this system isn't used anymore
        return GameObject.FindGameObjectsWithTag("OBJSpawnpoint");
    } // GetSpawnPoints

    /*
    Debug.Log stuff - prints each bool in room layout
    */
    private void PrintConnections(bool[][] inc)
    {
        string printThis = "";
        Debug.Log(inc[0].Length + " is length of position 0");
        for (int i=inc.Length-1 ; i>=0 ; i--)
        {
            for (int j=0 ; j<inc[i].Length ; j++)
            {
                printThis += inc[j][i] + " ";
            }
            Debug.Log(printThis);
            printThis = "";
        }
    } // PrintConnections

    private bool[][] CheckIfConnectedToPlayer(bool[][] inc)
    { // Deprecated - this one used built-in NavMesh on generated rooms, the new one uses Room[][] representation
        int index = 0;
        int numTrue = 0;
        for (int i=0 ; i<inc.Length; i++)
        {
            for (int j=0 ; j<inc[i].Length; j++)
            {
                NavMesh.CalculatePath(spawnPoints[index].transform.position, player.transform.position, NavMesh.AllAreas, path);
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    inc[i][j] = false;
                } else 
                {
                    numTrue++;
                }
                index++;
            }
        }
        return inc;    
    } // CheckIfConnectedToPlayer

    /*
    Compares each room to see if it is connected to every other room, with variable tolerance. 
    */
    private bool[][] CheckIfConnected(bool[][] inc)
    {
        int index = 0;
        int numTrue = 0; // Needed because if it's a simple true/false operation, one unreachable room will flag every room as unreachable to that room, resulting in all falses
        for (int i=0 ; i<inc.Length; i++)
        {
            for (int j=0 ; j<inc[i].Length; j++)
            {
                int index2 = 0;
                for (int a=0; a<inc.Length; a++)
                {
                    for (int b=0 ; b<inc[i].Length; b++)
                    {
                        NavMesh.CalculatePath(spawnPoints[index].transform.position, spawnPoints[index2].transform.position, NavMesh.AllAreas, path);
                        if (path.status == NavMeshPathStatus.PathComplete)
                        {
                        } else 
                        {
                            numTrue++;
                        }
                        index2++;
                    }
                }
                inc[i][j] = (numTrue < pathPossibleTolerance); // to prevent one false room making all rooms false because they cant reach it
                numTrue = 0;
                index++;
            }
        }
        return inc;
    } // CheckIfConnected

    private void PlaceRooms()
    { // Deprecated - used for testing
        for (int i=0; i<10; i++)
        {
            for (int j=0 ; j<10; j++)
            {
                Vector3 pos = new Vector3((5-i)*sizeOfTile, 0, (5-j)*sizeOfTile);
                Quaternion rot = Quaternion.Euler(0, 90*rotations[i][j], 0);
                rooms[i][j] = Instantiate(rooms[i][j], this.gameObject.transform);
                rooms[i][j].transform.position = pos;
                rooms[i][j].transform.rotation = rot;
            }
        }
        Debug.Log("Rooms placed");
    } // PlaceRooms

    /*
    Performs a crossover between the two provided string[][] dungeons, then recalculates their enemy and pickup numbers and updates the string attached to it to store it.
    */
    public string[][] PerformCrossOver(string[][] inc1, string[][] inc2)
    {
        if (inc1.Length != inc2.Length) return inc1;
        string[][] output = new string[inc1.Length][];
        for (int i=0 ; i<output.Length ; i++)
        {
            if (inc1[i].Length != inc2[i].Length) return inc1;
            output[i] = new string[inc1[i].Length];
            for (int j=0 ; j<output[i].Length ; j++)
            {
                int offsetVal = (int)UnityEngine.Random.Range(-3f, 3f);
                if ((output[i].Length/2)+offsetVal > j)
                {
                    output[i][j] = inc1[i][j];
                } else
                {
                    output[i][j] = inc2[i][j];
                }
            }
        }
        output[0][0] = CalculateEnemyNumbers(output);
        output[0][0] = CalculatePickupNumbers(output);
        return output;
    } // PerformCrossover
 
    /*
    Adds the "Y" string to the 0/0 position of the incoming string after counting pickup statistics based on the values stored in the string.
    */
    public string CalculatePickupNumbers(string[][] inc)
    {
        string output = "Y?";
        if (inc[0][0].Contains("Y")) // remove existing points counts first
            {
                int indexQ1 = inc[0][0].IndexOf("Y");
                int indexQ2 = inc[0][0].LastIndexOf("?");
                if (indexQ1>-1 && indexQ2>-1)
                {
                    inc[0][0] = inc[0][0].Replace(inc[0][0].Substring(indexQ1, 1+indexQ2-indexQ1), "");
                }
            }
            int numPickupsTotal = 0;
            int numPickupPointsTotal = 0;
            int numPickupHealthTotal = 0;
            for (int i=0 ; i<inc.Length ; i++)
            {
                for (int j=0 ; j<inc[i].Length ; j++)
                {
                    int index1 = inc[i][j].IndexOf("T");
                    int index2 = inc[i][j].IndexOf("/");
                    if (index1 > -1 && index2 > -1)
                    {
                        int numPickups = int.Parse(inc[i][j].Substring(index1+1, index2-(index1+1)));
                        numPickupsTotal += numPickups;
                    }
                    index1 = index2;
                    index2 = inc[i][j].IndexOf("/", index1+1);
                    if (index1 > -1 && index2 > -1)
                    {
                        int pickupPoints = int.Parse(inc[i][j].Substring(index1+1, index2-(index1+1)));
                        numPickupPointsTotal += pickupPoints;
                    }
                    index1 = index2;
                    index2 = inc[i][j].IndexOf("/", index1+1);
                    if (index1 > -1 && index2 > -1)
                    {
                        int pickupHealth = int.Parse(inc[i][j].Substring(index1+1, index2-(index1+1)));
                        numPickupHealthTotal += pickupHealth;
                    }
                }
            }
            output += numPickupsTotal + "?" + numPickupPointsTotal + "?" + numPickupHealthTotal + "?";
            return inc[0][0] + output;
    } // CalculatePickupNumbers

    /*
    Adds the "Z" string to the 0/0 position of the incoming string after counting enemy statistics based on the values stored in the string.
    */
    public string CalculateEnemyNumbers(string[][] inc)
    {
        string output = "Z,";
        if (inc[0][0].Contains("Z")) // remove existing enemy counts first
            {
                int indexComma1 = inc[0][0].IndexOf("Z");
                int indexComma2 = inc[0][0].LastIndexOf(",");
                if (indexComma1>-1 && indexComma2>-1)
                {
                    inc[0][0] = inc[0][0].Replace(inc[0][0].Substring(indexComma1, 1+indexComma2-indexComma1), "");
                }
            }
            int numEnemiesTotal = 0;
            int numEnemyHealthTotal = 0;
            int numEnemyDamageTotal = 0;
            for (int i=0 ; i<inc.Length ; i++)
            {
                for (int j=0 ; j<inc[i].Length ; j++)
                {
                    int index1 = inc[i][j].IndexOf("E");
                    int index2 = inc[i][j].IndexOf(".");
                    if (index1 > -1 && index2 > -1)
                    {
                        int numEnemies = int.Parse(inc[i][j].Substring(index1+1, index2-(index1+1)));
                        numEnemiesTotal += numEnemies;

                    }
                    index1 = index2;
                    index2 = inc[i][j].IndexOf(".", index1+1);
                    if (index1 > -1 && index2 > -1)
                    {
                        int enemyHealth = int.Parse(inc[i][j].Substring(index1+1, index2-(index1+1)));
                        numEnemyHealthTotal += enemyHealth;
                    }
                    index1 = index2;
                    index2 = inc[i][j].IndexOf(".", index1+1);
                    if (index1 > -1 && index2 > -1)
                    {
                        int enemyDamage = int.Parse(inc[i][j].Substring(index1+1, index2-(index1+1)));
                        numEnemyDamageTotal += enemyDamage;
                    }
                }
            }
            output += numEnemiesTotal + "," + numEnemyHealthTotal + "," + numEnemyDamageTotal + ",";
            return inc[0][0] + output;
    } // CalculateEnemyNumbers

    /*
    Performs genetic algorithm mutation, following the pre-set rate. It also keeps track of the number of treasures
    and enemies in the dungeon, as well as each of their values like health or points or enemy damage, for later use.
    */
    public string[][] PerformMutation(string[][] inc)
    { // this is the one used most of the time
        string[][] output = inc;
        int numEnemies = 0;
        int enemyDamages = 0;
        int enemyHealths = 0;
        int numPickups = 0;
        int pickupPoints = 0;
        int pickupHealths = 0;
        if (output[0][0].Contains("Z")) // remove existing enemy counts first
            {
                int indexComma1 = output[0][0].IndexOf("Z");
                int indexComma2 = output[0][0].LastIndexOf(",");
                if (indexComma1>-1 && indexComma2>-1)
                {
                    output[0][0] = output[0][0].Replace(output[0][0].Substring(indexComma1, 1+indexComma2-indexComma1), "");
                }
            }
        for (int i=0 ; i<output.Length ; i++)
        {
            for (int j=0 ; j<output[i].Length ; j++)
            {
                if (UnityEngine.Random.value * 100 < mutationRate)
                {
                    switch ((int)(UnityEngine.Random.value*8))
                    {
                        case 0:
                        output[i][j] = output[i][j].Replace("R" + output[i][j].Substring(output[i][j].IndexOf("R")+1, 1), "R1");
                        if (output[i][j].Contains("Q"))
                        {
                            output[i][j] = output[i][j].Replace("Q" + output[i][j].Substring(output[i][j].IndexOf("Q")+1, 1), "Q" + (int)(UnityEngine.Random.value*5));
                        } else 
                        {
                            output[i][j] = output[i][j].Insert(0, "Q" + (int)(UnityEngine.Random.value*5));
                        }
                        break;

                        case 1:
                        output[i][j] = output[i][j].Replace("R" + output[i][j].Substring(output[i][j].IndexOf("R")+1, 1), "R2");
                        if (output[i][j].Contains("Q"))
                        {
                            output[i][j] = output[i][j].Replace("Q" + output[i][j].Substring(output[i][j].IndexOf("Q")+1, 1), "Q" + (int)(UnityEngine.Random.value*5));
                        } else 
                        {
                            output[i][j] = output[i][j].Insert(0, "Q" + (int)(UnityEngine.Random.value*5));
                        }
                        break;

                        case 2:
                        output[i][j] = output[i][j].Replace("R" + output[i][j].Substring(output[i][j].IndexOf("R")+1, 1), "R3");
                        if (output[i][j].Contains("Q"))
                        {
                            output[i][j] = output[i][j].Replace("Q" + output[i][j].Substring(output[i][j].IndexOf("Q")+1, 1), "Q" + (int)(UnityEngine.Random.value*5));
                        } else 
                        {
                            output[i][j] = output[i][j].Insert(0, "Q" + (int)(UnityEngine.Random.value*5));
                        }
                        break;

                        case 3:
                        output[i][j] = output[i][j].Replace("R" + output[i][j].Substring(output[i][j].IndexOf("R")+1, 1), "R4");
                        if (output[i][j].Contains("Q"))
                        {
                            output[i][j] = output[i][j].Replace("Q" + output[i][j].Substring(output[i][j].IndexOf("Q")+1, 1), "Q" + (int)(UnityEngine.Random.value*5));
                        } else 
                        {
                            output[i][j] = output[i][j].Insert(0, "Q" + (int)(UnityEngine.Random.value*5));
                        }
                        break;

                        case 4:
                        output[i][j] = output[i][j].Replace("R" + output[i][j].Substring(output[i][j].IndexOf("R")+1, 1), "R5");
                        if (output[i][j].Contains("Q"))
                        {
                            output[i][j] = output[i][j].Replace("Q" + output[i][j].Substring(output[i][j].IndexOf("Q")+1, 1), "Q" + (int)(UnityEngine.Random.value*5));
                        } else 
                        {
                            output[i][j] = output[i][j].Insert(0, "Q" + (int)(UnityEngine.Random.value*5));
                        }
                        break;

                        case 5:
                        output[i][j] = output[i][j].Replace("R" + output[i][j].Substring(output[i][j].IndexOf("R")+1, 1), "R6");
                        if (output[i][j].Contains("Q"))
                        {
                            output[i][j] = output[i][j].Replace("Q" + output[i][j].Substring(output[i][j].IndexOf("Q")+1, 1), "Q" + (int)(UnityEngine.Random.value*5));
                        } else 
                        {
                            output[i][j] = output[i][j].Insert(0, "Q" + (int)(UnityEngine.Random.value*5));
                        }
                        break;

                        case 6:
                        output[i][j] = output[i][j].Replace("R" + output[i][j].Substring(output[i][j].IndexOf("R")+1, 1), "R7");
                        if (output[i][j].Contains("Q"))
                        {
                            output[i][j] = output[i][j].Replace("Q" + output[i][j].Substring(output[i][j].IndexOf("Q")+1, 1), "Q" + (int)(UnityEngine.Random.value*5));
                        } else 
                        {
                            output[i][j] = output[i][j].Insert(0, "Q" + (int)(UnityEngine.Random.value*5));
                        }
                        break;

                        case 7:
                        output[i][j] = output[i][j].Replace("R" + output[i][j].Substring(output[i][j].IndexOf("R")+1, 1), "R8");
                        if (output[i][j].Contains("Q"))
                        {
                            output[i][j] = output[i][j].Replace("Q" + output[i][j].Substring(output[i][j].IndexOf("Q")+1, 1), "Q" + (int)(UnityEngine.Random.value*5));
                        } else 
                        {
                            output[i][j] = output[i][j].Insert(0, "Q" + (int)(UnityEngine.Random.value*5));
                        }
                        break;

                        case 8:

                        break;
                    }

                    int val = (int)(UnityEngine.Random.value*8);
                    {
                        if (output[i][j].Contains("P")) val = 0;
                        if (output[i][j].Contains("E")) // remove existing enemy data first
                            {
                                output[i][j] = output[i][j].Replace("E" + output[i][j].Substring(output[i][j].IndexOf("E")+1, 1), "");
                                int indexPeriod1 = output[i][j].IndexOf(".");
                                int indexPeriod2 = output[i][j].LastIndexOf(".");
                                if (indexPeriod1>-1 && indexPeriod2>-1)
                                {
                                    output[i][j] = output[i][j].Replace(output[i][j].Substring(indexPeriod1, 1+indexPeriod2-indexPeriod1), "");
                                }
                            }
                        if (val == 0)
                        {
                        } else
                        {
                            string toAdd = "E" + val + "."; // EX.Y.Z., X num Y health Z damage
                            for (int z=0 ; z<val ; z++)
                            {
                                numEnemies++;
                                int enemyHealthToAdd = (int)(enemyMinimumHealth+UnityEngine.Random.value*(enemyMaximumHealth-enemyMinimumHealth));
                                int enemyDamageToAdd = (int)(enemyMinimumDamage+UnityEngine.Random.value*(enemyMaximumDamage-enemyMinimumDamage));
                                toAdd += enemyHealthToAdd + "." + enemyDamageToAdd + ".";   
                                enemyDamages += enemyDamageToAdd;
                                enemyHealths += enemyHealthToAdd;   
                            }
                            output[i][j] += toAdd;
                        }
                    }

                    val = (int)(UnityEngine.Random.value*8);
                    {
                        if (output[i][j].Contains("P")) val = 0;
                        if (output[i][j].Contains("T")) // remove existing treasure data first
                            {
                                output[i][j] = output[i][j].Replace("T" + output[i][j].Substring(output[i][j].IndexOf("T")+1, 1), "");
                                int indexSlash1 = output[i][j].IndexOf("/");
                                int indexSlash2 = output[i][j].LastIndexOf("/");
                                if (indexSlash1>-1 && indexSlash2>-1)
                                {
                                    output[i][j] = output[i][j].Replace(output[i][j].Substring(indexSlash1, 1+indexSlash2-indexSlash1), "");
                                }
                            }
                        if (val == 0)
                        {
                
                        } else
                        {
                       
                            string toAdd = "T" + val + "/"; // TX.Y.Z., X num Y points Z health
                            for (int z=0 ; z<val ; z++)
                            {
                                numPickups++;
                                int pickupHealthToAdd = (int)(pickupMinimumHealth+UnityEngine.Random.value*(pickupMaximumHealth-pickupMinimumHealth));
                                int pickupPointsToAdd = (int)(pickupMinimumPoints+UnityEngine.Random.value*(pickupMaximumPoints-pickupMinimumPoints));
                                toAdd += pickupPointsToAdd + "/" + pickupHealthToAdd + "/";   
                                pickupHealths += pickupHealthToAdd;
                                pickupPoints += pickupPointsToAdd;   
                            }
                            output[i][j] += toAdd;
                        }
                    }
                }
            }
        }
       output[0][0] += "Z," + numEnemies + "," + enemyHealths + "," + enemyDamages + ","; // add a count for enemy numbers to the 0,0 room
       output[0][0] += "Y?" + numPickups + "?" + pickupPoints + "?" + pickupHealths + "?"; // add a count for pickup numbers to the 0,0 room
        return output;
    } // PerformMutation

    /*
    Performs genetic algorithm mutation, following the provided rate instead of the pre-set one. This is used primarily for when it wants 100% rate,
    as that will result in a completely random dungeon, which the first generation is comprised of. It also keeps track of the number of treasures
    and enemies in the dungeon, as well as each of their values like health or points or enemy damage, for later use.
    */
    public string[][] PerformMutation(string[][] inc, float rate)
    { // this is the one used for 100% rate in creating entirely new dungeons
        string[][] output = inc;
        int numEnemies = 0;
        int enemyDamages = 0;
        int enemyHealths = 0;
        int numPickups = 0;
        int pickupPoints = 0;
        int pickupHealths = 0;
        if (output[0][0].Contains("Z")) // remove existing enemy counts first
            {
                int indexComma1 = output[0][0].IndexOf("Z");
                int indexComma2 = output[0][0].LastIndexOf(",");
                if (indexComma1>-1 && indexComma2>-1)
                {
                    output[0][0] = output[0][0].Replace(output[0][0].Substring(indexComma1, 1+indexComma2-indexComma1), "");
                }
            }
        for (int i=0 ; i<output.Length ; i++)
        {
            for (int j=0 ; j<output[i].Length ; j++)
            {
                if (UnityEngine.Random.value * 100 < rate)
                {
                    switch ((int)(UnityEngine.Random.value*8))
                    {
                        case 0:
                        output[i][j] = output[i][j].Replace("R" + output[i][j].Substring(output[i][j].IndexOf("R")+1, 1), "R1");
                        if (output[i][j].Contains("Q"))
                        {
                            output[i][j] = output[i][j].Replace("Q" + output[i][j].Substring(output[i][j].IndexOf("Q")+1, 1), "Q" + (int)(UnityEngine.Random.value*5));
                        } else 
                        {
                            output[i][j] = output[i][j].Insert(0, "Q" + (int)(UnityEngine.Random.value*5));
                        }
                        break;

                        case 1:
                        output[i][j] = output[i][j].Replace("R" + output[i][j].Substring(output[i][j].IndexOf("R")+1, 1), "R2");
                        if (output[i][j].Contains("Q"))
                        {
                            output[i][j] = output[i][j].Replace("Q" + output[i][j].Substring(output[i][j].IndexOf("Q")+1, 1), "Q" + (int)(UnityEngine.Random.value*5));
                        } else 
                        {
                            output[i][j] = output[i][j].Insert(0, "Q" + (int)(UnityEngine.Random.value*5));
                        }
                        break;

                        case 2:
                        output[i][j] = output[i][j].Replace("R" + output[i][j].Substring(output[i][j].IndexOf("R")+1, 1), "R3");
                        if (output[i][j].Contains("Q"))
                        {
                            output[i][j] = output[i][j].Replace("Q" + output[i][j].Substring(output[i][j].IndexOf("Q")+1, 1), "Q" + (int)(UnityEngine.Random.value*5));
                        } else 
                        {
                            output[i][j] = output[i][j].Insert(0, "Q" + (int)(UnityEngine.Random.value*5));
                        }
                        break;

                        case 3:
                        output[i][j] = output[i][j].Replace("R" + output[i][j].Substring(output[i][j].IndexOf("R")+1, 1), "R4");
                        if (output[i][j].Contains("Q"))
                        {
                            output[i][j] = output[i][j].Replace("Q" + output[i][j].Substring(output[i][j].IndexOf("Q")+1, 1), "Q" + (int)(UnityEngine.Random.value*5));
                        } else 
                        {
                            output[i][j] = output[i][j].Insert(0, "Q" + (int)(UnityEngine.Random.value*5));
                        }
                        break;

                        case 4:
                        output[i][j] = output[i][j].Replace("R" + output[i][j].Substring(output[i][j].IndexOf("R")+1, 1), "R5");
                        if (output[i][j].Contains("Q"))
                        {
                            output[i][j] = output[i][j].Replace("Q" + output[i][j].Substring(output[i][j].IndexOf("Q")+1, 1), "Q" + (int)(UnityEngine.Random.value*5));
                        } else 
                        {
                            output[i][j] = output[i][j].Insert(0, "Q" + (int)(UnityEngine.Random.value*5));
                        }
                        break;

                        case 5:
                        output[i][j] = output[i][j].Replace("R" + output[i][j].Substring(output[i][j].IndexOf("R")+1, 1), "R6");
                        if (output[i][j].Contains("Q"))
                        {
                            output[i][j] = output[i][j].Replace("Q" + output[i][j].Substring(output[i][j].IndexOf("Q")+1, 1), "Q" + (int)(UnityEngine.Random.value*5));
                        } else 
                        {
                            output[i][j] = output[i][j].Insert(0, "Q" + (int)(UnityEngine.Random.value*5));
                        }
                        break;

                        case 6:
                        output[i][j] = output[i][j].Replace("R" + output[i][j].Substring(output[i][j].IndexOf("R")+1, 1), "R7");
                        if (output[i][j].Contains("Q"))
                        {
                            output[i][j] = output[i][j].Replace("Q" + output[i][j].Substring(output[i][j].IndexOf("Q")+1, 1), "Q" + (int)(UnityEngine.Random.value*5));
                        } else 
                        {
                            output[i][j] = output[i][j].Insert(0, "Q" + (int)(UnityEngine.Random.value*5));
                        }
                        break;

                        case 7:
                        output[i][j] = output[i][j].Replace("R" + output[i][j].Substring(output[i][j].IndexOf("R")+1, 1), "R8");
                        if (output[i][j].Contains("Q"))
                        {
                            output[i][j] = output[i][j].Replace("Q" + output[i][j].Substring(output[i][j].IndexOf("Q")+1, 1), "Q" + (int)(UnityEngine.Random.value*5));
                        } else 
                        {
                            output[i][j] = output[i][j].Insert(0, "Q" + (int)(UnityEngine.Random.value*5));
                        }
                        break;

                        case 8:

                        break;
                    }

                    int val = (int)(UnityEngine.Random.value*8);
                    {
                        if (output[i][j].Contains("P")) val = 0;
                        if (output[i][j].Contains("E")) // remove existing enemy data first
                            {
                                output[i][j] = output[i][j].Replace("E" + output[i][j].Substring(output[i][j].IndexOf("E")+1, 1), "");
                                int indexPeriod1 = output[i][j].IndexOf(".");
                                int indexPeriod2 = output[i][j].LastIndexOf(".");
                                if (indexPeriod1>-1 && indexPeriod2>-1)
                                {
                                    output[i][j] = output[i][j].Replace(output[i][j].Substring(indexPeriod1, 1+indexPeriod2-indexPeriod1), "");
                                }
                            }
                        if (val == 0)
                        {
                        } else
                        {
                            string toAdd = "E" + val + "."; // EX.Y.Z., X num Y health Z damage
                            for (int z=0 ; z<val ; z++)
                            {
                                numEnemies++;
                                int enemyHealthToAdd = (int)(enemyMinimumHealth+UnityEngine.Random.value*(enemyMaximumHealth-enemyMinimumHealth));
                                int enemyDamageToAdd = (int)(enemyMinimumDamage+UnityEngine.Random.value*(enemyMaximumDamage-enemyMinimumDamage));
                                toAdd += enemyHealthToAdd + "." + enemyDamageToAdd + ".";   
                                enemyDamages += enemyDamageToAdd;
                                enemyHealths += enemyHealthToAdd;   
                            }
                            output[i][j] += toAdd;
                        }
                    }

                    val = (int)(UnityEngine.Random.value*8);
                    {
                        if (output[i][j].Contains("P")) val = 0; // make it so treasure doesnt spawn in player start room
                        if (output[i][j].Contains("T")) // remove existing treasure data first
                            {
                                output[i][j] = output[i][j].Replace("T" + output[i][j].Substring(output[i][j].IndexOf("T")+1, 1), "");
                                int indexSlash1 = output[i][j].IndexOf("/");
                                int indexSlash2 = output[i][j].LastIndexOf("/");
                                if (indexSlash1>-1 && indexSlash2>-1)
                                {
                                    output[i][j] = output[i][j].Replace(output[i][j].Substring(indexSlash1, 1+indexSlash2-indexSlash1), "");
                                }
                            }
                        if (val == 0)
                        {
                
                        } else
                        {
                       
                            string toAdd = "T" + val + "/"; // TX.Y.Z., X num Y points Z health
                            for (int z=0 ; z<val ; z++)
                            {
                                numPickups++;
                                int pickupHealthToAdd = (int)(pickupMinimumHealth+UnityEngine.Random.value*(pickupMaximumHealth-pickupMinimumHealth));
                                int pickupPointsToAdd = (int)(pickupMinimumPoints+UnityEngine.Random.value*(pickupMaximumPoints-pickupMinimumPoints));
                                toAdd += pickupPointsToAdd + "/" + pickupHealthToAdd + "/";   
                                pickupHealths += pickupHealthToAdd;
                                pickupPoints += pickupPointsToAdd;   
                            }
                            output[i][j] += toAdd;
                        }
                    }
                }
            }
        }
        output[0][0] += "Z," + numEnemies + "," + enemyHealths + "," + enemyDamages + ","; // add a count for enemy numbers to the 0,0 room
        output[0][0] += "Y?" + numPickups + "?" + pickupPoints + "?" + pickupHealths + "?"; // add a count for pickup numbers to the 0,0 room
        return output;
    } // PerformMutation

    /*
    Inserts the exit key code into the string representing the room furthest away from the spawnpoint, according to the int[][] provided.
    */
    private string[][] PlaceExit(string[][] inc, int[][] incInt)
    {
        int iPos = 0;
        int jPos = 0;
        int maxDist = -1;
        for (int i=0 ; i<incInt.Length ; i++)
        {
            for (int j=0 ; j<incInt[i].Length ; j++)
            {
                if (incInt[i][j] > maxDist && incInt[i][j] < int.MaxValue)
                {
                    iPos = i;
                    jPos = j;
                    maxDist = incInt[i][j];
                }
            }
        }
        inc[iPos][jPos] += "W";
        return inc;
    } // PlaceExit

    /*
    Debug.Log stuff - reads the provided string[][] values.
    */
    private void ReadDoubleStringArray(string[][] inc)
    {
        for (int i=0 ; i<inc.Length ; i++)
        {
            for (int j=0 ; j<inc[i].Length ; j++)
            {
                Debug.Log(i + ", " + j + ", " + inc[i][j]);
            }
        }
    } // ReadDoubleStringArray

    /*
    Debug.Log stuff - reads the provided int[][] values.
    */
    private void ReadDoubleIntArray(int[][] inc)
    {
        for (int i=inc.Length-1 ; i>=0 ; i--)
        {
            string readMe = "";
            for (int j=0 ; j<inc[i].Length ; j++)
            {
                readMe += (inc[j][i] + " ");
            }
            Debug.Log(readMe);
        }
    } // ReadDoubleIntArray

    /*
    Returns how many rooms in the provided bool[][] are reachable by the player. Should probably return an int instead of a float, it was a float initially to allow for decimal subtraction
    as a more refined fitness function than IsFullyTraversible but that got removed.
    */
    public float CalculateTraversalRate(bool[][] inc)
    {
        float output = 0.0f;
        for (int i=0 ; i<inc.Length ; i++)
        {
            for (int j=0 ; j<inc[i].Length ; j++)
            {
                if (inc[i][j]) 
                {
                    output += 1f;
                } else
                {
                }
            }
        }
        return output;
    } // CalculateTraversalRate

    /*
    Returns how many rooms in the provided int[][] are reachable by the player. Should probably return an int instead of a float, it was a float initially to allow for decimal subtraction
    as a more refined fitness function than IsFullyTraversible but that got removed. Also it's the exact same as the previous method, only accepts an int instead.
    */
    public float CalculateTraversalRate(int[][] inc)
    {
        float output = 0.0f;
        for (int i=0 ; i<inc.Length ; i++)
        {
            for (int j=0 ; j<inc[i].Length ; j++)
            {
                if (inc[i][j] < int.MaxValue) 
                {
                    output += 1f;
                } else
                {
                }
            }
        }
        return output;
    } // CalculateTraversalRate

    /*
    Returns true if every value in bool[][] inc is true, false otherwise. Initially used as part of a rudimentary fitness algorithm to see if the dungeon was 100% traversible.
    */
    private bool IsFullyTraversible(bool[][] inc)
    {
        for (int i=0 ; i<inc.Length ; i++)
        {
            for (int j=0; j<inc[i].Length ; j++)
            {
                if (!inc[i][j]) return false;
            }
        }
        return true;
    } // IsFullyTraversible

    private void PlaceEntities()
    { // Deprecated - used for test cases
        Debug.Log("Placing entities...");
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("OBJSpawnpoint");
        int maxVal = nothingWeight + pickupWeight + enemyWeight;
        pickupWeight += nothingWeight;
        enemyWeight += pickupWeight;
        for (int i=0; i<spawnPoints.Length; i++)
        {
            float x = UnityEngine.Random.value * maxVal;
            if (x < nothingWeight)
            {
            } else if (x < pickupWeight)
            {
                GameObject tempOBJ = Instantiate(pickup, spawnPoints[i].transform);
                Vector3 tempPos = new Vector3(tempOBJ.transform.position.x, tempOBJ.transform.position.y + 1.5f, tempOBJ.transform.position.z);
                tempOBJ.transform.position = tempPos;
            } else if (x < enemyWeight)
            {
                Instantiate(enemy, spawnPoints[i].transform);
            }
        }
    } // PlaceEntities

    /*  
    Removes all currently placed rooms, treasures, enemies, and the exit to make way for a new dungeon.
    */
    public void ClearRooms()
    {
        int numRooms = this.gameObject.transform.childCount; // for however many children this object has, delete that many objects 
        for (int i=0; i<numRooms ; i++)
        {
            Destroy(this.gameObject.transform.GetChild(i).gameObject);
        }
    } // ClearRooms

    /*
    Returns the GameObject representing the string provided, filled with enemies, the player, the exit, and treasures if specified.
    */
    public GameObject StringToRoom(string inc, int i, int j, float offsetDistI, float offsetDistJ)
    {
        GameObject o = room9;
        if (inc.Contains("R1"))
        {
            o = room1;
        }
        if (inc.Contains("R2"))
        {
            o = room2;
        }
        if (inc.Contains("R3"))
        {
            o = room3;
        }
        if (inc.Contains("R4"))
        {
            o = room4;
        }
        if (inc.Contains("R5"))
        {
            o = room5;
        }
        if (inc.Contains("R6"))
        {
            o = room6;
        }
        if (inc.Contains("R7"))
        {
            o = room7;
        }
        if (inc.Contains("R8"))
        {
            o = room8;
        }
        Quaternion rot = Quaternion.Euler(0, 0, 0);
        if (inc.IndexOf("Q") > -1)
        {
            rot = Quaternion.Euler(0, 90*int.Parse(inc.Substring(inc.IndexOf("Q")+1, 1)), 0);
        }
        GameObject outputOBJ = Instantiate(o, this.gameObject.transform);      
        outputOBJ.transform.rotation = rot;
        outputOBJ.transform.position = new Vector3((i-offsetDistI)*sizeOfTile, 0, (j-offsetDistJ)*sizeOfTile); // <------------- HERE IS WHERE ROOM IS MADE

        if (!inc.Contains("R9"))
        { // no point in spawning things in room 9, save space

            if (!inc.Contains("W") && inc.IndexOf("E") > -1) // don't spawn enemies in the exit room
            {
                int numEnemies = int.Parse(inc.Substring(inc.IndexOf("E")+1, 1));
                int index = 0;
                int healthval = 100;
                int damageval = 6;
                if (numEnemies > 0) 
                {
                    string[] values = inc.Split('.');
                    for (int z=1 ; z<=numEnemies*2; z++)
                    {
                        if (values.Length > z+1)
                        {
                            healthval = int.Parse(values[z]);
                            z++;
                            damageval = int.Parse(values[z]);
                        }
                        GameObject spawnedEnemy = Instantiate(enemy, outputOBJ.transform.Find("OBJSpawnpoint"));
                        spawnedEnemy.transform.position += Vector3.up * 1.2f;
                        spawnedEnemy.transform.position += Vector3.right * 2*(0.5f-UnityEngine.Random.value);
                        spawnedEnemy.transform.position += Vector3.forward * 2*(0.5f-UnityEngine.Random.value);
                        spawnedEnemy.GetComponent<EnemyScript>().SetHealthAndDamage(healthval, damageval);
                        spawnedEnemy.GetComponent<EnemyScript>().SetPlayer(player);
                        spawnedEnemy.GetComponent<EnemyScript>().HasGun(UnityEngine.Random.value < enemyGunRate);
                    }
                }
            }

            if (!inc.Contains("W") && inc.IndexOf("T") > -1) // don't spawn treasures in the exit room either
            {
                int numTreasures = int.Parse(inc.Substring(inc.IndexOf("T")+1, 1));
                int index = 0;
                int healthVal = 20;
                int pointsVal = 100;
                if (numTreasures > 0) 
                {
                    string[] values = inc.Split('/');
                    for (int z=1 ; z<=numTreasures*2; z++)
                    {
                        if (values.Length > z+1)
                        {
                            pointsVal = int.Parse(values[z]);
                            z++;
                            healthVal = int.Parse(values[z]);

                        }

                        GameObject spawnedTreasure = Instantiate(pickup, outputOBJ.transform.Find("OBJSpawnpoint"));
                        spawnedTreasure.transform.position += Vector3.up * 1.5f;
                        spawnedTreasure.transform.position += Vector3.right * 2*(0.5f-UnityEngine.Random.value);
                        spawnedTreasure.transform.position += Vector3.forward * 2*(0.5f-UnityEngine.Random.value);
                        spawnedTreasure.GetComponent<PickupScript>().SetPointsAndHealth(pointsVal, healthVal);
                    }
                }
            }
        }
            if (inc.IndexOf("P") > -1) // CAN spawn the player in room 9, if no other rooms exist
            {
                player.transform.position = outputOBJ.gameObject.transform.position + Vector3.up * 3;
            }

            if (inc.IndexOf("W") > -1) // if the player can spawn in room 9, the exit must also spawn in room 9
            {
                GameObject exitPoint = Instantiate(exitPrefab, outputOBJ.transform.Find("OBJSpawnpoint"));
                if (inc.Contains("R9")) // if the player and exit are in room 9, move the exit so the player doesn't spawn inside it
                {
                    exitPoint.transform.position += Vector3.forward * 10;
                }
            }
      return outputOBJ;
    } // StringToRoom
}
