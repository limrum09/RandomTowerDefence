#if UNITY_EDITOR
using UnityEngine;

public class LobbyProgressDebugger : MonoBehaviour
{
    [SerializeField]
    private LobbyUIController lobbyUI;
    [SerializeField]
    private int metaCurrenyAmount = 50000;
    [SerializeField]
    private int expAmount = 1000;

    public int MetaCurrenyAmount => metaCurrenyAmount;
    public int ExpAmount => expAmount;

    [ContextMenu("Debug/Add Meta Currency")]
    public void AddMetaCurrency()
    {
        Managers.Player.AddCurrency(metaCurrenyAmount);
        lobbyUI?.RefreshMetaProgress();
    }

    [ContextMenu("Debug/Add Meta Exp")]
    public void AddExp()
    {
        Managers.Player.AddExp(expAmount);
        lobbyUI?.RefreshMetaProgress();
    }

    public void SaveMetaPregressData()
    {
        Managers.Save.MarkPlayerDirty();
        _ = Managers.Save.SavePlayerProgressData();
    }
}
#endif