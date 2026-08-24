using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//this is not the class you can use to make the characters - those will be split into subclasses
public class NPCtry2 : MonoBehaviour, IClickable
{
    //character's name
    public string NPCname;
    bool inDialogue = false;
    public DialogueManagerV2 manager; //change to dialogue manager v2
    int currentDialogueSection = 0;
    public DialogueV2[] Dialogues;
    public bool[] conditions = new bool[3];
    public int[] indices;
    bool failTrade = false;
    int loopDialogueIndex = -1;
    public int failTradeIndex = 3;
    public GameObject tradeItem;
    public bool firstInteractionDone = false;
    public SpriteRenderer mesh;
    //When setting dialogue, the fail dialogue trade needs to be the index of the dialogue right after the successful dialogue sections.
    //this dialogue shouldn't play, unless you've failed dialogue
    //3 booleans for 3 conditions if I remember correctly
    //before given item
    //when given item
    //after given item


    public bool isBackgroundCharacter;
    //if it's a background character, it will use randomized lines of dialogue


     private void Start()
    {
        manager = DialogueManagerV2.Instance;
    }
    private void Update(){
         mesh.transform.LookAt(new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z));
    }
    
    public void startDialogue(DialogueV2[] dialogues)
    {
        DialogueManagerV2.Instance.SetCurrentNPC(this);   
        inDialogue = true;
        if (loopDialogueIndex != -1)
        {

            manager.StartDialogue(dialogues, loopDialogueIndex, NPCname);
        }
        else if(failTrade)
        {
            //if the dialogue isn't correct, then it should run the fail trade, which should be the index right after the succesful trade dialouge
            if(isBackgroundCharacter)
            {
                failTradeIndex = 0;
            }
            manager.StartDialogue(dialogues[failTradeIndex],0, NPCname);
            failTrade = false;
        }
        else
        {
            //turn everything on when the player is in dialogue
            manager.StartDialogue(dialogues, currentDialogueSection, NPCname);
        }
    }

    public void SetCurrentDialogueSection(int section)
    {
        currentDialogueSection = section;
    }

    public int GetCurrentDialogueSection()
    {
        return currentDialogueSection;
    }

    public void endDialogue()
    {
        
        inDialogue = false;
    }

    //this will be the main overridden method in subclasses- checks conditions, and picks index based on them, will be set
    public void checkConditionsForCurrI()
    {
        if(!firstInteractionDone)
        {
            currentDialogueSection = 0;
            firstInteractionDone = true;
        }
        else if(isBackgroundCharacter)
        {
            //Get third index - first index is for a failed trade interaction - not for anything else
            //second index should be for dialogue that should be played on the first interaction with that character
            currentDialogueSection = UnityEngine.Random.Range(2, Dialogues.Length);
        }
        else
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                if (conditions[i])
                {
                    currentDialogueSection = indices[i];
                }
                //maybe add "else; break;"
            }
        }
    }
    
    public void TryActivateTradeItem() {
        if (tradeItem == null) {
            return;
        }else{
            tradeItem.SetActive(true);
        }

    }
    

    public void Interact()
    {
        
        if(inDialogue)
        {
            manager.ToNextLine();
            //Debug.Log("in dialogue");
        } 
        else
        {
            
            ////call startDialogue
            //call checkConditions
            ////then send every complete and applicable dialogue section to the dialogue manager
            //the manager will sort out how to render and make note of everything
            //manager will set currentDialgoueSection accordingly
            //manager will deal with stopping the dialogue being run
            //manager will call the dialogue
            //manager will make any calls to timer, to give quest/complete quest
            checkConditionsForCurrI();
            startDialogue(Dialogues);
            //Debug.Log("Starting dialogue");
        }
        
        
    }

    public void setCondition(int index, bool setTo)
    {
        conditions[index] = setTo;
    }
    
    
    /*Planning for when the player trades
     *first, check  if the item is the correct one
     * if not, play the failure dialogue (should have a specific dialogue string that we can write up and assign to the character, and then immediately exit dialogue - don't set any conditions
     * and depending on the npc, restart the level
     * 
     * if yes, set the condition, and then restart normal dialogue - end all coroutines first?, want to disrupt any dialogue - any? Like would any dialogue other than for what they need be playing? 
     */

    //This should check (when the player trades an item) 
    public void checkForDesiredItem(/*Probably going to be the item here*/)
    {
        /*if player has the desired item*/
        if (false && !isBackgroundCharacter) /*Will need ot change this when inventory is done!!!!!*/
        {
            setCondition(1, true);
            //automatically goes to new dialogue for the trade
            checkConditionsForCurrI();
            startDialogue(Dialogues);
        }
        else
        {
            failTrade = true;
            startDialogue(Dialogues);
        }
    }

    public void setLoop(int dialogueToLoop)
    {
        loopDialogueIndex = dialogueToLoop;
    }

    public void stopLoop()
    {
            loopDialogueIndex = -1;
    }
    
}