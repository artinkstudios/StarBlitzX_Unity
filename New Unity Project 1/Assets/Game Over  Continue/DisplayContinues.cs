using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayContinues : MonoBehaviour
{
    public Text ContinuesLeftText;
    // Start is called before the first frame update
    void Start()
    {
        ContinuesLeftText.text = ApplicationValues.FreeContinue.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
