using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsPanel : MonoBehaviour
{
    public GameObject panel;

    private bool isActive;

    public void ToggleSettingsActive()
    {
        isActive = !isActive;
        panel.SetActive(isActive);

        if (isActive)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
