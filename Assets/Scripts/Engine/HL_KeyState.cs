using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EKeyQueryMode
{
    KEYQUERY_SINGLEPRESS = 0,
    KEYQUERY_ONHOTKEY,
    KEYQUERY_OFFHOTKEY,
    KEYQUERY_TOGGLE,
    KEYQUERY_FORCEDWORD,
}

public class HL_KeyState : MonoBehaviour
{
    const int MaxKeys = (int)KeyCode.Joystick8Button19 + 1;

    bool[] ToggleState;
    bool[] CurrentState;
    bool[] SinglePressState;
    void Start()
    {
        ToggleState = new bool[MaxKeys];
        CurrentState = new bool[MaxKeys];
        SinglePressState = new bool[MaxKeys];
    }

    public bool CheckKeyState(KeyCode Code, EKeyQueryMode Mode = EKeyQueryMode.KEYQUERY_ONHOTKEY)
    {
        if ((int)Code >= MaxKeys)
        {
            Debug.Log("Unsupported key " + (int)Code);
            return false;
        }

        switch (Mode)
        {
            case EKeyQueryMode.KEYQUERY_SINGLEPRESS:
                return SinglePressState[(int)Code];
            case EKeyQueryMode.KEYQUERY_ONHOTKEY:
                return CurrentState[(int)Code];
            case EKeyQueryMode.KEYQUERY_OFFHOTKEY:
                return !CurrentState[(int)Code];
            case EKeyQueryMode.KEYQUERY_TOGGLE:
                return ToggleState[(int)Code];
            default:
                return false;
        }

    }
    void Update()
    {
        for (int Current = 0; Current < ToggleState.Length; Current++)
        {
            CurrentState[Current] = Input.GetKey((KeyCode)Current);
            SinglePressState[Current] = Input.GetKeyDown((KeyCode)Current);

            if (SinglePressState[Current])
                ToggleState[Current] = !ToggleState[Current];

        }
    }
}
