using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HL_DoorOpenClose : MonoBehaviour
{
    public GameObject GameplayObject;

    private HL_UserInterface UI;
    private HL_KeyState KeyStates;
    private Animator baseAnimator = null;

    private bool bOpened = false;

    void Start()
    {
        baseAnimator = GetComponent<Animator>();

        if (GameplayObject == null)
            GameplayObject = GameObject.Find("GameplayObject");

        KeyStates = GameplayObject.GetComponent<HL_KeyState>();
        UI = GameplayObject.GetComponent<HL_UserInterface>();

        //baseAnimator.enabled = false;
    }
    void Update()
    {

        if (KeyStates.CheckKeyState(KeyCode.Y, EKeyQueryMode.KEYQUERY_SINGLEPRESS))
        {
            if (!bOpened)
            {

                    baseAnimator.Play("OpenDoor");
                    bOpened = true;

            }
            else
            {
                    baseAnimator.Play("CloseDoor");
                    bOpened = false;
            }
            
        }

    }
}
