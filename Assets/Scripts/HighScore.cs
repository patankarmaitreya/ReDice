//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class HighScore : MonoBehaviour
//{
//    public Transform scoreHolder;
//    public GameObject scoreTemplatePrefab;

//    private void Start()
//    {
//        SaveData saveData = SaveGame.GetInstance().Load();

//        for(int i = saveData.highScores.Count-1; i>=0; i--)
//        {
//            GameObject scoreGO = Instantiate(scoreTemplatePrefab, scoreHolder);

//            SetScoreValues(scoreGO, saveData.highScores[i], i+1);
//        }
//    }
    
//    public void SetScoreValues(GameObject gameObject, int item, int i)
//    {
//        foreach (Transform child in gameObject.transform)
//        {
//            if (child.gameObject.name == "ScoreText")
//            {
//                child.gameObject.GetComponent<TMP_Text>().text = item.ToString();
//            }
//            else if (child.gameObject.name == "SRText")
//            {
//                child.gameObject.GetComponent<TMP_Text>().text = i.ToString();
//            }
//        }

//    }
//}
