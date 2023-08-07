//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.IO;
//public class SaveGame : MonoBehaviour
//{
//    private static SaveGame Instance;
//    private static string saveFolder;
//    public HUDdata_SO score;
//    private void Awake()
//    {
//        Instance = this;
//        saveFolder = Application.dataPath + "/SAVES/";
//    }
//    public static SaveGame GetInstance()
//    {
//        return Instance;
//    }

//    private SaveData CreateSaveGO()
//    {
//        SaveData saveData = Load();

//        if (saveData.highScores.Count == 0)
//        {
//            saveData.highScores.Add(score.currentScore);
//        }
//        else
//        {

//            if (score.currentScore > saveData.highScores[saveData.highScores.Count - 1])
//            {
//                if (saveData.highScores.Count < 10)
//                {
//                    saveData.highScores.Add(score.currentScore);
//                }
//                else
//                {
//                    saveData.highScores.RemoveAt(0);
//                    saveData.highScores.Add(score.currentScore);
//                }
//            }
//        }
        

//        return saveData;       
//    }

//    public void  SaveAsJSON()
//    {
//        SaveData saveData = CreateSaveGO();
       
//        string json = JsonUtility.ToJson(saveData);
//        File.WriteAllText(saveFolder + "/save.txt", json);
//    }

//    public SaveData Load()
//    {
//        SaveData saveData = new SaveData();
//        if (File.Exists(saveFolder + "/save.txt"))
//        {
//            string saveString = File.ReadAllText(saveFolder + "/save.txt");

//            saveData = JsonUtility.FromJson<SaveData>(saveString);
            
//        }
//        else
//        {
//            string json = JsonUtility.ToJson(saveData);
//            File.WriteAllText(saveFolder + "/save.txt", json);
//        }
//        return saveData;
//    }
//}
