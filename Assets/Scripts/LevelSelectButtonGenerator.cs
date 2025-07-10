using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelSelectButtonGenerator : MonoBehaviour
{
    [SerializeField] private LevelLoader loader;
    [SerializeField] private UITweener tweener;

    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private int levelStartIndex;

    private void Start()
    {
        int numLevels = SceneManager.sceneCountInBuildSettings - levelStartIndex;
        GameObject[] buttons = new GameObject[numLevels];

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i] = Instantiate(buttonPrefab, transform);
            buttons[i].GetComponentInChildren<TMP_Text>().text = (i + 1).ToString();

            int id = i;
            buttons[i].GetComponent<Button>().onClick.AddListener(() =>
            {
                tweener.ButtonPop(buttons[id]);
                loader.LoadLevel(id + levelStartIndex);
            });
        }
    }
}
