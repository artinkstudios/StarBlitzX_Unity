#pragma strict

private var EffectTime : float;

private var RemainingTime : float[];

private var ShieldColor : UnityEngine.Color;
private var tempColor : UnityEngine.Color;
private var hitCount : byte = 0;
private var shieldHP : float;

//alpha value of the ENTIRE shield in the moment of hit. minimum value is the shieldcolor alpha
var tempAlpha : float = 0.01;
//the time in ms while the above flashing happens
var flashTime : float;
//UI Text type object to show shield HP readout
var textHP : UI.Text;

function Start()
{
	hitCount = 0;
	RemainingTime = new float[10];
	
	ShieldColor = GetComponent.<Renderer>().material.GetColor("_ShieldColor");
	EffectTime = GetComponent.<Renderer>().material.GetFloat("_EffectTime");
	shieldHP = GetComponent.<Renderer>().material.GetFloat("_ShieldHP");
	
	tempColor = ShieldColor;
	tempColor.a = Mathf.Clamp(tempAlpha, ShieldColor.a, 1);
}

function Update()
{
	//shield regeneration
	shieldHP = Mathf.Clamp(shieldHP + 0.005 * Time.deltaTime, 0, 1);
	
	//if there is shield turn on collider
	if(shieldHP > 0.001)
	{
		GetComponent.<Renderer>().material.SetFloat("_ShieldHP", shieldHP);
		this.GetComponent.<Collider>().enabled = true;
	}
	
	if(Mathf.Max(RemainingTime) > 0)
	{
		if(Mathf.Max(RemainingTime) < (EffectTime - flashTime))
		{
			GetComponent.<Renderer>().material.SetColor("_ShieldColor", ShieldColor);
		}
		
		for(var i=0; i<10; i++)
		{
			RemainingTime[i] = Mathf.Clamp(RemainingTime[i] - Time.deltaTime * 1000, 0, EffectTime);				
			GetComponent.<Renderer>().material.SetFloat("_RemainingTime" + i.ToString(), RemainingTime[i]);
			GetComponent.<Renderer>().material.SetVector("_Position" + i.ToString(), transform.FindChild("hitpoint" + i.ToString()).position);		
		}
	}
	
	textHP.text = "Shield:" + (Mathf.RoundToInt(shieldHP * 1000)).ToString();
}
	
function OnCollisionEnter(collision : Collision)
{
	//draining shield HP
	shieldHP = Mathf.Clamp(shieldHP - 0.001, 0, 1);
	
	//if there is no shield, turn off collider, so the shots can hit the ship instead
	if(shieldHP <= 0.001)
	{
		shieldHP = 0;
		this.GetComponent.<Collider>().enabled = false;
		GetComponent.<Renderer>().material.SetFloat("_ShieldHP", shieldHP);
	}
	else
	{
		for (var contact : ContactPoint in collision.contacts)
		{		
			GetComponent.<Renderer>().material.SetColor("_ShieldColor", tempColor);
			GetComponent.<Renderer>().material.SetFloat("_ShieldHP", shieldHP);
			transform.FindChild("hitpoint" + hitCount.ToString()).position = contact.point;
			RemainingTime[hitCount] = EffectTime;
			hitCount++;
			if(hitCount > 9)
				hitCount = 0;
		}
	}
}