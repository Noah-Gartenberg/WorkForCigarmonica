using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTreeManager : MonoBehaviour
{
    public static DialogueTreeManager Instance;
    
    public bool onByDefault = false;

    public TextMeshProUGUI dialogueTreeText;
    [SerializeField] GameObject dialogueBox;
    public GameObject answerBox;
    public Button[] answerObjects;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        if(!onByDefault)
        {
            dialogueTreeText.gameObject.SetActive(false);
        }
    }

    public void ShowDialogue(string text)
    {
        dialogueBox.gameObject.SetActive(true);
        dialogueTreeText.text = text;
    }

    public void HideDialogue()
    {
        dialogueTreeText.gameObject.SetActive(false);
    }
}
