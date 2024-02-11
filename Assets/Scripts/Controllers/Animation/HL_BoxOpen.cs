using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class  HL_BoxOpen : MonoBehaviour
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

        if (KeyStates.CheckKeyState(KeyCode.T, EKeyQueryMode.KEYQUERY_SINGLEPRESS))
        {
            AnimatorStateInfo State = baseAnimator.GetCurrentAnimatorStateInfo(0);
            if (State.shortNameHash == Animator.StringToHash("Idle") || State.shortNameHash == Animator.StringToHash("OpenedState"))
            {
                if (!bOpened)
                {

                    baseAnimator.Play("BoxOpen");
                    bOpened = true;

                }
                else
                {
                    baseAnimator.Play("BoxClose");
                    bOpened = false;
                }
            }
        }

        /*
        AnimatorStateInfo State = baseAnimator.GetCurrentAnimatorStateInfo(0);

        if (State.shortNameHash == Animator.StringToHash("YourStateName"))
        {


          //  Debug.Log("Current State is: YourStateName");
        }*/

        //  Debug.Log("Current State Hash: " + State.nameHash.ToString());

    }
}
