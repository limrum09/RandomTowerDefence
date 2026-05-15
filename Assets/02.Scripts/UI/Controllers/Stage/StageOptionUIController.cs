using System;
using UnityEngine;
using UnityEngine.UI;

public class StageOptionUIController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField]
    private Button continueButton;
    [SerializeField]
    private Button soundOptionButton;
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

    public event Action OnStageGameContinue;
    public event Action OnMoveToLobby;

    private void Start()
    {
        frame.SetActive(false);
    }

    private void OnEnable()
    {
        lobbyCheckPopup.SetActive(false);
    }

    public void ShowOptionPanel()
    {
        frame.SetActive(true);
    }

    public void StageGameContinue()
    {
        frame.SetActive(false);
        OnStageGameContinue?.Invoke();
    }

    public void MoveToLobby()
    {
        OnMoveToLobby?.Invoke();
    }
}
