using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazeFader : MonoBehaviour
{
	public float					theDepth = 0.5f;

	Color									theColor;
	List<SpriteRenderer>	theRenderers;
	float									theFade = 1f;

	void Start () 
	{
		theRenderers = new List<SpriteRenderer>();

		foreach ( Transform child in transform )
		{
			SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
			theRenderers.Add( sr );
			theColor = sr.color;
		}
		iTween.Init( this.gameObject );
		StartPhase ();
	}

	void Fader ( float _fade )
	{
		float _alpha = theColor.a * _fade;
		Color _newCol = new Color ( theColor.r, theColor.b, theColor.g, _alpha );

		foreach ( SpriteRenderer sr in theRenderers )
		{
			sr.color = _newCol;
		}
		theFade = _fade;
	}

	void StartPhase ()
	{
		float _duration = Random.Range ( 2f, 4f );
		float _newDest = Random.Range ( theDepth, 1f );
		float _delay = Random.Range ( 1f, 2f );

		iTween.ValueTo( this.gameObject, iTween.Hash( "from", theFade, "to" , _newDest, "delay", _delay, "time", _duration, "onupdate", "Fader", "oncomplete", "StartPhase" ) );
	}
}
