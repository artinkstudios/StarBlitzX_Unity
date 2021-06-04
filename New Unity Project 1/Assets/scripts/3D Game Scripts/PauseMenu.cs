using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Slider MusicSlider;
    public Slider SFXSlider;
    public GameObject resumebutton;
    public EventSystem uihandler;
    public GameAudio LaserTestSound;
    // Start is called before the first frame update
    void Start()
    {
        float music, sfx;
        ApplicationValues.GameMixer.GetFloat("MusicVolume", out music);
        ApplicationValues.GameMixer.GetFloat("SFXVolume", out sfx);
        MusicSlider.value = ApplicationValues.GetSliderfromVolume(music);
        SFXSlider.value = ApplicationValues.GetSliderfromVolume(sfx);
        uihandler.SetSelectedGameObject(resumebutton);
    }

    public void ChangeMusic()
    {
        ApplicationValues.GameMixer.SetFloat("MusicVolume", ApplicationValues.GetVolumeFromSlider(MusicSlider.value));
    }
    public void ChangeSFX()
    {
        ApplicationValues.GameMixer.SetFloat("SFXVolume", ApplicationValues.GetVolumeFromSlider(SFXSlider.value));
    }
    public void TestSFX()
    {
        LaserTestSound.PlayLaserShot();
    }

    public void TextDarkenHighlight(Text darken)
    {
        darken.color = new Color(0, 0, 0);
    }
    public void TextNormalGrey(Text normal)
    {
        normal.color = new Color(0.566f, 0.566f, 0.566f);
    }
}
