using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSphere : MonoBehaviour
{
    public Material material;

    private void OnCollisionEnter(Collision other)
    {
        material.SetVector("_InteractPoint", other.contacts[0].point);
        material.SetFloat("_Toggle", 1);//Use toggle to control whether the input vertex is valid


    }
    private void OnCollisionExit(Collision other)
    {
        material.SetFloat("_Toggle", 0);
    }
}