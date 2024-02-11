using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum EPlayerMoveState : int
{
    STATE_IDLE = 0,
    STATE_RUN,
    STATE_WALK,
    MOVE_STATE_FORCE_DWORD,
}

public enum EPlayerBodyState : int
{
    BODY_STATE_IDLE = 0,
    BODY_STATE_CROUCH,
    BODY_STATE_JUMP,
    BODY_STATE_PRONE,
    BODY_STATE_FORCE_DWORD,
}

public class HL_PlayerController : MonoBehaviour
{
    public GameObject GameplayObject;
    private HL_UserInterface UI;
    private HL_KeyState KeyStates;

    public Transform camFirstPerson;
    public Transform camThirdPerson;
    public Transform modelLocalPlayer;
    public Transform modelView;

    private CharacterController characterController;

    public float flSensitivity = 5.0f;
    public float jumpHeight = 2.0f;

    private Quaternion quatViewModelInitialPosition = Quaternion.identity;


    private Vector3 vecMouseMoveDelta = Vector3.zero;
    private Vector3 vecKeyboardMoveDelta = Vector3.zero;
    private Vector3 vecViewAngles = Vector3.zero;
    private float flCurrentPlayerSpeed = 0.0f;

    EPlayerMoveState moveState = EPlayerMoveState.STATE_IDLE;
    EPlayerBodyState bodyState = EPlayerBodyState.BODY_STATE_IDLE;
    EPlayerMoveState moveStateLast = EPlayerMoveState.STATE_IDLE;
    EPlayerBodyState bodyStateLast = EPlayerBodyState.BODY_STATE_IDLE;

    void Start()
    {
        if (GameplayObject == null)
            GameplayObject = GameObject.Find("GameplayObject");

        KeyStates = GameplayObject.GetComponent<HL_KeyState>();
        UI = GameplayObject.GetComponent<HL_UserInterface>();

        if (modelLocalPlayer == null)
            modelLocalPlayer = transform.Find("PlayerModel");

        if (camFirstPerson == null)
            camFirstPerson = modelLocalPlayer.Find("FirstPersonCamera");

        if (camThirdPerson == null)
            camThirdPerson = modelLocalPlayer.Find("ThirdPersonCamera");

        if (modelView == null)
            modelView = camFirstPerson.Find("ViewModel");

        characterController = modelLocalPlayer.GetComponent<CharacterController>();

        quatViewModelInitialPosition = modelView.transform.localRotation;
    }

    void Update()
    {
        if (UI.bMenuPaused)
            return;

        HandleInput();
        UpdateMoveState();
        ApplyMovement();
    }

    float GetPlayerMoveSpeedModifier()
    {

        if (moveState == (int)EPlayerMoveState.STATE_IDLE)
            return 0.0f;

        float flMoveSpeed = 0.0f;

        switch (moveState)
        {
            case EPlayerMoveState.STATE_RUN:
                {
                    switch (bodyState)
                    {
                        case EPlayerBodyState.BODY_STATE_CROUCH:
                            {
                                flMoveSpeed = 1.5f;
                                break;
                            }
                        case EPlayerBodyState.BODY_STATE_PRONE:
                            {
                                flMoveSpeed = 1.0f;
                                break;
                            }
                        default:
                            {
                                flMoveSpeed = 2.5f;
                                break;
                            }

                    }

                    break;
                }

            case EPlayerMoveState.STATE_WALK:
                {

                    switch (bodyState)
                    {
                        case EPlayerBodyState.BODY_STATE_CROUCH:
                            {
                                flMoveSpeed = 1.0f;
                                break;
                            }
                        case EPlayerBodyState.BODY_STATE_PRONE:
                            {
                                flMoveSpeed = 0.5f;
                                break;
                            }
                        default:
                            {
                                flMoveSpeed = 1.5f;
                                break;
                            }

                    }

                    break;
                }
            default:
                {
                    flMoveSpeed = 1.5f;
                    break;
                }

        }

        return flMoveSpeed;

    }
    float GetPlayerMaxMoveSpeed()
    {
        float flSpeedModifier = GetPlayerMoveSpeedModifier();

        float MaxPlayerSpeed = 5.0f;   

        return flSpeedModifier * MaxPlayerSpeed;
    }

    bool bSprintingKeyState = false;
    bool bCrouchingKeyState = false;
    bool bJumpKeyState = false;
    bool bProneKeyState = false;
    void UpdateMoveState()
    {
        bSprintingKeyState = KeyStates.CheckKeyState(KeyCode.LeftShift, EKeyQueryMode.KEYQUERY_ONHOTKEY);
        bCrouchingKeyState = KeyStates.CheckKeyState(KeyCode.LeftControl, EKeyQueryMode.KEYQUERY_ONHOTKEY);
        bJumpKeyState = KeyStates.CheckKeyState(KeyCode.Space, EKeyQueryMode.KEYQUERY_ONHOTKEY);
        bProneKeyState = KeyStates.CheckKeyState(KeyCode.Z, EKeyQueryMode.KEYQUERY_ONHOTKEY);

        moveStateLast = moveState;
        bodyStateLast = bodyState;

        if (bCrouchingKeyState)
        {
            bodyState = EPlayerBodyState.BODY_STATE_CROUCH;
            bJumpKeyState = false;
            bProneKeyState = false;
        }
        else if (bJumpKeyState)
        {
            bodyState = EPlayerBodyState.BODY_STATE_JUMP;
            bCrouchingKeyState = false;
            bProneKeyState = false;
        }
        else if (bProneKeyState)
        {
            bodyState = EPlayerBodyState.BODY_STATE_PRONE;
            bCrouchingKeyState = false;
            bJumpKeyState = false;
        }
        else
            bodyState = EPlayerBodyState.BODY_STATE_IDLE;

        if (vecKeyboardMoveDelta.y != 0.0f || vecKeyboardMoveDelta.x != 0.0f)
        {
            if (bSprintingKeyState)
                moveState = EPlayerMoveState.STATE_RUN;
            else
                moveState = EPlayerMoveState.STATE_WALK;
        }
        else
        {
            moveState = EPlayerMoveState.STATE_IDLE;
        }

        flCurrentPlayerSpeed = GetPlayerMaxMoveSpeed();
    }

    void HandleInput()
    {
        vecMouseMoveDelta.x = Input.GetAxisRaw("Mouse X") * flSensitivity;
        vecMouseMoveDelta.y = Input.GetAxisRaw("Mouse Y") * flSensitivity;

        vecKeyboardMoveDelta.y = Input.GetAxisRaw("Vertical");
        vecKeyboardMoveDelta.x = Input.GetAxisRaw("Horizontal");

    }

    private bool isGrounded;
    private float verticalSpeed = 0.0f;
    void ApplyMovement()
    {
        isGrounded = characterController.isGrounded;
        if (isGrounded && verticalSpeed < 0)
        {
            verticalSpeed = -2f;
        }

        vecViewAngles.x -= vecMouseMoveDelta.y;
        vecViewAngles.y += vecMouseMoveDelta.x;

        vecViewAngles.x = Mathf.Clamp(vecViewAngles.x, -90.0f, 90.0f);

        camFirstPerson.localRotation = Quaternion.Euler(vecViewAngles.x, 0, 0);
        modelLocalPlayer.localRotation = Quaternion.Euler(0, vecViewAngles.y, 0);

        Vector3 moveDirection = modelLocalPlayer.transform.forward * vecKeyboardMoveDelta.y + modelLocalPlayer.transform.right * vecKeyboardMoveDelta.x;
        moveDirection *= flCurrentPlayerSpeed * Time.deltaTime;

        verticalSpeed += Physics.gravity.y * Time.deltaTime;

        if (bJumpKeyState && isGrounded)
        {
            verticalSpeed = Mathf.Sqrt(2 * jumpHeight * -Physics.gravity.y);
        }

        moveDirection.y = verticalSpeed * Time.deltaTime; 

        characterController.Move(moveDirection);
    }


}
