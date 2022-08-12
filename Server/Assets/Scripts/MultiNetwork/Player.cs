using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();  //  < id, Player.cs > ��� �ڷᱸ��

    public ushort Id { get; private set; } // ���� ID
    public string Username { get; private set; } //  ���� �̸�

    public PlayerKeyinput playerkeyinput;

    [SerializeField] private Status status;

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

    public void GetHurt(float damage)
    {
        status.currentHP -= damage;
        SendUIUpdate();
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

    private void SendUIUpdate()
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.playerUIUpdate);
        message.AddUShort(Id);
        message.AddFloat(status.MaxHP);
        message.AddFloat (status.currentHP);
        message.AddFloat(status.AttackPower);
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
            player.playerkeyinput.SetInput(message.GetBools(8));
    }

    [MessageHandler((ushort)ClientToServerId.MyPose)]
    private static void Pose(ushort fromClientId, Message message)
    {
        if (list.TryGetValue(fromClientId, out Player player))
            player.PositionUpdate(fromClientId, message.GetVector2());
    }
    public void PositionUpdate(ushort id, Vector3 newPosition)
    {
        transform.position = newPosition;
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.PoseUpdate);
        message.AddUShort(id);
        message.AddVector2(transform.position);
        NetworkManager.Singleton.Server.SendToAll(message);
    }

    [MessageHandler((ushort)ClientToServerId.MyAnim)]
    private static void Anim(ushort fromClientId, Message message)
    {
        if (list.TryGetValue(fromClientId, out Player player))
            player.AnimationUpdate(fromClientId, message.GetBools(3));
    }
    public void AnimationUpdate(ushort id, bool[] newAnimation)
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.AnimUpdate);
        message.AddUShort(id);
        message.AddBools(newAnimation, false);
        NetworkManager.Singleton.Server.SendToAll(message);
    }
    
    [MessageHandler((ushort)ClientToServerId.GetDamage)]
    private static void GethurtAccept(ushort fromClientId, Message message)
    {
        if(list.TryGetValue(message.GetUShort(), out Player player))
        {
            Debug.Log("�� ���ƾ��" + player.Id);
            
            player.GetHurt(message.GetFloat());
        }
    }

    [MessageHandler((ushort)ClientToServerId.Skill)]
    private static void SkillAccept(ushort fromClientId, Message message)
    {
        ushort id = message.GetUShort();
        if (list.TryGetValue(id, out Player player))
        {
            player.SkillStart(id, message.GetInt(), message.GetString());
        }
    }

    public void SkillStart(ushort id, int direction, string key)
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.SkillStart);
        message.AddUShort(id);
        message.AddInt(direction);
        message.AddString(key);
        NetworkManager.Singleton.Server.SendToAll(message);
    }
    #endregion
}
