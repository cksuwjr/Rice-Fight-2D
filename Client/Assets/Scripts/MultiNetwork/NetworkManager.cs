using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using RiptideNetworking.Utils;
using System;

public enum ServerToClientId : ushort
{
    playerSpawned = 1,
    Move,
    PoseUpdate,
    AnimUpdate,
    SkillReady,
    SkillStart,
    playerUIUpdate,
}
public enum ClientToServerId : ushort
{
    name = 1,
    input,
    MyPose,
    MyAnim,
    GetDamage,
    Skill,
}

public class NetworkManager : MonoBehaviour
{
    private static NetworkManager _singleton;
    public static NetworkManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(NetworkManager)} 인스턴스가 이미 존재합니다. 기존 파괴할게");
                Destroy(value);
            }
        }
    }

    public Client Client { get; private set; }

    [SerializeField] private string ip;
    [SerializeField] private ushort port;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        Client = new Client();
        Client.Connected += DidConnect;
        Client.ConnectionFailed += FailedToConnect;
        Client.ClientDisconnected += PlayerLeft;
        Client.Disconnected += DidDisconnect;
    }

    private void FixedUpdate()
    {
        Client.Tick();
    }

    private void OnApplicationQuit()
    {
        Client.Disconnect();
    }
    public void Connect(string ip, string port)
    {
        if (ip != "")
            this.ip = ip.ToString();
        else
            this.ip = "127.0.0.1";
        if (port != "")
            this.port = ushort.Parse(port);
        else
            this.port = 7777;
        Client.Connect($"{this.ip}:{this.port}");
    }

    private void DidConnect(object sender, EventArgs e)
    {
        UIManager.Singleton.SendInformation();
    }

    private void FailedToConnect(object sender, EventArgs e)
    {
        UIManager.Singleton.BackToMain();
    }
    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        Destroy(Player.list[e.Id].gameObject);
    }
    private void DidDisconnect(object sender, EventArgs e)
    {
        UIManager.Singleton.BackToMain();

    }
    
}
