using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorPlatform : MonoBehaviour
{

    public RuntimeAnimatorController PC;
    public RuntimeAnimatorController Mobile;

    void Start()
    {
#if UNITY_STANDALONE
        GetComponent<Animator>().runtimeAnimatorController = PC;
#endif

#if UNITY_IOS || UNITY_ANDROID
		GetComponent<Animator>().runtimeAnimatorController = Mobile;
#endif

    }


    void Update()
    {
        
    }
}
