using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class isn't made to be used. Use one of its sub classes
[CreateAssetMenu]
public abstract class DialogueV2 : ScriptableObject
{
    [TextArea] //use dialogue array for ending, repeating, or calling any functions based on what's in it - therefore I have reason to use it in other classes
    [SerializeField] string[] dialogue;
    /*[SerializeField] bool shouldGiveQuest = false;

    //This doesn't do too much right now/at all - it's only goal is to give its owner a tag that is along the lines of "has quest" so that way we can have a condition for that in the dialogue
    public void giveQuest()
    {
        if(shouldGiveQuest)
        {
            //do something
        }
    }
    
    //This doesn't do anything either - its goal is going to be to give the owner a completed quest tag, remove the has quest tag, and switch out any items into/out of the player's inventory
    public void completeQuest()
    {
        //Do something
        //only do this if the parent object has tag has quest or whatever
    }*/ //none of these commented out lines are necessary and can theoretically be deleted, but I want to comment them out for the time being
    //basic getter method for individual lines
    public string getDialogue(int index)
    {
        return dialogue[index];
    }

    public string[] getDialogueArray()
    {
        return dialogue;
    }
    //This line returns the last line in the section which will tell the system which seciton to go to next and stuff/what to do next (repeat, loop, end, next section, etc...)
    public string getLastLine()
    {
        if(dialogue.Length != 0)
        {
            return dialogue[dialogue.Length - 1];
        }
        else
        { return null; }
        
    }

}