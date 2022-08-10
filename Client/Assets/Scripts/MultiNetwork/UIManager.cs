using RiptideNetworking;
using RiptideNetworking.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _singleton;
    public static UIManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(UIManager)} 인스턴스가 이미 존재합니다. 기존 파괴할게");
                Destroy(value);
            }
        }
    }

    [Header("Connect")]
    [SerializeField] private GameObject connectUI;
    [SerializeField] private InputField IpField;
    [SerializeField] private InputField PortField;
    [SerializeField] private InputField usernameField;

    private void Awake()
    {
        Singleton = this;
    }

    public void ConnectClicked()
    {
        IpField.interactable = false;
        PortField.interactable = false;
        usernameField.interactable = false;
        connectUI.SetActive(false);

        NetworkManager.Singleton.Connect(IpField.text, PortField.text);
    }

    public void BackToMain()
    {
        IpField.interactable = true;
        PortField.interactable = true;
        usernameField.interactable = true;
        connectUI.SetActive(true);
    }
    public void SendName()
    {
        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.name);
        message.AddString(usernameField.text);
        NetworkManager.Singleton.Client.Send(message);
    }
}
