//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PointAndClickMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public bool canMove = true;
    public bool interacting = false;
    public float capsuleRadius;
    //private int countTest = 0;

    //checks if an object is interactable
    private bool canInteract;
    //where the input will try to move
    private Vector3 targetPosition;
    private bool isMoving;
    private IClickable clickableObject = null;
    public CharacterController characterController;
    public SpriteRenderer mesh;
    /**
     * Executes once per frame, main code for movement
     */
    LayerMask mask;
    private void Start()
    {
         mask = LayerMask.GetMask("Default", "UI");
    }
    void Update()
    {
        //first do raycast
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray: ray, hitInfo: out RaycastHit hit, mask);
        if (isMoving)
        {
            //get active camera and turn it towards the player
            Camera.main.transform.LookAt(transform.position);
            MoveCharacter();
        }
        //testing trading
        /*if(Input.GetKeyDown(KeyCode.R))
        {
            if(interacting && clickableObject != null && clickableObject.GetType() == typeof(NPCtry2))
            {
                ((NPCtry2)clickableObject).checkForDesiredItem();
            }
        }*/
        //if you want to do anything that is tied to the raycast, before movement/click event, do it here
        if (Input.GetMouseButtonDown(0))
        {
            if (canMove)
            {
                if (hit.collider != null)
                {
                    targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                    targetPosition -= (targetPosition - transform.position).normalized * capsuleRadius;
                    if(hit.collider.gameObject.tag == "Door")  // Check if the clicked object is a Door
                    {
                        hit.collider.gameObject.GetComponent<IClickable>().Interact();
                        return;
                    }
                   if (hit.collider.gameObject.TryGetComponent(out IClickable clickableObject) )
                    {
                        canInteract = true;
                        this.clickableObject = clickableObject;
                    }
                    else if (canInteract && !interacting)
                    {
                        //if the object this raycast hits isn't a clickable object, then make sure clickableObject is null and can interact is false
                        canInteract = false;
                        Debug.Log("set clickableObject to null");
                        clickableObject = null;
                    }
                    setIsMoving(true);
                }
                else
                {
                    targetPosition = new Vector3(ray.direction.x * 100, transform.position.y, ray.direction.z * 10000);
                    if (canInteract && !interacting)
                    {
                        canInteract = false;
                        clickableObject = null;
                    }
                    setIsMoving(true);
                }
            }
            else if (interacting)
            {
                //ending interaction only comes from dialogue code - what I need to do!!!!
                clickableObject.Interact();
            }

        }
    }



    /**
     * Method moves character to point or as close as possible
     * param: the point that the raycast hit
     */
    private void MoveCharacter()
    {
        if((transform.position - targetPosition).magnitude > capsuleRadius)
        {
            Vector3 direction = targetPosition - transform.position;
            if(Vector3.Dot(Camera.main.transform.forward,direction) >= 0)
            {
                mesh.flipX = true;
            }
            else
            {
                mesh.flipX = false;
            }
            mesh.transform.LookAt(new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z));
            Vector3 movement = direction.normalized * moveSpeed * Time.deltaTime;
            Debug.Log("Setting direction and movement");
            characterController.Move(movement);
            //use dotproducs for movement direction and right vector of camera to rotate character: right and forwards turns character to right facing, left and backwards turns character left facing
        }   
        else
        {
            setIsMoving(false);
            if (canInteract)
            {
                Debug.Log("To interaction");
                //Debug.Log("Setting Interacting to true");
                StartDialogue();
                //countTest++;
                //Debug.Log(countTest + " done");
            }
        }
    }

    public void setIsMoving(bool newVal)
    {
        isMoving = newVal;
    }

    public void setCanMove(bool newVal)
    {
        canMove = newVal;
    }

    public void endDialogue()
    {
        interacting = false;
        canInteract = false;
        canMove = true;

        if (clickableObject != null && clickableObject.GetType() == typeof(NPCtry2))
            ((NPCtry2)clickableObject).endDialogue();
        clickableObject = null;
    }

    public void StartDialogue()
    {
        Debug.Log("Trying to start dialouge");
        interacting = true;
        canMove = false;
        //Debug.Log("Going to next line");
        
        if (clickableObject.GetType() == typeof(NPCtry2))
        {
            ((NPCtry2)clickableObject).Interact();
        }
            

        
    }

    public void setNPCCondition(int index, bool condition)
    {
        if(clickableObject != null && clickableObject.GetType() == typeof(NPCtry2))
        {
            ((NPCtry2)clickableObject).setCondition(index, condition);
        }
    }

    public void setLoop(int dialogueToLoop)
    {
        if (clickableObject != null && clickableObject.GetType() == typeof(NPCtry2))
        {
            ((NPCtry2)clickableObject).setLoop(dialogueToLoop);
        }
    }

    public void stopLoop()
    {
        if (clickableObject != null && clickableObject.GetType() == typeof(NPCtry2))
        {
            ((NPCtry2)clickableObject).stopLoop();
        }
    }
    
}

