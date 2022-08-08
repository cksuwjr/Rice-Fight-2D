using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();  //  < id, Player.cs > ��� �ڷᱸ��

    public ushort Id { get; private set; } // ���� ID
    public string Username { get; private set; } //  ���� �̸�

    public PlayerMovement playerMove;   
    private void OnDestroy()
    {
        list.Remove(Id);
    }
    public static void Spawn(ushort id, string username) // ��ȯ(id, �̸�)
    {
        foreach(Player otherPlayer in list.Values)      // ���� ��� �÷��̾�鿡�� SendSpawned(id) ȣ�� (���ο� �÷��̾� ����!)
            otherPlayer.SendSpawned(id);

        // ���� ������ �÷��̾� ����
        Player player = Instantiate(GameLogic.Singleton.PlayerPrefab, new Vector2(0, 1), Quaternion.identity).GetComponent<Player>();
        player.name = $"Player {id} ({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
        player.Id = id;
        player.Username = username;

        player.SendSpawned(); // ���� ������ �÷��̾�� ��� �÷��̾�鿡�� SendSpawned()ȣ��
        list.Add(id, player);
    }
    private void Move(Vector2 newPosition)   //  �����κ��� ServerToClientId.playerMovement ������ Player.cs �ϴ��� �ܿ��� �ش�������� ǥ���� Player.cs ȣ��.
    {
        transform.position = newPosition;                     // �̵� ��ǥ �޾ƿͼ� �̵���Ű��
    }

    #region Messages
    private void SendSpawned()
    {
        NetworkManager.Singleton.Server.SendToAll(AddSpawnData(Message.Create(MessageSendMode.reliable, ServerToClientId.playerSpawned)));
    }

    private void SendSpawned(ushort toClientId)
    {
        NetworkManager.Singleton.Server.Send(AddSpawnData(Message.Create(MessageSendMode.reliable, ServerToClientId.playerSpawned)), toClientId);
    }

    private Message AddSpawnData(Message message)
    {
        message.AddUShort(Id);
        message.AddString(Username);
        message.AddVector2(transform.position);
        return message;
    }

    public void PositionUpdate(ushort id, Vector3 newPosition)
    {
        transform.position = newPosition;
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.PoseUpdate);
        message.AddUShort(id);
        message.AddVector2(transform.position);
        NetworkManager.Singleton.Server.SendToAll(message);
    }


    [MessageHandler((ushort)ClientToServerId.name)]
    private static void Name(ushort fromClientId, Message message)
    {
        Spawn(fromClientId, message.GetString());
    }
    [MessageHandler((ushort)ClientToServerId.input)]
    private static void Input(ushort fromClientId, Message message)
    {
        if (list.TryGetValue(fromClientId, out Player player))
            player.playerMove.SetInput(message.GetBools(4));
    }

    [MessageHandler((ushort)ClientToServerId.MyPose)]
    private static void Pose(ushort fromClientId, Message message)
    {
        if (list.TryGetValue(fromClientId, out Player player))
            player.PositionUpdate(fromClientId, message.GetVector2());
    }
    #endregion
}
