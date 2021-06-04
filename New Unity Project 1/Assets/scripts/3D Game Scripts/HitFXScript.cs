using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitFXScript : MonoBehaviour
{
    private float TimeAlive;

    void Start()
    {
        TimeAlive = 0;
    }

    // Update is called once per frame
    void Update()
    {
        TimeAlive += Time.deltaTime;
        if (TimeAlive >= 1)
        {
            Destroy(gameObject);
        }
    }
}
