using UnityEngine;
using UnityEngine.Audio;

public class ApplicationValues {

	static public string FileName = "HighScore.dat";
    static public int EarthHealth = 10;
	static public bool isHard = false;
	static public float Score = 0;
	static public float HighScore = 0;
	static public int Part = 1;
	static public bool demo = true;
	static public int FreeContinue = 0;
	static public AudioMixer GameMixer;
	static public bool FirstPlay = true;
	static public HighScore ScoreFile;
    static public bool MouseControl = false;
    static public float KeyboardSpeed = 5;
    static public float MouseSpeed = 18;
    static public float JoystickSpeed = 7.5f;

    public static float GetVolumefromValue(int value)
    {
        //float volume = ((50.0f / 9) * currentValue) - 50;
        float volume = 0;
        if (value == 0)
        {
            volume = -80;
        }
        else
        {
            volume = Mathf.Log10(value / 9.0f) * 20;
        }
        return volume;
    }
    public static int GetValuefromVolume(float volume)
    {
        int value = 0;
        value = (volume == -80 ? 0 : (int)(Mathf.Pow(10, (volume / 20)) * 9));
        return value;
    }
    public static float GetVolumeFromSlider(float value)
    {
        float volume = 0;
        volume = (value == 0 ? -80 : Mathf.Log10(value) * 20);
        return volume;
    }
    public static float GetSliderfromVolume(float volume)
    {
        float value = 0;
        value = (volume == -80 ? 0 : (Mathf.Pow(10, (volume / 20))));
        return value;
    }
}
