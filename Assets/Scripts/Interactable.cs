using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string promptMessage = "Press E to interact";

    public virtual void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);
    }

}
