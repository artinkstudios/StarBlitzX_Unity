using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpotShieldEffect : MonoBehaviour
{

    //Uses shader "Shieldeffect"

    private float EffectTime;

    private float[] RemainingTime;
    private Vector3[] HitPoints;

    private Color ShieldColor;
    private Color tempColor;
    private int hitCount = 0;
    //private float shieldHP;

    //alpha value of the ENTIRE shield in the moment of hit. minimum value is the shieldcolor alpha
    float tempAlpha = 0.001f;
    //the time in ms while the above flashing happens
    float flashTime;
    //UI Text type object to show shield HP readout
    //Text textHP;

    // Start is called before the first frame update
    void Start()
    {
        hitCount = 0;
        RemainingTime = new float[10];
        HitPoints = new Vector3[10];

        ShieldColor = GetComponent<Renderer>().material.GetColor("_ShieldColor");
        EffectTime = GetComponent<Renderer>().material.GetFloat("_EffectTime");
        //shieldHP = GetComponent<Renderer>().material.GetFloat("_ShieldHP");

        tempColor = ShieldColor;
        tempColor.a = Mathf.Clamp(tempAlpha, ShieldColor.a, 1);
    }

    // Update is called once per frame
    void Update()
    {
        //shield regeneration
        //shieldHP = Mathf.Clamp(shieldHP + 0.005f * Time.deltaTime, 0, 1);

        //if there is shield turn on collider
        /*if (shieldHP > 0.001)
        {
            GetComponent<Renderer>().material.SetFloat("_ShieldHP", shieldHP);
            this.GetComponent<Collider>().enabled = true;
        }*/

        if (Mathf.Max(RemainingTime) > 0)
        {
            if (Mathf.Max(RemainingTime) < (EffectTime - flashTime))
            {
                GetComponent<Renderer>().material.SetColor("_ShieldColor", ShieldColor);
            }

            for (int i = 0; i < 10; i++)
            {
                RemainingTime[i] = Mathf.Clamp(RemainingTime[i] - Time.deltaTime * 1000, 0, EffectTime);
                GetComponent<Renderer>().material.SetFloat("_RemainingTime" + i.ToString(), RemainingTime[i]);
                GetComponent<Renderer>().material.SetVector("_Position" + i.ToString(), HitPoints[i]);
            }
        }
        
        //textHP.text = "Shield:" + (Mathf.RoundToInt(shieldHP * 1000)).ToString();
    }

    void OnCollisionEnter(Collision other)
    {
        //draining shield HP
        //shieldHP = Mathf.Clamp(shieldHP - 0.001f, 0, 1);

        //if there is no shield, turn off collider, so the shots can hit the ship instead
        /*if (shieldHP <= 0.001f)
        {
            shieldHP = 0;
            this.GetComponent< Collider > ().enabled = false;
            GetComponent< Renderer > ().material.SetFloat("_ShieldHP", shieldHP);
        }*/
        
        foreach (ContactPoint contact in other.contacts)
        {
            GetComponent< Renderer > ().material.SetColor("_ShieldColor", tempColor);
            //GetComponent< Renderer > ().material.SetFloat("_ShieldHP", 1);
            HitPoints[hitCount] = contact.point;
            RemainingTime[hitCount] = EffectTime;
            hitCount++;
            if (hitCount > 9)
                hitCount = 0;
        }
    }
    
    public void Add(Vector3 Impact)
    {
        GetComponent<Renderer>().material.SetColor("_ShieldColor", tempColor);
        //GetComponent< Renderer > ().material.SetFloat("_ShieldHP", 1);
        HitPoints[hitCount] = Impact;
        RemainingTime[hitCount] = EffectTime;
        hitCount++;
        if (hitCount > 9)
            hitCount = 0;
    }
}
