using UnityEngine;

public class LobbyMetaUpgrade : ObjectCheckPlayer<LobbyMetaUpgrade>
{
    [SerializeField]
    private LobbyUIController lobbyUICtr;

    private bool isPlayerEnter;

    private void Start()
    {
        isPlayerEnter = false;
    }

    private void Update()
    {
        if (isPlayerEnter)
        {
            if (Input.GetKeyDown(Managers.InputData.GetKeyCode(InputAction.NPCInteraction)))
            {
                ShowMetaUI();
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                PlayerMouseClick();
                return;
            }
        }
    }

    private void ShowMetaUI()
    {
        if (lobbyUICtr == null)
            return;

        lobbyUICtr.ShowMetaUpgradeView();
    }

    protected override bool PlayerMouseClick()
    {
        if (!base.PlayerMouseClick())
            return false;

        ShowMetaUI();

        return true;
    }

    protected override void PlayerEnter()
    {
        isPlayerEnter = true;
    }

    protected override void PlayerExit()
    {
        isPlayerEnter = false;

        if (lobbyUICtr == null)
            return;

        lobbyUICtr.HideMetaUpgradeView();
    }
}
