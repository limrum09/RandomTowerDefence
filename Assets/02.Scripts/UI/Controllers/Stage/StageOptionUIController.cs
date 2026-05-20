using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageOptionUIController : MonoBehaviour
{
    [Header("Canvas Group")]
    [SerializeField]
    private CanvasGroup canvas;

    [Header("Buttons")]
    [SerializeField]
    private Button continueButton;
    [SerializeField]
    private Button inputOptionButton;
    [SerializeField]
    private Button lobbyButton;
    [SerializeField]
    private Button lobbyOkButton;

    [Header("Game Objects")]
    [SerializeField]
    private GameObject frame;
    [SerializeField]
    private GameObject lobbyCheckPopup;

    [Header("Panels")]
    [SerializeField]
    private SettingUIController settingPanel;

    [Header("Local Texts")]
    [SerializeField]
    private TextMeshProUGUI continueButtonText;
    [SerializeField]
    private TextMeshProUGUI inputOptionButtonText;
    [SerializeField]
    private TextMeshProUGUI lobbyButtonText;
    [SerializeField]
    private TextMeshProUGUI lobbyCheckText;
    [SerializeField]
    private TextMeshProUGUI lobbyMoveText;
    [SerializeField]
    private TextMeshProUGUI lobbyCancelText;


    public event Action OnStageGameContinue;
    public event Action OnMoveToLobby;

    private void Start()
    {
        continueButtonText.text = Managers.Local.GetString("BUTTON_CONTINUE");
        inputOptionButtonText.text = Managers.Local.GetString("BUTTON_INPUT_OPTION");
        lobbyButtonText.text = Managers.Local.GetString("BUTTON_MOVE_LOBBY");
        lobbyCheckText.text = Managers.Local.GetString("TEXT_MOVE_LOBBY_CHECK");
        lobbyMoveText.text = Managers.Local.GetString("BUTTON_MOVE");
        lobbyCancelText.text = Managers.Local.GetString("BUTTON_CANCEL_LONG");

        HideOption();
    }

    private void OnEnable()
    {
        lobbyCheckPopup.SetActive(false);
    }

    public void ShowOptionPanel()
    {
        ShowOption();
    }

    public void StageGameContinue()
    {
        HideOption();
        OnStageGameContinue?.Invoke();
    }

    public void MoveToLobby()
    {
        OnMoveToLobby?.Invoke();
    }

    public void ShowOption()
    {
        canvas.alpha = 1.0f;
        canvas.interactable = true;
        canvas.blocksRaycasts = true;
    }

    public void HideOption()
    {
        canvas.alpha = 0.0f;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;

    }
}
