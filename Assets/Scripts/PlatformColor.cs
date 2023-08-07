using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformColor : MonoBehaviour
{
    private Vector3 origin;
    RaycastHit hit;

    private void Start()
    {
        origin = new Vector3(0,-155,0);    
    }

    void Update()
    {
        int layerMask = 1 << 7;

        
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(origin, Vector3.up, out hit, Mathf.Infinity, layerMask))
        {
            Color();
        }
    }

    public string Color()
    {
        return hit.transform.gameObject.GetComponent<PlatformName>().platformName;
    }
}
