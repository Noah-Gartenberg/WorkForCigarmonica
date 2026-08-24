using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour, IClickable
{
    public LinearDialogue speak2Booker;
    public string loadLevel;
    public void Interact()
    {
        GameObject item = GameObject.FindGameObjectWithTag("Item");
        if(item != null && item.activeInHierarchy)
        {
            SceneManager.LoadScene(loadLevel);
        }
        else
        {   
            DialogueManagerV2.Instance.StartDialogue(speak2Booker,0," ");
            
        }
        
    }
}
