using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public float transitionTime = 1f;

    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevelCoroutine(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void LoadLevel(int levelIndex)
    {
        StartCoroutine(LoadLevelCoroutine(levelIndex));
    }

    public void ResetLevel()
    {
        StartCoroutine(LoadLevelCoroutine(SceneManager.GetActiveScene().buildIndex));
    }

    public IEnumerator LoadLevelCoroutine(int index)
    {
        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(index);
    }
}
