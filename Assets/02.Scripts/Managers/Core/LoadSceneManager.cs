using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneStageStat
{
    Easy,
    Normal,
    Hard,
    Hell
}
public class LoadSceneManager : SingletonMono<LoadSceneManager>
{
    private bool isDataLoaded;
    private bool isSceneManagerReady;
    private bool isSceneUIReady;
    private bool isLoading;

    private string targetSceneName;

    private async void LoadSceneWithLoading(string sceneName)
    {
        if (isLoading)
            return;

        targetSceneName = sceneName;
        ResetLoadingStat();

        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Single);

        await Task.Yield();

        AsyncOperation op = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);

        while (!op.isDone)
            await Task.Yield();

        Scene targetScene = SceneManager.GetSceneByName(targetSceneName);

        SceneManager.SetActiveScene(targetScene);
    }

    private void TryCompletedLoading()
    {
        if (!isLoading)
            return;

        if (!isDataLoaded)
            return;

        if (!isSceneManagerReady)
            return;

        if (!isSceneUIReady)
            return;

        isLoading = false;
        SceneManager.UnloadSceneAsync("LoadingScene");
    }

    public void ResetLoadingStat()
    {
        isSceneManagerReady = false;
        isSceneUIReady = false;
        isLoading = true;
    }

    public void NotifyDataLoaded()
    {
        isDataLoaded = true;
        TryCompletedLoading();
    }

    public void NotifySceneManagerReady()
    {
        isSceneManagerReady = true;
        TryCompletedLoading();
    }

    public void NotifySceneUIReady()
    {
        isSceneUIReady = true;
        TryCompletedLoading();
    }

    public void OnCompletedSignIn()
    {
        isDataLoaded = false;
        OnLoadStringScene("LobbyScene");
    }

    public void OnLoadStageScene() 
    {
        SceneManager.LoadScene("StageScene", LoadSceneMode.Single);
    }

    public void OnLoadStringScene(string sceneName) 
    {
        LoadSceneWithLoading(sceneName);
    }
}
