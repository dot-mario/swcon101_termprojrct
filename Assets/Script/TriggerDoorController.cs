using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoorController : MonoBehaviour
{
    [SerializeField] Animator myDoor = null;
    [SerializeField] bool openTrigger = false;
    [SerializeField] bool closeTrigger = false;

    public void OpenDoor()
    {
        myDoor = GetComponent<Animator>();
        myDoor.Play("DoorOpen", 0, 0.0f);
    }
}
