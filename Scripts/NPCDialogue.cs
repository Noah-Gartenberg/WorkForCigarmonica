using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class NPCDialogue : MonoBehaviour, IClickable
{
    //You'll never believe what I nearly forgot when writing this code
    //Spoiler alert, it's the actual dialogue...
    //Anyway, here it is
    public Dialogue dialogue;

    //variable for the number of days past in case that's necessary
    //[SerializeField]  int daysPast;
    //If there should be any looping dialogue
    [SerializeField] bool shouldRepeatDialogue = false;
    //dialogue it should start on when dialogue starts
    [SerializeField] int currDialogue = 0;
    //Which dialogue line to end on
    public int endDialogueAfter;

    public PointAndClickMove playerCharacter;


    //if we want to use a dialogue tree - otherwise leave null
    public DialogueTree dialogueTree;
    int i = 0; //line of dialogue within trees - needed to be kept ouside of the Interact method
    bool canAnswer = false; //whether or not  player can answer questions
    bool showAnswers = false; //whether or not to show answers
    int answerChosen = -1;
    bool answerTriggered = false;

    //Method from which dialogue will basicallly be triggered and sent to dialogue manager
    //  -saving things like conditions and stuff for a subclass, because I'm not sure what's needed
    public void Interact()
    {
        if (dialogueTree != null)
        {
            if (DialogueManager.Instance.isActiveAndEnabled)
            {
                DialogueManager.Instance.HideDialogue();
                playerCharacter.setCanMove(false);
                if (DialogueTreeManager.Instance.isActiveAndEnabled && i == -1 && canAnswer)
                {
                    i = 0;
                }
            }
            if (!dialogueTree.sections[currDialogue].endAfterDialogue || (i < dialogueTree.sections[currDialogue].dialogue.dialogue.Length && i != -1))
            {

                if (i < dialogueTree.sections[currDialogue].dialogue.dialogue.Length)
                {
                    DialogueTreeManager.Instance.ShowDialogue(dialogueTree.sections[currDialogue].dialogue.dialogue[i]);
                    i++;
                }
                //should only do anything after this so long as it's set up correclty - shoudlnt' jump to end randomly
            }
            else if (dialogueTree.sections[currDialogue].endAfterDialogue && i >= dialogueTree.sections[currDialogue].dialogue.dialogue.Length)
            {
                playerCharacter.setCanMove(true);
                DialogueTreeManager.Instance.HideDialogue();
            }
            else if (!canAnswer && !showAnswers && !answerTriggered)
            {
                DialogueTreeManager.Instance.HideDialogue();
                i = -1;
                DialogueTreeManager.Instance.ShowDialogue(dialogueTree.sections[currDialogue].branchPoint.question);
                canAnswer = true;
            }
            else if (canAnswer && !showAnswers)
            {
                showAnswers = true;
                for (int j = 0; j < dialogueTree.sections[currDialogue].branchPoint.answers.Length; j++)
                {
                    DialogueTreeManager.Instance.answerObjects[j].GetComponentInChildren<TextMeshProUGUI>().text = dialogueTree.sections[currDialogue].branchPoint.answers[j].answerLabel;
                    DialogueTreeManager.Instance.answerObjects[j].gameObject.SetActive(true);
                }
            }
            else if (showAnswers && !answerTriggered)
            {
                //Do nothing - player will need to use num keys to trigger it
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    answerTriggered = true;
                    answerChosen = 1;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    answerTriggered = true;
                    answerChosen = 2;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    answerTriggered = true;
                    answerChosen = 3;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    answerTriggered = true;
                    answerChosen = 4;
                }
                else
                {

                }
            }
            else if (showAnswers && answerTriggered)
            {
                currDialogue = dialogueTree.sections[currDialogue].branchPoint.answers[answerChosen].nextElement;
                canAnswer = false;
                showAnswers = false;
                i = 0;
                answerChosen = 0;
            }
            else
            {
                answerTriggered = true;
            }

            

            
        }
        else if(currDialogue <= endDialogueAfter)
        {
            //show dialogue in dialogue manager
            playerCharacter.setCanMove(false);
            DialogueManager.Instance.ShowDialogue(dialogue.GetDialogue(currDialogue));
            currDialogue++;
            //Debug.Log("2");
        }
        else if(currDialogue > endDialogueAfter && !shouldRepeatDialogue)
        {
            DialogueManager.Instance.HideDialogue();
            playerCharacter.setCanMove(true);
            shouldRepeatDialogue = true;
        }
        else if (shouldRepeatDialogue)
        {
            playerCharacter.setCanMove(false);
            //Debug.Log("3");
            DialogueManager.Instance.ShowDialogue(dialogue.GetDialogue(currDialogue));
            shouldRepeatDialogue= false;
        }
        
    }
}
