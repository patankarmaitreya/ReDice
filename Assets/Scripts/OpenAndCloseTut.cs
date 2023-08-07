using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenAndCloseTut : MonoBehaviour
{
    public GameObject tutTab;

    public void OpenTab()
    {
        tutTab?.SetActive(true);
        foreach (GameObject tab in GameObject.FindGameObjectsWithTag("Tutorial_Tab"))
        {
            if(tab!=tutTab)
            {
                tab.SetActive(false);
            }
        }
    }

    public void CloseTab()
    {
        tutTab?.SetActive(false);
    }
}
