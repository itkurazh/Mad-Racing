using System;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : WindowUI
{
    [Header("Buttons")]
    [SerializeField] private ButtonUI _hostButton;
    [SerializeField] private ButtonUI _clientButton;

    private void Awake()
    {
        _hostButton.onClick.AddListener(OnHost);
        _clientButton.onClick.AddListener(OnClient);
    }

    private void OnHost()
    {
        Services.Game.StartGame();
    }

    private void OnClient()
    {
        Services.Game.ConnectToGame();
    }
}