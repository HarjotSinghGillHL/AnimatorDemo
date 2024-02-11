using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HL_UserInterface : MonoBehaviour
{
    private HL_KeyState KeyStates = null;

    [HideInInspector]
    public bool bMenuPaused = false;

    [HideInInspector]
    public float flTimeScaleToAdjust = 0.0f;

    GUIStyle guiStylePaused = null;
    GUIStyle guiStyleIndicators = null;

    void Start()
    {
        KeyStates = GetComponent<HL_KeyState>();
        flTimeScaleToAdjust = Time.timeScale;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        if (KeyStates.CheckKeyState(KeyCode.Escape, EKeyQueryMode.KEYQUERY_SINGLEPRESS))
        {
            bMenuPaused = !bMenuPaused;
            if (bMenuPaused)
            {
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0.0f;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = flTimeScaleToAdjust;
            }

        }
    }

    void OnGUI()
    {
        if (guiStylePaused == null)
        {
            guiStylePaused = new GUIStyle(GUI.skin.label);
            guiStylePaused.fontSize = 60;
            guiStylePaused.alignment = TextAnchor.MiddleCenter;

        }

        if (guiStyleIndicators == null)
        {
            guiStyleIndicators = new GUIStyle(GUI.skin.label);
            guiStyleIndicators.fontSize = 20;
        }

        if (bMenuPaused)
        {
            Rect rect_ = new Rect(Screen.width / 2 - 100, Screen.height / 2 - 25, 300, 100);
            GUI.Label(rect_, "PAUSED", guiStylePaused);
        }

        int Pad = 10;

        Rect rect = new Rect(10, Pad, 300, 30);
        GUI.Label(rect, "Movement keys : WASD", guiStyleIndicators);

        Pad += 30;

        rect = new Rect(10, Pad, 300, 30);
        GUI.Label(rect, "Sprint : Shift", guiStyleIndicators);

        Pad += 30;

        rect = new Rect(10, Pad, 300, 30);
        GUI.Label(rect, "Jump : Space", guiStyleIndicators);

        Pad += 30;

        rect = new Rect(10, Pad, 300, 30);
        GUI.Label(rect, "Pause : Escape", guiStyleIndicators);

    }

}
