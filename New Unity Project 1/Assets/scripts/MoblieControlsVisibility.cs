using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoblieControlsVisibility : MonoBehaviour
{
    public GameObject MobileControls;
    
    void Start()
    {
#if UNITY_STANDALONE
        MobileControls.SetActive (false);
#endif
#if UNITY_ANDROID || UNITY_IOS
        MobileControls.SetActive (true);
#endif
    }

}
