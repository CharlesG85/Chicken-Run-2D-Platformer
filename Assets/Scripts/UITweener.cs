using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITweener : MonoBehaviour
{
    [SerializeField]
    private GameObject
        settingsButton,
        settingsBackground,
        settingsPanel,
        levelPassedPanel,
        startLevelBackground,
        startLevelText,
        crossfadeOverlay;

    [SerializeField] private TMP_Text startLevelTimeReqText;

    [Space]

    [SerializeField] private float tweenTime = 0.1f;
    [SerializeField] private float startPanelTweenTime = 0.5f;
    [SerializeField] private float crossFadeTweenTime = 1f;
    [SerializeField] private float buttonTweenTime = 0.25f;
    [SerializeField] private LeanTweenType settingsEaseType;
    [SerializeField] private LeanTweenType startPanelEaseType;
    [SerializeField] private LeanTweenType easeType;

    bool settingsPanelActive;

    private void Start()
    {
        if (startLevelBackground != null && startLevelText != null)
            AnimateStartLevelPanel();

        if (crossfadeOverlay != null)
            CrossFadeEnd();
    }

    public void ButtonPop(GameObject buttonGO)
    {
        LeanTween.scale(buttonGO, Vector3.one * 1.25f, buttonTweenTime).setEasePunch();
    }

    public void CrossFadeStart()
    {
        crossfadeOverlay.SetActive(true);
        LeanTween.value(1f, 0f, crossFadeTweenTime).setIgnoreTimeScale(true)
        .setOnUpdate((float value) =>
        {
            crossfadeOverlay.GetComponent<CanvasGroup>().alpha = value;
        })
        .setOnComplete(() =>
        {
            crossfadeOverlay.SetActive(false);
        });
    }

    public void CrossFadeEnd()
    {
        
        LeanTween.value(1f, 0f, crossFadeTweenTime).setIgnoreTimeScale(true)
        .setOnUpdate((float value) =>
        {
            crossfadeOverlay.GetComponent<CanvasGroup>().alpha = value;
        })
        .setOnComplete(() =>
        {
            crossfadeOverlay.SetActive(false);
        });
    }

    public void AnimateStartLevelPanel()
    {
        // Tween Background Panel
        LeanTween.scaleY(startLevelBackground, 0f, 0f);
        startLevelBackground.SetActive(true);

        LeanTween.scaleY(startLevelBackground, 1f, startPanelTweenTime / 2f).setIgnoreTimeScale(true).setEaseOutCirc().setDelay(0.5f).setOnComplete(() =>
        {
            LeanTween.scaleY(startLevelBackground, 0f, startPanelTweenTime / 2f).setIgnoreTimeScale(true).setEaseInCirc().setDelay(0.5f).setOnComplete(() =>
            {
                startLevelBackground.SetActive(false);
            });

        });

        // Tween Start Level Text
        LeanTween.moveY(startLevelText, 2000, 0f);
        float posY = Screen.height / 2f;

        LeanTween.moveY(startLevelText, posY, startPanelTweenTime / 2f).setIgnoreTimeScale(true).setEaseOutCirc().setDelay(0.5f).setOnComplete(() =>
        {
            LeanTween.moveY(startLevelText, -2000f, startPanelTweenTime / 2f).setIgnoreTimeScale(true).setEaseInCirc().setDelay(0.5f).setOnComplete(() =>
            {
                startLevelText.SetActive(false);
                GameManager.Instance.SetGameRunning(true);
            });
        });

        // Tween Time Requirement Text
        LeanTween.value(0f, 1f, startPanelTweenTime).setIgnoreTimeScale(true)
        .setOnUpdate((float value) =>
        {
            Color col = startLevelTimeReqText.color;
            col.a = value;
            startLevelTimeReqText.color = col;
        })
        .setOnComplete(() =>
        {
            LeanTween.value(1f, 0f, startPanelTweenTime / 2f).setIgnoreTimeScale(true)
            .setOnUpdate((float value) =>
            {
                Color col = startLevelTimeReqText.color;
                col.a = value;
                startLevelTimeReqText.color = col;
            });
        });
    }

    public void ToggleSettingsPanel()
    {
        LeanTween.scale(settingsButton, Vector3.one * 1.25f, buttonTweenTime).setEasePunch().setIgnoreTimeScale(true);

        settingsPanelActive = !settingsPanelActive;

        if (settingsPanelActive)
        {
            GameManager.Instance.SetGameRunning(false);
            Time.timeScale = 0f;

            settingsPanel.SetActive(true);
            settingsBackground.SetActive(true);

            LeanTween.scale(settingsPanel, Vector3.one, tweenTime).setIgnoreTimeScale(true).setEase(settingsEaseType);
            LeanTween.alpha(settingsBackground, 120, tweenTime);
        }
        else
        {
            LeanTween.scale(settingsPanel, Vector3.zero, tweenTime).setIgnoreTimeScale(true).setOnComplete(() =>
            {
                settingsPanel.SetActive(false);
                GameManager.Instance.SetGameRunning(true);
                Time.timeScale = 1f;
            });
            LeanTween.alpha(settingsBackground, 0, tweenTime).setIgnoreTimeScale(true).setOnComplete(() =>
            {
                settingsBackground.SetActive(false);
            });
        }
    }
}
