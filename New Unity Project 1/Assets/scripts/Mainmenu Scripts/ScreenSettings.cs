using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenSettings : MonoBehaviour
{
    public Button FullScreenButton;
    public Sprite FullScreenYesHighlight;
    public Sprite FullScreenYesNormal;
    public Sprite FullScreenNoHighlight;
    public Sprite FullScreenNoNormal;

    public Image QualitySettingsImage;
    public Sprite[] QualityLevelSprites;

    public Text DisplayResolution;

    private int CurrentQuality;
    private bool CurrentFullScreen;
    private int currentResolutionIndex;
    private Resolution ScreenResolution;
    private Resolution[] AvailableResolution;
    private static int[] widths = { 800, 1024, 1280, 1280, 1280, 1360, 1366, 1440, 1600, 1920 };
    private static int[] heights = {600, 768, 600, 720, 768, 768, 768, 1080, 900, 1080};
    
    void Start()
    {
        CurrentFullScreen = Screen.fullScreen;
        ChangeFullScreenSprite();
        CurrentQuality = QualitySettings.GetQualityLevel();
        ChangeQualitySprite();
        AvailableResolution = Screen.resolutions;
        ScreenResolution = Screen.currentResolution;
        
        GetResolutionIndex();
        SetDisplayResolution();
    }

    
    void Update()
    {
        
    }

    public void DecreaseQuality()
    {
        CurrentQuality--;
        if (CurrentQuality < 0)
        {
            CurrentQuality = 0;
        }
        ChangeQualitySprite();
        QualitySettings.DecreaseLevel(true);
    }
    public void IncreaseQuality()
    {
        CurrentQuality++;
        if (CurrentQuality >= QualitySettings.names.Length)
        {
            CurrentQuality = QualitySettings.names.Length - 1;
        }
        ChangeQualitySprite();
        QualitySettings.IncreaseLevel(true);
    }
    private void ChangeQualitySprite()
    {
        if (CurrentQuality >= 0 && CurrentQuality < QualityLevelSprites.Length)
        {
            QualitySettingsImage.sprite = QualityLevelSprites[CurrentQuality];
        }
    }

    public void ChangeFullScreen()
    {
        CurrentFullScreen = !CurrentFullScreen;
        ChangeFullScreenSprite();
        Screen.fullScreen = CurrentFullScreen;
    }
    private void ChangeFullScreenSprite()
    {
        SpriteState buttonstate = FullScreenButton.spriteState;
        if (!CurrentFullScreen)
        {
            FullScreenButton.image.sprite = FullScreenNoNormal;
            buttonstate.highlightedSprite = FullScreenNoHighlight;
            buttonstate.selectedSprite = FullScreenNoHighlight;
        }
        else
        {
            FullScreenButton.image.sprite = FullScreenYesNormal;
            buttonstate.highlightedSprite = FullScreenYesHighlight;
            buttonstate.selectedSprite = FullScreenYesHighlight;
        }
        FullScreenButton.spriteState = buttonstate;
    }

    public void IncreaseResolution()
    {
        currentResolutionIndex++;
        if (currentResolutionIndex >= widths.Length)
        {
            currentResolutionIndex = widths.Length - 1;
        }
        
        SetDisplayResolution();
        SetScreenResolution();
    }
    public void DecreaseResolution()
    {
        currentResolutionIndex--;
        if (currentResolutionIndex < 0)
        {
            currentResolutionIndex = 0;
        }
        
        SetDisplayResolution();
        SetScreenResolution();
    }
    private void SetScreenResolution()
    {
        Screen.SetResolution(widths[currentResolutionIndex], heights[currentResolutionIndex], Screen.fullScreen);
        //ScreenResolution = Screen.currentResolution;
    }
    private void SetDisplayResolution()
    {
        DisplayResolution.text = widths[currentResolutionIndex] + " x " + heights[currentResolutionIndex];
    }
    private void GetResolutionIndex()
    {
        bool found = false;
        for (int i = widths.Length-1; i>=0 && !found; i--)
        {
            if (widths[i] == ScreenResolution.width && heights[i] == ScreenResolution.height)
            {
                found = true;
                currentResolutionIndex = i;
            }
        }
    }
    public static bool InAcceptableResolutions(Resolution current)
    {
        bool found = false;
        for (int i = widths.Length - 1; i >= 0 && !found; i--)
        {
            if (widths[i] == current.width && heights[i] == current.height)
            {
                found = true;
                //currentResolutionIndex = i;
            }
        }
        return found;
    }
    public static Resolution GetHighestResolution()
    {
        Resolution highest = new Resolution();
        highest.width = widths[widths.Length - 1];
        highest.height = heights[heights.Length - 1];
        highest.refreshRate = 60;
        return highest;
    }
}
