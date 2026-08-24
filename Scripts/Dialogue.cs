using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Dialogue : ScriptableObject
{
    [TextArea]
    public string[] dialogue;

    public string GetDialogue(int index)
    {
        return dialogue[index];
    }
}
