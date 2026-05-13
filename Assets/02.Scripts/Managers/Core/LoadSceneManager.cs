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
    public void OnLoadStageScene() => SceneManager.LoadScene("StageScene", LoadSceneMode.Single);

    public void OnLoadStringScene(string sceneName) => SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
}
