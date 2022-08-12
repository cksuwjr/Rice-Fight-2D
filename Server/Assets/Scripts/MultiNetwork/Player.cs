using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();  //  < id, Player.cs > 담는 자료구조

    public ushort Id { get; private set; } // 고유 ID
    public string Username { get; private set; } //  유저 이름

    public PlayerKeyinput playerkeyinput;

    [SerializeField] private Status status;

    private void OnDestroy()
    {
        list.Remove(Id);
    }
    public static void Spawn(ushort id, string username) // 소환(id, 이름)
    {
        foreach(Player otherPlayer in list.Values)      // 현재 모든 플레이어들에게 SendSpawned(id) 호출 (새로운 플레이어 등장!)
            otherPlayer.SendSpawned(id);

        // 실제 서버에 플레이어 생성
        Player player = Instantiate(GameLogic.Singleton.PlayerPrefab, new Vector2(0, 1), Quaternion.identity).GetComponent<Player>();
        player.name = $"Player {id} ({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
        player.Id = id;
        player.Username = username;

        player.SendSpawned(); // 새로 생성된 플레이어에서 모든 플레이어들에게 SendSpawned()호출
        list.Add(id, player);
    }
    private void Move(Vector2 newPosition)   //  서버로부터 ServerToClientId.playerMovement 받으면 Player.cs 하단의 단에서 해당움직임을 표출한 Player.cs 호출.
    {
        transform.position = newPosition;                     // 이동 좌표 받아와서 이동시키기
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
            Debug.Log("나 다쳤어요" + player.Id);
            
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
