using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EarthHitAnimationScript : MonoBehaviour
{
    public Animator HitAnimator;
    public Image HitImage;
    public AudioSource HitAudio;

    public AudioClip FirstClip;
    public AudioClip LastClip;
    public Sprite[] HitSprites;
    
    void Start()
    {
        //HitImage = GetComponent<Image>();
        HitAnimator = GetComponent<Animator>();
        HitAnimator.SetInteger("Health", Mathf.Abs(ApplicationValues.EarthHealth - 10));
        UpdateSprite();
    }

    

    public void EarthHit()
    {
        HitAnimator.SetInteger("Health", Mathf.Abs(ApplicationValues.EarthHealth - 10));
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (ApplicationValues.EarthHealth > 5)
        {
            HitAnimator.enabled = false;
            HitImage.sprite = HitSprites[Mathf.Abs(ApplicationValues.EarthHealth - 10)];
            HitAudio.Stop();
        }
        else if (ApplicationValues.EarthHealth > 1)
        {
            HitAnimator.enabled = true;
            HitAudio.clip = FirstClip;
            HitAudio.Play();
        }
        else if (ApplicationValues.EarthHealth == 1)
        {
            HitAudio.clip = LastClip;
            HitAudio.Play();
        }
        else
        {
            HitAudio.Stop();
        }
    }
    public void Mute()
    {
        HitAudio.Stop();
    }
}
