using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLevel : MonoBehaviour
{
    public void SetGameRunning()
    {
        GameManager.Instance.SetGameRunning(true);
    }
}
