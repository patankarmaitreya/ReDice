using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private bool isMainMenuActive=false;
    public GameObject mainMenu;

    public void OnPause()
    {
        StartCoroutine(OnClickPause());
    }
    private IEnumerator OnClickPause()
    {
        if(!isMainMenuActive)
        {
            isMainMenuActive=true;
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enabled = false;
            yield return new WaitForSeconds(0.05f);
            mainMenu.SetActive(true);
        }
        else
        {
            isMainMenuActive=false;
            mainMenu.SetActive(false);
            yield return new WaitForSeconds(0.05f);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enabled = true;
        }
    }
}
