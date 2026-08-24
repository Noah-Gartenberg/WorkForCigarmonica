using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CameraChange : MonoBehaviour
{
    //need to give it the camera instance
    public Camera newCamera;
    //Need to attach this to a game object with a box collider in it, with trigger volume checked
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if(Camera.main != newCamera)
            {
                Camera oldCamera = Camera.main;

                newCamera.gameObject.SetActive(true);
                newCamera.enabled = true;

                oldCamera.enabled = false;
                oldCamera.gameObject.SetActive(false);
            }
        }
    }
}
