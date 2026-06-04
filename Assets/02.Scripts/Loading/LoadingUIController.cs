using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LoadingUIController : MonoBehaviour
{
    [SerializeField]
    private Image currentLoadingBar;
    [SerializeField]
    private TextMeshProUGUI loadingInfoText;


    private bool isDataLoad;
    private bool isSceneManager;
    private bool isSceneUI;
    private float loadingValue;

    private Tween loadingTween;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isDataLoad = false;
        isSceneManager = false;
        isSceneUI = false;
        loadingValue = 0;
        currentLoadingBar.fillAmount = 0f;
        StartCoroutine(LoadingBar());
    }
    
    private void SetLoadingBar(float addValue, float duration)
    {
        loadingValue = Mathf.Min(loadingValue +  addValue, 100f);

        float targetFill = loadingValue / 100f;

        loadingTween?.Kill();

        loadingTween = currentLoadingBar.DOFillAmount(targetFill, duration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            currentLoadingBar.fillAmount = targetFill;
        });
    }

    private IEnumerator LoadingBar()
    {
        while(loadingValue < 100)
        {
            if (!isDataLoad && LoadSceneManager.Instance.isDataLoaded)
            {
                yield return new WaitForSeconds(1f);
                loadingInfoText.text = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "UI_DATA_LOAD_COMPLETE");
                isDataLoad = true;
                SetLoadingBar(60f, 0.4f);
            }

            if (!isSceneManager && LoadSceneManager.Instance.isSceneManagerReady)
            {
                yield return new WaitForSeconds(0.6f);
                loadingInfoText.text = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "UI_SCENE_MANAGER_READY");
                isSceneManager = true;
                SetLoadingBar(30f, 0.3f);
            }

            if (!isSceneUI && LoadSceneManager.Instance.isSceneUIReady)
            {
                yield return new WaitForSeconds(0.4f);
                loadingInfoText.text = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "UI_SCENE_UI_READY");
                isSceneUI = true;
                SetLoadingBar(10f, 0.2f);
            }

            yield return null;
        }

        yield return new WaitForSeconds(1f);

        LoadSceneManager.Instance.NotifyLoadingScene();

        yield return null;
    }
}
