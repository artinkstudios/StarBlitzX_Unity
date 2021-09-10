using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsSettings : MonoBehaviour
{
    public Dropdown DifficultyDropdown;
    public Dropdown ScreenResolutionDropdown;
    public Dropdown QualityDropdown;
    public Toggle FullScreenSelection;
    public Button tempButton;
    public Text DifficultyAltHighlight;
    public Dropdown ControlsDropdown;

    public Slider MusicSlider;
    public Slider SFXSlider;
    public Slider ControlSlider;
    public AudioClip LaserShotSample;
    public AudioSource SampleSource;

    private static int[] ScreenWidths = { 800, 1024, 1280, 1280, 1280, 1360, 1366, 1440, 1600, 1920 };
    private static int[] ScreenHeights = { 600, 768, 600,  720,  768,  768,  768,  1080, 900,  1080 };
    // Start is called before the first frame update
    void Start()
    {
        DifficultyDropdown.value = (ApplicationValues.isHard ? 1 : 0);
        ControlsDropdown.value = (ApplicationValues.MouseControl ? 1 : 0);
        if (ApplicationValues.MouseControl)
        {
            ControlSlider.value = (ApplicationValues.MouseSpeed / 12) - 1; //default mouse is 18, range 12-24
        }
        else
        {
            ControlSlider.value = ((ApplicationValues.KeyboardSpeed / 5) - 1) / 2; //default keyboard is 5, range 5-15
        }
        ScreenResolutionDropdown.value = OptionsSettings.GetResolutionIndex(Screen.currentResolution);
        QualityDropdown.value = QualitySettings.GetQualityLevel();
        FullScreenSelection.isOn = Screen.fullScreen;

        float music, sfx;
        ApplicationValues.GameMixer.GetFloat("MusicVolume", out music);
        ApplicationValues.GameMixer.GetFloat("SFXVolume", out sfx);
        MusicSlider.value = ApplicationValues.GetSliderfromVolume(music);
        //MusicSlider.SendMessage("onValueChanged");
        SFXSlider.value = ApplicationValues.GetSliderfromVolume(sfx);
        //SFXSlider.SendMessage("onValueChanged");

#if UNITY_IOS || UNITY_ANDROID
        ControlsDropdown.captionText.text = "Mobile Controls";
        ControlsDropdown.interactable = false;
        ControlSlider.value = (ApplicationValues.JoystickSpeed / 5) - 1;
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DifficultySelection()
    {
        if (DifficultyDropdown.value == 0)
        {
            ApplicationValues.isHard = false;
        }
        else
        {
            ApplicationValues.isHard = true;
        }
    }

    public void ChangeResolution()
    {
        Screen.SetResolution(ScreenWidths[ScreenResolutionDropdown.value], ScreenHeights[ScreenResolutionDropdown.value], Screen.fullScreen);
    }

    public void ChangeQuality()
    {
        QualitySettings.SetQualityLevel(QualityDropdown.value, true);
    }

    public void ChangeControls()
    {
        ApplicationValues.MouseControl = (ControlsDropdown.value == 0 ? false : true);
        if (ApplicationValues.MouseControl)
        {
            ControlSlider.value = (ApplicationValues.MouseSpeed / 12) - 1;
        } else
        {
            ControlSlider.value = ((ApplicationValues.KeyboardSpeed / 5) - 1) / 2;
        }
    }

    public void ChangeFullScreen()
    {
        Screen.fullScreen = FullScreenSelection.isOn;
    }
    public void ChangeToggle()
    {
        FullScreenSelection.isOn = !FullScreenSelection.isOn;
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
        SampleSource.PlayOneShot(LaserShotSample);
    }
    public void ChangeControlSpeed()
    {
#if UNITY_STANDALONE
        if (ControlsDropdown.value == 0)
        {
            ApplicationValues.KeyboardSpeed = ((ControlSlider.value *2) + 1) * 5;
        } else
        {
            ApplicationValues.MouseSpeed = (ControlSlider.value + 1) * 12;
        }

#endif
#if UNITY_IOS || UNITY_ANDROID
        ApplicationValues.JoystickSpeed = (ControlSlider.value + 1) * 5;
#endif
    }

    public void DifficultyTextHighlight()
    {
        DifficultyAltHighlight.color = new Color(1, 1, 1);
    }
    public void DifficultyTextNormal()
    {
        DifficultyAltHighlight.color = new Color(0.568f, 0.568f, 0.568f);
    }


    public void TextLightenHighlight(Text lighten)
    {
        lighten.color = new Color(1, 1, 1);
    }
    public void TextDarkenHighlight(Text darken)
    {
        darken.color = new Color(0, 0, 0);
    }
    public void TextNormalGrey(Text normal)
    {
        normal.color = new Color(0.568f, 0.568f, 0.568f);
    }

    public static bool InAcceptableResolutions(Resolution current)
    {
        bool found = false;
        for (int i = ScreenWidths.Length - 1; i >= 0 && !found; i--)
        {
            if (ScreenWidths[i] == current.width && ScreenHeights[i] == current.height)
            {
                found = true;
            }
        }
        return found;
    }
    public static int GetResolutionIndex (Resolution current)
    {
        bool found = false;
        for (int i = ScreenWidths.Length - 1; i >= 0 && !found; i--)
        {
            if (ScreenWidths[i] == current.width && ScreenHeights[i] == current.height)
            {
                found = true;
                return i;
            }
        }
        return ScreenWidths.Length - 1;
    }
    public static Resolution GetHighestResolution()
    {
        Resolution highest = new Resolution();
        highest.width = ScreenWidths[ScreenWidths.Length - 1];
        highest.height = ScreenHeights[ScreenHeights.Length - 1];
        highest.refreshRate = 60;
        return highest;
    }
}
