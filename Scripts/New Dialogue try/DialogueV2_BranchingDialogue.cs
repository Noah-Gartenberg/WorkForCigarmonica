using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BranchingDialogue : DialogueV2
{
    //This class provides the functionality for the branching dialogue - generally will be
    [TextArea]
    public string prompt;

    public answer[] answers;

    public answer getAnswer(int index) { return answers[index]; }
    public string getPrompt() { return  prompt; }
    

}


[System.Serializable]
public struct answer
{//Contains data for an answer struct
    [TextArea]
    public string answerChoice;
    public DialogueV2 transitionDialogueSection;
    public int nextSection;

    public string getAnswerChoice() { return answerChoice; }
    //Basically just says "hey, what's the transition dialogue for this section, and if there isn't one, it will return null - otherwise it returns the section
    public DialogueV2 getTransitionDialogueSection() {
        if(transitionDialogueSection == null)
        {
            transitionDialogueSection = null;
        }
        
        return transitionDialogueSection;
    }
    //The next section from one's array to be read - input the index of that section - will be reaad after any transition dialogue if there is any
    public int getNextSection() { return nextSection; }

    
}