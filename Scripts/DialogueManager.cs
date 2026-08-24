using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public TextMeshProUGUI dialogueText;
    public bool OnByDefault = true;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    public void Start()
    {
        if(!OnByDefault)
        {
            dialogueText.gameObject.SetActive(false);
        }
    }

    public void ShowDialogue(string text)
    {
        dialogueText.text = text;
        dialogueText.gameObject.SetActive(true);
    }

    public void HideDialogue()
    {
        dialogueText.gameObject.SetActive(false);
    }
}