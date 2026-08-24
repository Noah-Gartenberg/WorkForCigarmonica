using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInterfaceObject : MonoBehaviour, IClickable
{
    // Start is called before the first frame update
    public void Interact()
    {
        Debug.Log("Works");
    }
}
