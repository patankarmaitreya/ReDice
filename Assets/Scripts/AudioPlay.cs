using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlay : MonoBehaviour
{
    void Update()
    {
        if(this.GetComponent<AudioSource>().isPlaying == false)
        {
            Destroy(this.gameObject);
        }
    }
}
