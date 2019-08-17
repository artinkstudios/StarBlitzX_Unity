using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraterImpact : MonoBehaviour
{
    //Crater Impact
    public GameObject craterImpact;
    float floatInFrontOfWall = 0.00001f;
    private int maxDist;

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDist))
        {
            if (craterImpact && hit.transform.tag == "crater")
                Instantiate(craterImpact, hit.point + (hit.normal * floatInFrontOfWall), Quaternion.LookRotation(hit.normal));
        }
    }
}
    
        
   

   
