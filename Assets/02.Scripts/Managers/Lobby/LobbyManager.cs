using UnityEditor.SceneManagement;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [SerializeField]
    private LobbyUIController lobbyUICtr;

    private void Awake()
    {
        lobbyUICtr.OnSelectStage += OnSelectStageLevel;
    }

    private void OnDestroy()
    {
        lobbyUICtr.OnSelectStage -= OnSelectStageLevel;
    }

    private void OnSelectStageLevel(string level)
    {
        Managers.Game.SelectStageDifficultyLevel(level);

        LoadSceneManager.Instance.OnLoadStageScene();
    }
}
