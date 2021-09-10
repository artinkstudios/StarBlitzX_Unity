using UnityEngine;
using System.Collections;
[RequireComponent(typeof(AudioSource))]

public class play_video : MonoBehaviour
{

    // Use this for initialization 
    void Start()
    {
		Debug.Log ("Used Movie texture");
        /*MovieTexture movie = GetComponent<Renderer>().material.mainTexture as MovieTexture;
        movie.loop = true;
        GetComponent<AudioSource>().clip = movie.audioClip;
        GetComponent<AudioSource>().Play();
        movie.Play();*/
    }

    // Update is called once per frame 
    void Update()
    {

    }
}
