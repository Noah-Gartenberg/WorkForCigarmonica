using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Collections.Specialized.BitVector32;

public class DialogueManagerV2 : MonoBehaviour
{
     //universal
    public static DialogueManagerV2 Instance;
    public bool OnByDefault = true;

    //should specifically be a reference to the player's character, 
    //included as a pointAndClickMove because I didn't do more casting than I absolutely had ot
    public PointAndClickMove playerReference ; 

    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] GameObject dialogueBox;
    //these only apply to the branching dialogue
    [SerializeField] GameObject answerBox;
    [SerializeField] Button[] answerObjects;
    public NPCtry2 currentNPC;

    //DialogueV2 Looping = null;
    bool nextLine;
    bool answering = false;
    int answerChosen = -1;


    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject); 
            Instance = this;
        }
        // else if (Instance != this)
        // {
        //     Destroy(gameObject);
        //     return;
        // }
    }
    //set to off if not being used? maybe that will avoid null reference errors?
    public static event Action OnDialogueStarted;
    public static event Action OnDialogueEnded;
    public void Start()
    {
        OnDialogueEnded += EndDialogue;
        if (!OnByDefault)
        {
            dialogueBox.SetActive(false);
        }
    }

    private void EndDialogue()
    {
        //Debug.Log("This is running");
        playerReference.endDialogue();
        ResetBox();
        HideDialogue();
        //setting currentNPC to null - hopefully won't break anything
        currentNPC = null;
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

    //overloaded method call is for specific cases where we need to play singular sections, without sending a whole array
    public void StartDialogue(DialogueV2 dialogue, int startLine, string name)
    {
        ResetBox();
        setNameText(name);
        dialogueBox.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(RunDialogue(dialogue, startLine));
    }
    //General use method call for entering dialogue
    public void StartDialogue(DialogueV2[] dialogue, int startSection, string name)
    {
        ResetBox();
        setNameText(name);
        dialogueBox.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(RunDialogue(dialogue, startSection));
    }


    private void setNameText(string name)
    {
        if(name != null && !name.Equals("empty"))
        {
            nameText.text = name + "";
        }
        else
        {
            nameText.text = "";
        }
    }

    //overloaded so I can also call singular dialogue interactions, and not just multi-array ones
    //only for linear dialogue
    //specifically for the transitional dialogue
    IEnumerator RunDialogue(DialogueV2 dialogue, int start)
    {
        for(int k = start; k < dialogue.getDialogueArray().Length - 1; k++)
        {
            if (!dialogue.getDialogue(k).Equals("SKIP") && dialogue.getDialogue(k).IndexOf("COMMANDS") == -1)
            {
                ShowDialogue(dialogue.getDialogue(k));
            }
            else if(dialogue.getDialogue(k).IndexOf("COMMANDS") != -1)
            {

                string[] commandArray = dialogue.getDialogue(k).Split(' ');
                int i = 1;
                while (i < commandArray.Length)
                {
                    switch (commandArray[i])
                    {
                        case "CHANGE_NAME":
                            i++;
                            nameText.text = "";
                            if (commandArray[i].IndexOf("_") != -1)
                            {
                                for (int j = 0; j < commandArray[i].Length; j++)
                                {
                                    if (commandArray[i].Substring(j, 1).Equals("_"))
                                    {
                                        nameText.text += " ";
                                    }
                                    else
                                    {
                                        nameText.text += commandArray[i].Substring(j,1);
                                    }
                                }
                            }
                            else
                            {
                                nameText.text = commandArray[i];
                            }
                            nameText.ForceMeshUpdate();
                            i++;
                            break;
                        case "SET_CONDITION":
                            i++;
                            int index = Int32.Parse(commandArray[i]);
                            i++;
                            bool condition = bool.Parse(commandArray[i]);
                            playerReference.setNPCCondition(index, condition);
                            resetLooping();
                            break;
                        case "DEBUG":
                            i++;
                            //a case to debug text that isn't working
                            Debug.Log(commandArray[i]);
                            break;
                        case "TRY_TRADE":
                        if (!CheckIfItemIsActive(currentNPC) && currentNPC.tradeItem != null){
                            currentNPC.tradeItem.SetActive(true);
                        }else{
                            Debug.Log("Another NPC Has The Item");
                        }
                        break;
                    }
                    i++;
                }
                continue;
            }
            else
            {//If the line should be skipped (for whatever reason) (specifically for transitional dialogue), then skip it
                //this is to ensure that there are no null references which were causing issues
                break;
            }
            while (!nextLine)
            {
                yield return null;
            }
            nextLine = false;
        }
        checkForCommands(dialogue.getLastLine());
    }
    IEnumerator RunDialogue(DialogueV2[] dialogue, int startSection)
    {
        nextLine = false;
        OnDialogueStarted?.Invoke();
        
        for(int section = startSection; section < dialogue.Length; section++)
        {
            if(!nameText.IsActive())
            {
                nameText.gameObject.SetActive(true);
            }
            //need to do a check here if the type of dialogue for each section is linear or branching
            //then do a nested for loop for each one
            if (dialogue[section].GetType() == typeof(LinearDialogue))
            {
                //should be minus one because the very last line would be what tells the dialogue to 
                //stop or keep going.
                for (int line = 0; line < dialogue[section].getDialogueArray().Length - 1 ; line++)
                {
                    
                    string theLine = dialogue[section].getDialogue(line);
                    //Debug.Log(theLine);
                    //before showing any dialogue, need to check that line, and see if it has any commands in it.
                    //check for commands
                    if(theLine.IndexOf("COMMANDS")!= -1)
                    {
                        string[] commandArray = theLine.Split(' ');
                        int i = 1;
                        while (i < commandArray.Length)
                        {
                            switch (commandArray[i])
                            {
                                case "CHANGE_NAME":
                                    i++;
                                    nameText.text = "";
                                    if (commandArray[i].IndexOf("_") != -1)
                                    {
                                        for (int j = 0; j < commandArray[i].Length; j++)
                                        {
                                            if (commandArray[i].Substring(j, 1).Equals("_"))
                                            {
                                                nameText.text += " ";
                                            }
                                            else
                                            {
                                                nameText.text += commandArray[i].Substring(j, 1);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        nameText.text = commandArray[i];
                                    }
                                    nameText.ForceMeshUpdate();
                                    i++;
                                    break;
                                case "SET_CONDITION":
                                    i++;
                                    int index = Int32.Parse(commandArray[i]);
                                    i++;
                                    bool condition = bool.Parse(commandArray[i]);
                                    playerReference.setNPCCondition(index, condition);
                                    resetLooping();
                                    break;
                                case "DEBUG":
                                    i++;
                                    //a case to debug text that isn't working
                                    Debug.Log(commandArray[i]);
                                    break;
                                case "TRY_TRADE":
                                    if(currentNPC != null){
                                        currentNPC.TryActivateTradeItem();
                                    } else {
                                        Debug.Log("No NPC set");
                                    }
                                    break;
                                case "SET_ACTIVITY":
                                    i++;
                                    string tag = commandArray[i++];
                                    GameObject[] meshes = GameObject.FindGameObjectsWithTag(tag);
                                    for(int j =  0; j < meshes.Length; j++)
                                    {
                                        meshes[j].GetComponent<SpriteRenderer>().enabled = Boolean.Parse(commandArray[i]);
                                    }
                                    break;
                                    
                                case "PLAY":
                                    string AudioTag = commandArray[++i];
                                    AudioSource Harmonica = GameObject.FindGameObjectWithTag(AudioTag).gameObject.GetComponent<AudioSource>();
                                    Harmonica.Play();
                                    break;
                                case "WAIT_FOR":
                                    float seconds = float.Parse(commandArray[++i]);
                                    yield return new WaitForSeconds(seconds);
                                    break;
                                case "LOADSCENE":
                                    SceneManager.LoadScene(commandArray[i++]);
                                    break;
                                case "DISABLE":
                                    i++;
                                    string theTag = commandArray[i++];
                                    GameObject[] theMeshes = GameObject.FindGameObjectsWithTag(theTag);
                                    for (int j = 0; j < theMeshes.Length; j++)
                                    {
                                        theMeshes[j].gameObject.SetActive(false);
                                    }
                                    break;
                            }
                            i++;
                        }
                        continue;
                    }
                    else
                    {
                        ShowDialogue(theLine);
                        while (!nextLine)
                        {
                            //wait until the player clicks
                            yield return null;
                        }
                        nextLine = false;
                    }
                }

                //make any sets or resets necessary here
          
            }
            else
            {
                //nameText.gameObject.SetActive(false);
                //Debug.Log("This works");
                if(((BranchingDialogue)dialogue[section]).getDialogueArray().Length > 1)
                {
                    yield return StartCoroutine(RunDialogue(dialogue[section], 0));
                }
                //changed to make sure that if there is dialgoue to play before the branch, then it gets played
                //this change had to be made so that players would be given the option to trade with all npcs, not just the one they should trade with
                ShowDialogue(((BranchingDialogue)dialogue[section]).getPrompt());
                ShowAnswers(((BranchingDialogue)dialogue[section]));
                answering = true;
                while (answerChosen < 0)
                {
                    yield return null;
                }
                answering = false;
                answerBox.SetActive(false);
                yield return null;
                if (((BranchingDialogue)dialogue[section]).getAnswer(answerChosen).getTransitionDialogueSection() != null)
                {
                    //transition dialogue shouldn't stop, loop, or anything like that - it also shoudln't change anything - if it needs to be done there, it can be done after
                    //remember people, illusion of free will
                    yield return StartCoroutine(RunDialogue(((BranchingDialogue)dialogue[section]).getAnswer(answerChosen).getTransitionDialogueSection(), 0));
                    //also this should only be for linear dialogue (I can make it for branching dialogue I guess, but it should be fine without that I think
                }
                //send chosen answer to player if necessary (probably won't be though)
                answerChosen = -1;

                //if we're gonna have truly branching conversations (which I don't think we will)
                //then the next section integer is implemented in the answer struct
                //but like I said, I don't think we will

            }
            checkForCommands(dialogue[section].getLastLine());
        }
    }
    public void SetCurrentNPC(NPCtry2 npc) {
        currentNPC = npc;
    }
    private bool CheckIfItemIsActive(NPCtry2 currentNpc)
    {
        foreach (var npc in FindObjectsOfType<NPCtry2>()){
            if (npc.tradeItem == null) {
                Debug.LogWarning("NPC " + npc.NPCname + " does not have a tradeItem assigned.");
                continue; 
            }
            if (npc != currentNpc && npc.tradeItem.activeSelf)
            {
                Debug.Log("Active item found with NPC: " + npc.NPCname);
                return true;
            }
        }
    return false;
    }

    private void ShowAnswers(BranchingDialogue Branch)
    {
        answerBox.SetActive(true);
        for(int i = 0; i < answerObjects.Length; i++)
        {
            if (i >= Branch.answers.Length)
            {
                answerObjects[i].gameObject.SetActive(false);
            }
            else
            {
                answerObjects[i].GetComponentInChildren<TextMeshProUGUI>().text = Branch.getAnswer(i).getAnswerChoice();
                answerObjects[i].gameObject.SetActive(true);
            }
            
        }
        
    }

    public void AnswerQuestion(int answer)
    {
        answerChosen = answer;
    }

    //at end of the dialogue section, check to see whether or not to go onto the next dialogue and which commands to do
    private void checkForCommands(string lastLine)
    {
        //if it's not null, but no conditions met, it'll be like it was null
        if(!lastLine.Equals(""))
        {
            int i = 0;
            string[] commands = lastLine.Split(' ');
            while (i < commands.Length)
            {
                switch (commands[i])
                {
                    case "END":
                        OnDialogueEnded?.Invoke();
                        break;
                    case "TRY_TRADE":
                        //check player character's inventory and take from it if possible, unless player lacks the item
                        //if player lacks item, call runDialogue
                        break;
                    case "GIVE_ITEM":
                        //give player item in inventory
                        //maybe add any dialogue if necessary
                        //how?
                        //make trade exit dialogue, and then re-enter it
                        break;
                    case "SET_CONDITION":
                        i++;
                        int index = Int32.Parse(commands[i]);
                        i++;
                        bool condition = bool.Parse(commands[i]);
                        playerReference.setNPCCondition(index, condition);
                        //goal for this one would be to set a condition in the npc being spoken to through use of a method in
                        //the player's class
                        //next two indices in commands would contain (respectively) a) the index of the condition 
                        //(array of booleans or strings in npc in question) and b) what to set it to
                        //move i forwards two
                        //resets looping because a condition changed, and therefore looping may not be necessary anymore
                        resetLooping();
                        break;
                    case "LOOP":
                        //loops that specific dialogue section
                        //add looping to npc character - call method here, not there idiot
                        //Looping = source;
                        i++;
                        playerReference.setLoop(Int32.Parse(commands[i]));
                        break;
                    case "CHANGE_NAME":
                        i++;
                        //for this one, put the new name in the next word
                        //need to test if this breaks if I put three spaces in a row
                        //also need to test if this breaks if I put quotes
                        nameText.text = "";
                        if (commands[i].IndexOf("_") != -1)
                        {
                            for (int j = 0; j < commands[i].Length; j++)
                            {
                                if (commands[i].Substring(j, 1).Equals("_"))
                                {
                                    nameText.text += " ";
                                }
                                else
                                {
                                    nameText.text += commands[i].Substring(j, 1);
                                }
                            }
                        }
                        else
                        {
                            nameText.text = commands[i];
                        }

                        break;
                    case "DEBUG":
                        //a case to debug text that isn't working
                        Debug.Log(commands[i++]);
                        break;
                    case "LOADSCENE":
                        SceneManager.LoadScene(commands[i++]);
                        break;
                }
                i++;
            }
        }
    }

    void ResetBox()
    {
        StopAllCoroutines();
        dialogueBox.SetActive(false);
        answerBox.SetActive(false);
        nextLine = false;
        answerChosen = -1;
        answering = false;
    }

    public void ToNextLine()
    {
        if(!answering)
        {
            nextLine = true;
        }
    }

    private void resetLooping()
    {
        playerReference.stopLoop();
        //instead make call to character and have it set looping to null
        //Looping = null;
    }
}