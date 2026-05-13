using UnityEngine;

public class LobbyGateCtr : ObjectCheckPlayer<LobbyGateCtr>
{
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private LobbyUIController lobbyUI;

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
                lobbyUI.ShowSelectStageLevelView();
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                PlayerMouseClick();
                return;
            }
        }
    }

    protected override bool PlayerMouseClick()
    {
        if (!base.PlayerMouseClick())
            return false;

        lobbyUI.ShowSelectStageLevelView();

        return true;
    }

    protected override void PlayerEnter()
    {
        isPlayerEnter = true;
        anim.SetBool("IsOpen", isPlayerEnter);
    }

    protected override void PlayerExit()
    {
        if (lobbyUI == null)
            return;

        isPlayerEnter = false;
        lobbyUI.HideSelectStageLevelView();
        anim.SetBool("IsOpen", isPlayerEnter);
    }
}
