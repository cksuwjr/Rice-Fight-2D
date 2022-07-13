using System.Collections;
using System.Collections.Generic;



using UnityEngine;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;
using System;
using System.IO;
public class ServerClient  // 1
{
    public string clientName;
    public TcpClient tcp;

    public ServerClient(TcpClient clientSocket)
    {
        clientName = "Guest";
        tcp = clientSocket;
    }
}



public class Server : MonoBehaviour
{
    public InputField PortInput;

    List<ServerClient> clients;         // 1
    List<ServerClient> disconnectList;

    TcpListener server;
    bool serverStarted;

    public void ServerCreate()
    {
        clients = new List<ServerClient>();
        disconnectList = new List<ServerClient>();

        try
        {
            int port = PortInput.text == "" ? 7777 : int.Parse(PortInput.text);
            server = new TcpListener(IPAddress.Any, port);

            server.Start();
            StartListening(); // 2
            serverStarted = true;
            Chat.instance.ShowMessage($"������ {port}���� ���۵Ǿ����ϴ�");
        }
        catch(Exception e)
        {
            Chat.instance.ShowMessage($"Socket error: {e.Message}");
        }
    }
    private void Update()
    {
        if (!serverStarted) return;
        foreach(ServerClient c in clients)
        {
            if (IsConnected(c.tcp)) // 4
            {
                c.tcp.Close();
                disconnectList.Add(c);
                continue;
            }
            else
            {
                NetworkStream s = c.tcp.GetStream();
                if (s.DataAvailable)
                {
                    string data = new StreamReader(s, true).ReadLine(); // ??
                    if (data != null)
                        OnIncomingData(c, data);

                }
            }

        }
        for (int i = 0; i < disconnectList.Count - 1; i++)
        {
            Broadcast($"{disconnectList[i].clientName}������ ���������ϴ�", clients); 
            clients.Remove(disconnectList[i]);
            disconnectList.RemoveAt(i);
        }
    }
    bool IsConnected(TcpClient c) // 4
    {
        try
        {
            if(c != null && c.Client != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead)) // ??
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0); // ??
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }
    public void StartListening() // 2
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server); // 3
    }
    void AcceptTcpClient(IAsyncResult ar) // 3
    {
        TcpListener listener = (TcpListener)ar.AsyncState;
        clients.Add(new ServerClient(listener.EndAcceptTcpClient(ar)));
        StartListening();

        // �޽��� ����� ��ο��� ����
        Broadcast("%Name", new List<ServerClient>() { clients[clients.Count - 1] }); // ??
    }
    void OnIncomingData(ServerClient c, string data)
    {
        if (data.Contains("&Name"))
        {
            c.clientName = data.Split('|')[1];
            Broadcast($"{c.clientName}�� ����Ǿ����ϴ�", clients);
            return;
        }

        Broadcast($"{c.clientName} : {data}", clients);
    }
    void Broadcast(string data, List<ServerClient> cl)
    {
        foreach(var c in cl)
        {
            try
            {
                StreamWriter writer = new StreamWriter(c.tcp.GetStream());
                writer.WriteLine(data);
                writer.Flush(); // ??
            }
            catch(Exception e)
            {
                Chat.instance.ShowMessage($"���� ���� : {e.Message}�� Ŭ���̾�Ʈ���� {c.clientName}");
            }
        }
    }

}
