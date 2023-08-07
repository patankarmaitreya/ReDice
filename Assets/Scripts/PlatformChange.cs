using System.Collections;
using System.Collections.Generic;
using AssociativeFiles.Custom;
using UnityEngine;

public class PlatformChange : MonoBehaviour
{
    private static PlatformChange Instance;

    public GameObject PlayerPrefab;
    public EventSystem EventSys;

    GameObject Dice;

    public GameObject Platform;

    //Player Speed
    internal int PlayerSpeed1;
    internal int PlayerSpeed2;
    internal int PlayerSpeed3;
    internal int PlayerSpeed4;

    public GameObject GetDice
    {
        get { return Dice; }
        set { Dice = value; }
    }

    void Awake()
    {
        Instance = this;
        StartCoroutine(ChangePlatform());
    }

    public static PlatformChange GetInstance()
    {
        return Instance;
    }

    public bool platformChanging = false;
    public IEnumerator ChangePlatform()
    {
        platformChanging = true;

        ModProbability();

        EventSys.RemoveExsistingObstacleMinesOrbs();

        Dice = Instantiate(PlayerPrefab);
        Dice.GetComponent<PlayerController>().Init(EventSys);
        Dice.SetActive(true);
        EventSys.Init(Dice);

        Dice.GetComponent<PlayerController>().rollSpeeds[0] = PlayerSpeed1;
        Dice.GetComponent<PlayerController>().rollSpeeds[1] = PlayerSpeed2;
        Dice.GetComponent<PlayerController>().rollSpeeds[2] = PlayerSpeed3;
        Dice.GetComponent<PlayerController>().rollSpeeds[3] = PlayerSpeed4;

        yield return new WaitUntil(() => GameObject.FindGameObjectWithTag("Player") == Dice);

        EventSys.resetFlag = true;

        platformChanging = false;
        yield return null;
    }

    private void ModProbability()
    {
        string platformFace = null ;

        RaycastHit hit;

        if (Physics.Raycast(Platform.transform.position,Vector3.up, out hit, Mathf.Infinity,
            1<<LayerMask.NameToLayer("Platform")))
        {
            platformFace = hit.collider.GetComponent<PlatformName>().platformName;
        }

        switch (platformFace)
        {
            case "Red":
                //Red
                EventSys.probOrb_1.SetWeightedRNG(0, 0, 65);
                EventSys.probOrb_2.SetWeightedRNG(1, 1, 25);
                EventSys.probOrb_3.SetWeightedRNG(2, 2, 6);
                EventSys.probOrb_4.SetWeightedRNG(3, 3, 4);

                EventSys.maxOrbsOnField = 15;
                EventSys.maxMinesOnField = 20;
                EventSys.maxObstacleOnField = 20;

                //Mine Mod
                EventSys.Mines.GetComponent<SpawnItemBehavior>().damageCaused = 2;

                //Orb Mod
                EventSys.OrbPrefab[0].GetComponent<SpawnItemBehavior>().Score = 5;
                EventSys.OrbPrefab[1].GetComponent<SpawnItemBehavior>().Score = 10;
                EventSys.OrbPrefab[2].GetComponent<SpawnItemBehavior>().Score = 15;
                EventSys.OrbPrefab[3].GetComponent<SpawnItemBehavior>().Score = 20;

                //Player Speed
                PlayerSpeed1 = 6;
                PlayerSpeed2 = 9;
                PlayerSpeed3 = 9;
                PlayerSpeed4 = 10;

                break;

            case "Orange":

                //Orange
                EventSys.probOrb_1.SetWeightedRNG(0, 0, 55);
                EventSys.probOrb_2.SetWeightedRNG(1, 1, 30);
                EventSys.probOrb_3.SetWeightedRNG(2, 2, 14);
                EventSys.probOrb_4.SetWeightedRNG(3, 3, 6);

                EventSys.maxOrbsOnField = 15;
                EventSys.maxMinesOnField = 15;
                EventSys.maxObstacleOnField = 20;
                
                //Mine Mod
                EventSys.Mines.GetComponent<SpawnItemBehavior>().damageCaused = 2;

                //Orb Mod
                EventSys.OrbPrefab[0].GetComponent<SpawnItemBehavior>().Score = 10;
                EventSys.OrbPrefab[1].GetComponent<SpawnItemBehavior>().Score = 15;
                EventSys.OrbPrefab[2].GetComponent<SpawnItemBehavior>().Score = 20;
                EventSys.OrbPrefab[3].GetComponent<SpawnItemBehavior>().Score = 25;

                //Player Speed
                PlayerSpeed1 = 9;
                PlayerSpeed2 = 9;
                PlayerSpeed3 = 10;
                PlayerSpeed4 = 18;

                break;

            case "Yellow":

                //Yellow
                EventSys.probOrb_1.SetWeightedRNG(0, 0, 40);
                EventSys.probOrb_2.SetWeightedRNG(1, 1, 30);
                EventSys.probOrb_3.SetWeightedRNG(2, 2, 20);
                EventSys.probOrb_4.SetWeightedRNG(3, 3, 10);

                EventSys.maxOrbsOnField = 20;
                EventSys.maxMinesOnField = 10;
                EventSys.maxObstacleOnField = 15;

                //Mine Mod
                EventSys.Mines.GetComponent<SpawnItemBehavior>().damageCaused = 2;

                //Orb Mod
                EventSys.OrbPrefab[0].GetComponent<SpawnItemBehavior>().Score = 15;
                EventSys.OrbPrefab[1].GetComponent<SpawnItemBehavior>().Score = 20;
                EventSys.OrbPrefab[2].GetComponent<SpawnItemBehavior>().Score = 25;
                EventSys.OrbPrefab[3].GetComponent<SpawnItemBehavior>().Score = 30;

                //Player Speed
                PlayerSpeed1 = 9;
                PlayerSpeed2 = 9;
                PlayerSpeed3 = 18;
                PlayerSpeed4 = 30;
                break;

            case "Green":

                //Green
                EventSys.probOrb_1.SetWeightedRNG(0, 0, 40);
                EventSys.probOrb_2.SetWeightedRNG(1, 1, 10);
                EventSys.probOrb_3.SetWeightedRNG(2, 2, 30);
                EventSys.probOrb_4.SetWeightedRNG(3, 3, 20);

                EventSys.maxOrbsOnField = 20;
                EventSys.maxMinesOnField = 10;
                EventSys.maxObstacleOnField = 10;

                //Mine Mod
                EventSys.Mines.GetComponent<SpawnItemBehavior>().damageCaused = 1;

                //Orb Mod
                EventSys.OrbPrefab[0].GetComponent<SpawnItemBehavior>().Score = 10;
                EventSys.OrbPrefab[1].GetComponent<SpawnItemBehavior>().Score = 20;
                EventSys.OrbPrefab[2].GetComponent<SpawnItemBehavior>().Score = 35;
                EventSys.OrbPrefab[3].GetComponent<SpawnItemBehavior>().Score = 40;

                //Player Speed
                PlayerSpeed1 = 9;
                PlayerSpeed2 = 10;
                PlayerSpeed3 = 18;
                PlayerSpeed4 = 30;

                break;

            case "Blue":

                //Blue
                EventSys.probOrb_1.SetWeightedRNG(0, 0, 25);
                EventSys.probOrb_2.SetWeightedRNG(1, 1, 15);
                EventSys.probOrb_3.SetWeightedRNG(2, 2, 35);
                EventSys.probOrb_4.SetWeightedRNG(3, 3, 25);

                EventSys.maxOrbsOnField = 25;
                EventSys.maxMinesOnField = 15;
                EventSys.maxObstacleOnField = 15;

                //Mine Mod
                EventSys.Mines.GetComponent<SpawnItemBehavior>().damageCaused = 1;

                //Orb Mod
                EventSys.OrbPrefab[0].GetComponent<SpawnItemBehavior>().Score = 10;
                EventSys.OrbPrefab[1].GetComponent<SpawnItemBehavior>().Score = 20;
                EventSys.OrbPrefab[2].GetComponent<SpawnItemBehavior>().Score = 30;
                EventSys.OrbPrefab[3].GetComponent<SpawnItemBehavior>().Score = 40;

                //Player Speed
                PlayerSpeed1 = 10;
                PlayerSpeed2 = 18;
                PlayerSpeed3 = 18;
                PlayerSpeed4 = 30;

                break;

            case "White":

                //White
                EventSys.probOrb_1.SetWeightedRNG(0, 0, 20);
                EventSys.probOrb_2.SetWeightedRNG(1, 1, 10);
                EventSys.probOrb_3.SetWeightedRNG(2, 2, 40);
                EventSys.probOrb_4.SetWeightedRNG(3, 3, 30);

                EventSys.maxOrbsOnField = 25;
                EventSys.maxMinesOnField = 10;
                EventSys.maxObstacleOnField = 20;

                //Mine Mod
                EventSys.Mines.GetComponent<SpawnItemBehavior>().damageCaused = 1;

                //Orb Mod
                EventSys.OrbPrefab[0].GetComponent<SpawnItemBehavior>().Score = 30;
                EventSys.OrbPrefab[1].GetComponent<SpawnItemBehavior>().Score = 20;
                EventSys.OrbPrefab[2].GetComponent<SpawnItemBehavior>().Score = 40;
                EventSys.OrbPrefab[3].GetComponent<SpawnItemBehavior>().Score = 50;

                //Player Speed
                PlayerSpeed1 = 10;
                PlayerSpeed2 = 18;
                PlayerSpeed3 = 30;
                PlayerSpeed4 = 90;

                break;
        }
    }
}
