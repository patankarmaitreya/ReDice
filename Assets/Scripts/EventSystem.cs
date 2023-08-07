using System.Collections;
using System.Collections.Generic;
using AssociativeFiles.Custom;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;

public class EventSystem : MonoBehaviour
{
    private static EventSystem instance;
    internal GameObject Player;

    //SpawnAssets           #Start

    public List<GameObject> OrbPrefab;

    private List<GameObject> spawnableOrbPrefab;

    public GameObject Obstacles;

    public GameObject Mines;

    //SpawnAssets           #End

    //Spawn Rules           #Start

    public int maxOrbsOnField;
    [SerializeField]
    private Transform orbHolder;

    public int maxObstacleOnField;
    [SerializeField]
    private Transform obstacleHolder;

    public int maxMinesOnField;
    [SerializeField]
    private Transform mineHolder;

    private float scaleOfDice;
    private float scaleOfPlatform;

    public int gridX;
    public int gridZ;

    //Platform Offset from the center
    public int offsetX;
    public int offsetZ;

    public bool[,] Grid;

    //RNG
    private List<WeightedRNG> weightedRNG;

    internal WeightedRNG probOrb_1;
    internal WeightedRNG probOrb_2;
    internal WeightedRNG probOrb_3;
    internal WeightedRNG probOrb_4;

    //Modify this if we change platform size
    private int[] centerCell = new int[2] { 7, 7 };

    internal bool resetFlag;
    internal bool pauseSpawning=false;

    //Spawn Rules           #End

    //Timers                #Start

    [Tooltip("Timer that signals when everything resets")]
    public float ResetTimer;
    [Tooltip("Time after which everything resets")]
    public float ResetTime;

    //Timers                #End

    [SerializeField]
    private GameObject diceRoll;

    public GameObject RollOfTheDice;
    private bool RollGiven = false;

    private PlayerController playerController;
    internal bool RollAvailable=false;
    public static EventSystem GetInstance()
    {
        return instance;
    }

    public EventSystem Init(GameObject player)
    {
        Player = player;
        return this;
    }

    private void Awake()
    {
        instance = this;

        spawnableOrbPrefab = new List<GameObject>();
        weightedRNG = new List<WeightedRNG>();

        probOrb_1 = new WeightedRNG();
        probOrb_2 = new WeightedRNG();
        probOrb_3 = new WeightedRNG();
        probOrb_4 = new WeightedRNG();

        probOrb_1.SetWeightedRNG(0, 0, 40);
        probOrb_2.SetWeightedRNG(1, 1, 30);
        probOrb_3.SetWeightedRNG(2, 2, 20);
        probOrb_4.SetWeightedRNG(3, 3, 10);

        weightedRNG.Add(probOrb_1);
        weightedRNG.Add(probOrb_2);
        weightedRNG.Add(probOrb_3);
        weightedRNG.Add(probOrb_4);
    }
    private void Start()
    {
        playerController = Player.GetComponent<PlayerController>();
        scaleOfDice = Player.transform.localScale.x;
        scaleOfPlatform = GameObject.FindGameObjectWithTag("Platform").transform.localScale.x;

        gridX = (int)(scaleOfPlatform / scaleOfDice);
        gridZ = gridX;

        Grid = new bool[gridX, gridZ];

        for (int i = 0; i < gridX; i++)
        {
            for (int j = 0; j < gridZ; j++)
            {
                Grid[i, j] = false;
            }
        }

        playerController.playerOccupiedPos[0, 0] = 7;
        playerController.playerOccupiedPos[0, 1] = 7;

        Grid[7, 7] = true;
        setSurroundingGrid();

        RollOfTheDice.SetActive(false);
        //To Roll the dice once game starts
        RollAvailable = true;
        diceRoll.GetComponent<FollowCamera>().Camera_performed();
    }
    private void Update()
    {
        ResetTimer += Time.deltaTime;
        //Show time on HUD using the converted time

        //Modify speed as per the time passed
        DecideCurrentSpeed();
        //Modify spawnable orbs as per the time passed
        DecideSpawnableOrbs();

        if (spawnEventRunning)
        {
            return;
        }
        if(RollAvailable)
        {
            RollOfTheDice.SetActive(true);
        }
        else
        {
            RollOfTheDice.SetActive(false);
        }
        //This is to constantly spawn more orbs incase of orb is consumed       #Start

        GameObject[] orbs = GameObject.FindGameObjectsWithTag("Orb");
        if (orbs.Length < maxOrbsOnField && !spawning && !pauseSpawning)
        {
            StartCoroutine(SpawnScoreOrb());
        }

        //This is to constantly spawn more orbs incase of orb is consumed      #End

        if (ResetTimer >= ResetTime || resetFlag)
        {
            ResetTimer = 0;
            RollGiven = false;
            resetFlag = false;
            StartCoroutine(SpawnEvent());
        }

        if (((ResetTime - ResetTimer) / ResetTime) * 100 <= 50f && !RollGiven)
        {
            playerController.currentSpeed = playerController.rollSpeeds[0];
            RollAvailable = true;
            RollGiven = true;
        }
    }

    private bool spawning = false;
    private IEnumerator SpawnScoreOrb(bool checkCondition = false)
    {
        int spawnOrbOrNot = Random.Range(0, 2);
        if (spawnOrbOrNot == 1 || checkCondition)
        {
            spawning = true;
        Recheck:
            int randomCellX = Random.Range(0, gridX);
            int randomCellZ = Random.Range(0, gridZ);

        RecheckSpawanableOrb:
            int orbIns = CalcWeightedRNG.GetRandomValue(weightedRNG);

            if (spawnableOrbPrefab.Count == 3 && orbIns == 3)
            {
                goto RecheckSpawanableOrb;
            }
            else if (spawnableOrbPrefab.Count == 2 && (orbIns == 2 || orbIns == 3))
            {
                goto RecheckSpawanableOrb;
            }
            else if (spawnableOrbPrefab.Count == 1)
            {
                orbIns = 0;
            }

            if (!Grid[randomCellX, randomCellZ])
            {
                Grid[randomCellX, randomCellZ] = true;
                Vector2 loc;
                calculateWorldPosXZ(randomCellX, randomCellZ, out loc);
                Instantiate(spawnableOrbPrefab[orbIns], new Vector3(loc.x, 15, loc.y), Quaternion.identity, orbHolder);
            }
            else
            {
                goto Recheck;
            }
        }
        yield return new WaitForSeconds(1f);
        spawning = false;
    }

    private void SpawnObstacle()
    {
    Recheck:
        int randomCellX = Random.Range(0, gridX - 1);
        int randomCellZ = Random.Range(0, gridZ - 1);

        if (!Grid[randomCellX, randomCellZ])
        {
            Grid[randomCellX, randomCellZ] = true;
            Vector2 loc;
            calculateWorldPosXZ(randomCellX, randomCellZ, out loc);
            Instantiate(Obstacles, new Vector3(loc.x, 15, loc.y), Quaternion.identity, obstacleHolder);
        }
        else
        {
            goto Recheck;
        }
    }

    private void SpawnMines()
    {
    Recheck:
        int randomCellX = Random.Range(0, gridX - 1);
        int randomCellZ = Random.Range(0, gridZ - 1);

        if (!Grid[randomCellX, randomCellZ])
        {
            Grid[randomCellX, randomCellZ] = true;
            Vector2 loc;
            calculateWorldPosXZ(randomCellX, randomCellZ, out loc);
            Instantiate(Mines, new Vector3(loc.x, 15, loc.y), Quaternion.identity, mineHolder);
        }
        else
        {
            goto Recheck;
        }

    }

    internal bool spawnEventRunning = false;
    public IEnumerator SpawnEvent()
    {
        spawnEventRunning = true;

        yield return new WaitUntil(()=> Player!=null);

        playerController = Player.GetComponent<PlayerController>();

        //Clear Obstacle,Mine and Orbs
        RemoveExsistingObstacleMinesOrbs();


        for (int i = 0; i < maxObstacleOnField; i++)
        {
            SpawnObstacle();
        }

        for (int i = 0; i < maxMinesOnField; i++)
        {
            SpawnMines();
        }

        for (int i = 0; i < maxOrbsOnField; i++)
        {
            StartCoroutine(SpawnScoreOrb(true));
        }

        yield return new WaitForSeconds(0.1f);

        spawnEventRunning = false;
        playerController.enabled = true;
    }

    public void calculateCell(Vector2 worldPos, out int x, out int z)
    {
        //worldpos/20+8
        int incX = (int)((worldPos.x - offsetX) / scaleOfDice);
        int incZ = (int)((worldPos.y - offsetZ) / scaleOfDice);

        x = centerCell[0] + incX;
        z = centerCell[1] + incZ;
    }

    public void calculateWorldPosXZ(int x, int z, out Vector2 worldPosXZ)
    {
        //cell-8*20
        float Xpos = (x - centerCell[0]) * scaleOfDice;
        float Zpos = (z - centerCell[1]) * scaleOfDice;

        worldPosXZ = new Vector2(Xpos + offsetX, Zpos + offsetZ);
    }

    public void setSurroundingGrid(int unitToset = 1, bool status = true)
    {
        //down of current Pos
        for (int i = 1; i <= unitToset; i++)
        {
            if (playerController.playerOccupiedPos[0, 0] + i >= 0 && playerController.playerOccupiedPos[0, 0] + i < gridX &&
               playerController.playerOccupiedPos[0, 1] >= 0 && playerController.playerOccupiedPos[0, 1] < gridZ)
                Grid[playerController.playerOccupiedPos[0, 0] + i, playerController.playerOccupiedPos[0, 1]] = status;
        }

        //right of current Pos
        for (int i = 1; i <= unitToset; i++)
        {
            if (playerController.playerOccupiedPos[0, 0] >= 0 && playerController.playerOccupiedPos[0, 0] < gridX &&
               playerController.playerOccupiedPos[0, 1] + i >= 0 && playerController.playerOccupiedPos[0, 1] + i < gridZ)
                Grid[playerController.playerOccupiedPos[0, 0], playerController.playerOccupiedPos[0, 1] + i] = status;
        }

        //left of current Pos
        for (int i = 1; i <= unitToset; i++)
        {
            if (playerController.playerOccupiedPos[0, 0] >= 0 && playerController.playerOccupiedPos[0, 0] < gridX &&
               playerController.playerOccupiedPos[0, 1] - i >= 0 && playerController.playerOccupiedPos[0, 1] - i < gridZ)
                Grid[playerController.playerOccupiedPos[0, 0], playerController.playerOccupiedPos[0, 1] - i] = status;
        }

        //up of current Pos
        for (int i = 1; i <= unitToset; i++)
        {
            if (playerController.playerOccupiedPos[0, 0] - i >= 0 && playerController.playerOccupiedPos[0, 0] - i < gridX &&
               playerController.playerOccupiedPos[0, 1] >= 0 && playerController.playerOccupiedPos[0, 1] < gridZ)
                Grid[playerController.playerOccupiedPos[0, 0] - i, playerController.playerOccupiedPos[0, 1]] = status;
        }

        //top left diagonal
        for (int i = 1; i <= unitToset; i++)
        {
            if (playerController.playerOccupiedPos[0, 0] - i >= 0 && playerController.playerOccupiedPos[0, 0] - i < gridX &&
               playerController.playerOccupiedPos[0, 1] - i >= 0 && playerController.playerOccupiedPos[0, 1] - i < gridZ)
                Grid[playerController.playerOccupiedPos[0, 0] - i, playerController.playerOccupiedPos[0, 1] - i] = status;

        }

        //top right diagonal
        for (int i = 1; i <= unitToset; i++)
        {
            if (playerController.playerOccupiedPos[0, 0] - i >= 0 && playerController.playerOccupiedPos[0, 0] - i < gridX &&
               playerController.playerOccupiedPos[0, 1] + i >= 0 && playerController.playerOccupiedPos[0, 1] + i < gridZ)
                Grid[playerController.playerOccupiedPos[0, 0] - i, playerController.playerOccupiedPos[0, 1] + i] = status;
        }

        //bottom right diagonal
        for (int i = 1; i <= unitToset; i++)
        {
            if (playerController.playerOccupiedPos[0, 0] + i >= 0 && playerController.playerOccupiedPos[0, 0] + i < gridX &&
               playerController.playerOccupiedPos[0, 1] + i >= 0 && playerController.playerOccupiedPos[0, 1] + i < gridZ)
                Grid[playerController.playerOccupiedPos[0, 0] + i, playerController.playerOccupiedPos[0, 1] + i] = status;
        }

        //bottom left diagonal
        for (int i = 1; i <= unitToset; i++)
        {
            if (playerController.playerOccupiedPos[0, 0] + i >= 0 && playerController.playerOccupiedPos[0, 0] + i < gridX &&
               playerController.playerOccupiedPos[0, 1] - i >= 0 && playerController.playerOccupiedPos[0, 1] - i < gridZ)
                Grid[playerController.playerOccupiedPos[0, 0] + i, playerController.playerOccupiedPos[0, 1] - i] = status;
        }
    }

    //Converting seconds to min and sec
    private void ConvertToMinAndSec(int timeLeft, out int min, out int sec)
    {
        min = timeLeft / 60;
        float totalConversion = timeLeft / 60;
        float secConversion = Mathf.RoundToInt((totalConversion - min) * 60);
        sec = (int)secConversion;
    }

    private void DecideCurrentSpeed()
    {
        if (((ResetTime - ResetTimer) / ResetTime) * 100 >= 75f)
        {
            playerController.currentSpeed = playerController.rollSpeeds[0];

        }
        else if (((ResetTime - ResetTimer) / ResetTime) * 100 >= 50f)
        {
            playerController.currentSpeed = playerController.rollSpeeds[1];
        }
        else if (((ResetTime - ResetTimer) / ResetTime) * 100 >= 25f)
        {
            playerController.currentSpeed = playerController.rollSpeeds[2];
        }
        else if (((ResetTime - ResetTimer) / ResetTime) * 100 >= 0f)
        {
            playerController.currentSpeed = playerController.rollSpeeds[3];
        }
    }

    private void DecideSpawnableOrbs()
    {
        if (((ResetTime - ResetTimer) / ResetTime) * 100 >= 75f)
        {
            if (spawnableOrbPrefab.Count != OrbPrefab.Count)
            {
                spawnableOrbPrefab.Clear();
                for (int i = 0; i < OrbPrefab.Count; i++)
                {
                    spawnableOrbPrefab.Add(OrbPrefab[i]);
                }
            }
        }
        else if (((ResetTime - ResetTimer) / ResetTime) * 100 >= 50f)
        {
            if (spawnableOrbPrefab.Count == 4)
            {
                spawnableOrbPrefab.Clear();
                for (int i = 0; i < OrbPrefab.Count; i++)
                {
                    if (i + 1 < OrbPrefab.Count)
                        spawnableOrbPrefab.Add(OrbPrefab[i + 1]);
                    else
                        break;
                }
            }
        }
        else if (((ResetTime - ResetTimer) / ResetTime) * 100 >= 25f)
        {
            if (spawnableOrbPrefab.Count == 3)
            {
                spawnableOrbPrefab.Clear();
                for (int i = 0; i < OrbPrefab.Count; i++)
                {
                    if (i + 2 < OrbPrefab.Count)
                        spawnableOrbPrefab.Add(OrbPrefab[i + 2]);
                    else
                        break;
                }
            }
        }
        else if (((ResetTime - ResetTimer) / ResetTime) * 100 >= 0f)
        {
            if (spawnableOrbPrefab.Count == 2)
            {
                spawnableOrbPrefab.Clear();
                for (int i = 0; i < OrbPrefab.Count; i++)
                {
                    if (i + 3 < OrbPrefab.Count)
                        spawnableOrbPrefab.Add(OrbPrefab[i + 3]);
                    else
                        break;
                }
            }
        }
    }

    public void RemoveExsistingObstacleMinesOrbs()
    {

        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        if (obstacles.Length != 0)
        {
            foreach (GameObject obstacle in obstacles)
            {
                int x, z;
                calculateCell(new Vector2(obstacle.transform.position.x, obstacle.transform.position.z)
                    , out x, out z);
                Destroy(obstacle);
                Grid[x, z] = false;
            }
        }
        GameObject[] mines = GameObject.FindGameObjectsWithTag("Mines");
        if (mines.Length != 0)
        {
            foreach (GameObject mine in mines)
            {
                int x, z;
                calculateCell(new Vector2(mine.transform.position.x, mine.transform.position.z)
                    , out x, out z);
                Destroy(mine);
                Grid[x, z] = false;
            }
        }
        GameObject[] orbs = GameObject.FindGameObjectsWithTag("Orb");
        if (orbs.Length != 0)
        {
            foreach (GameObject orb in orbs)
            {
                int x, z;
                calculateCell(new Vector2(orb.transform.position.x, orb.transform.position.z)
                    , out x, out z);
                Destroy(orb);
                Grid[x, z] = false;
            }
        }
    }
}
