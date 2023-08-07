using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopROTD : MonoBehaviour
{
    public void StopRoll()
    {
        EventSystem.GetInstance().RollAvailable = false;
    }
}
