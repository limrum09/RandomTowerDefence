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
    public bool isDataLoaded { get; private set; }
    public bool isSceneManagerReady { get; private set; }
    public bool isSceneUIReady { get; private set; }
    public bool isLoadingSceneReady {  get; private set; }
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

        if (!isLoadingSceneReady)
            return;

        isLoading = false;
        SceneManager.UnloadSceneAsync("LoadingScene");
    }

    public void ResetLoadingStat()
    {
        isSceneManagerReady = false;
        isSceneUIReady = false;
        isLoadingSceneReady = false;
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

    public void NotifyLoadingScene()
    {
        isLoadingSceneReady = true;
        TryCompletedLoading();
    }

    public void OnCompletedSignIn()
    {
        isDataLoaded = false;
        OnLoadStringScene("LobbyScene");
    }

    public void OnLoadStageScene() 
    {
        OnLoadStringScene("StageScene");
    }

    public void OnLoadStringScene(string sceneName) 
    {
        LoadSceneWithLoading(sceneName);
    }
}
