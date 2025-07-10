using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    #region Singleton
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("There are multiple instances of 'GameManager'!");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }
    #endregion

    public static int numLevels = 100;
    public int Level;
    public float TimeRequirement = 20f;

    [Space]

    private float[] bestTimes;

    public Vector2 playerStartPosition;
    public float playerResetControlDelay = 0.2f;
    public Transform playerDeathParticles;

    [Header("UI")]
    public GameObject levelPassedUI;
    public GameObject levelFailedUI;

    [Header("Text Elements")]
    public TMP_Text levelPassedTimeText;
    public TMP_Text levelFailedTimeText;
    public TMP_Text timerText;
    public TMP_Text lowestTimeText;
    public TMP_Text timeRequirementText;
    public TMP_Text levelNameText;

    private PlayerController m_player;
    private bool m_gameRunning;
    private float m_gameTimer;
    private float m_lowestTime;
    private float m_playerResetControlDelay;

    public delegate void EventHandler();
    public event EventHandler onPlayerReset;

    private void Start()
    {
        bestTimes = SaveSystem.LoadScores();
        if (bestTimes == null)
        {
            bestTimes = new float[numLevels];

            for (int i = 0; i < bestTimes.Length; i++)
            {
                bestTimes[i] = Mathf.Infinity;
            }

            SaveSystem.SaveScores(bestTimes);
        }

        levelPassedUI.SetActive(false);

        // Initiate Player
        m_player = GameObject.Find("Player").GetComponent<PlayerController>();
        m_player.canControl = false;
        m_playerResetControlDelay = playerResetControlDelay;

        // Initiate Timer
        m_lowestTime = bestTimes[Level];
        lowestTimeText.text = m_lowestTime == Mathf.Infinity ? "" : m_lowestTime.ToString("F2");

        // Set Time Requirement Text
        timeRequirementText.text = "REQ: " + TimeRequirement.ToString("F2") + "s";

        levelNameText.text = "Level " + Level.ToString();
    }

    public IEnumerator ResetPlayer()
    {
        if (m_player == null) yield return null;

        m_gameRunning = false;

        Instantiate(playerDeathParticles, m_player.transform.position + Vector3.down * 1f, Quaternion.identity);

        m_player.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        m_player.canControl = false;
        m_player.hasMoved = false;

        m_player.transform.position = m_player.transform.position + Vector3.down * 20f;

        yield return new WaitForSeconds(1f);

        m_gameRunning = true;

        m_player.anim.SetBool("Dead", false); 
        m_player.transform.position = playerStartPosition;
        m_player.isResetting = false;

        m_gameTimer = 0f;

        timerText.text = "0.00";
        timerText.color = Color.white;

        onPlayerReset?.Invoke();
    }

    public void SetGameRunning(bool canRun)
    {
        m_gameRunning = canRun;
    }

    public void LevelComplete()
    {
        // Activate UI: Passed or Failed
        if (m_gameTimer <= TimeRequirement)
        {
            levelPassedTimeText.text = m_gameTimer.ToString("F2");
            levelPassedUI.SetActive(true);
        }
        else
        {
            levelFailedTimeText.text = m_gameTimer.ToString("F2");
            levelFailedUI.SetActive(true);
        }

        m_gameRunning = false;
        m_player.canControl = false;

        if (m_gameTimer < m_lowestTime)
        {
            m_lowestTime = m_gameTimer;

            bestTimes[Level] = m_lowestTime;
            SaveSystem.SaveScores(bestTimes);

            lowestTimeText.text = m_lowestTime.ToString("F2");
        }
    }

    private void FixedUpdate()
    {
        if (m_gameRunning && m_player.hasMoved)
        {
            m_gameTimer += Time.deltaTime;
            timerText.text = m_gameTimer.ToString("F2");

            if (m_gameTimer > TimeRequirement)
            {
                timerText.color = Color.red;
            }
        }

        if (m_player.canControl == false && m_playerResetControlDelay > 0f)
        {
            m_playerResetControlDelay -= Time.deltaTime;
        }
        else if (m_gameRunning)
        {
            m_player.canControl = true;
            m_playerResetControlDelay = playerResetControlDelay;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            lowestTimeText.text = "";
            m_lowestTime = Mathf.Infinity;
            bestTimes[Level] = m_lowestTime;
            SaveSystem.SaveScores(bestTimes);
        }
    }
}
