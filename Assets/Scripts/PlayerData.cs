using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public List<int> scores = new List<int>();

    public PlayerData(HUDdata_SO hudData)
    {
        if(scores.Count == 0)
        {
            scores.Insert(0, hudData.currentScore);
        }
        else
        {
            if (scores[0] < hudData.currentScore)
            {
                scores.Insert(0, hudData.currentScore);
                if(scores.Count > 10)
                {
                    scores.Remove(11);
                }
            }
        }
    }
}
