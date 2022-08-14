using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();   //  < id, Player.cs > ��� �ڷᱸ������
    public ushort Id { get; private set; }  // ���� ID
    public bool IsLocal { get; private set; }  //  �� �÷��̾��ΰ�
    private string username;

    [SerializeField] private MoveController moveController;
    [SerializeField] private AnimManager animManager;
    [SerializeField] private PlayerSkill playerSkill;
    [SerializeField] private PlayerUIManager playerUIManager;

    Rigidbody2D rb;
    private void OnDestroy()
    {
        list.Remove(Id);    
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    public void Gethurt(float damage)
    {
        Debug.Log(Id + "������" + damage + "����");
        Message message = Message.Create(MessageSendMode.unreliable, ClientToServerId.GetDamage);
        message.AddUShort(Id);
        message.AddFloat(damage);
        NetworkManager.Singleton.Client.Send(message);
    }

    public void SetState(string state)
    {
        switch (state) {
            case "Nothing":
                moveController.state = MoveController.State.Nothing;
                break;
            case "Charging":
                moveController.state = MoveController.State.Charging;
                break;
        }
    }











    public static void Spawn(ushort id, string username, Vector2 position)   //  �÷��̾� ��ȯ
    {
        Player player;
        if (id == NetworkManager.Singleton.Client.Id)  // �����̸�
        {
            player = Instantiate(GameLogic.Singleton.LocalPlayerPrefab, position, Quaternion.identity).GetComponent<Player>();
            player.IsLocal = true;
        }
        else  // Ÿ���̸�
        {
            player = Instantiate(GameLogic.Singleton.PlayerPrefab, position, Quaternion.identity).GetComponent<Player>();
            player.IsLocal = false;
        }

        player.name = $"Player {id} ({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
        player.Id = id;
        player.username = username;

        player.playerUIManager.SetNickname($" {(string.IsNullOrEmpty(username) ? $"Player {id} (Guest)" : username)}");
        list.Add(id, player);
    }

    public static void Move(ushort id, int direction, bool jump)
    {
        if (list.TryGetValue(id, out Player player))
        {
            if (direction != 0 && player.moveController.state == MoveController.State.Nothing)
                player.transform.GetChild(1).localScale = new Vector3(-direction, 1, 1);

            if(NetworkManager.Singleton.Client.Id == id)
                player.moveController.Move(direction, jump);
        }
    }
    public void PositionUpdated(Vector3 position)
    {
        rb.MovePosition(position); 
        // 1.  transform.position = position;     ������Ʈ ��ġ ��� �����̵�,  ��� �ݸ����� ������ٵ� ��ġ ����, ��������
        // 2.  rb.position = position;            ���� ���� �ùķ��̼� ���� ������Ʈ ��ġ �����̵�, 10�� ���� tranfrom���� ����
        // 3.  rb.MovePosition(position);         �ڿ������� ������
    }
    [MessageHandler((ushort)ServerToClientId.playerSpawned)]
    private static void SpawnPlayer(Message message)
    {
        Spawn(message.GetUShort(), message.GetString(), message.GetVector2());
    }

    [MessageHandler((ushort)ServerToClientId.Move)]
    private static void SendMoveAccept(Message message)
    {//NetworkManager.Singleton.Client.Id
        Move(message.GetUShort(), message.GetInt(), message.GetBool());
    }

    [MessageHandler((ushort)ServerToClientId.PoseUpdate)]
    private static void PoseUpdateAccept(Message message)
    {
        ushort id = message.GetUShort();
        if (list.TryGetValue(id, out Player player))
        {
            if(id != NetworkManager.Singleton.Client.Id)
                player.transform.position = message.GetVector2();
        }
    }
    [MessageHandler((ushort)ServerToClientId.AnimUpdate)] // ���� ������ �ٸ������ �ִϸ��̼� ������Ʈ
    private static void AnimUpdateAccept(Message message)
    {
        ushort id = message.GetUShort();
        bool[] animParameter = message.GetBools(3);
        if (list.TryGetValue(id, out Player player))
        {
            if (id != NetworkManager.Singleton.Client.Id)
            {
                player.animManager.AnimSetMove(animParameter[0], animParameter[1], animParameter[2]);
            }
                
        }
    }

    [MessageHandler((ushort)ServerToClientId.SkillReady)]
    private static void SkillAccept(Message message) // �ݵ�� �޽��� ���޼������ Get-- �Ұ�, �ƴϸ� ������
    {
        if (list.TryGetValue(NetworkManager.Singleton.Client.Id, out Player player))
        {
            player.playerSkill.SkillReady(message.GetInt(), message.GetString());
        }
    }
    [MessageHandler((ushort)ServerToClientId.SkillStart)]
    private static void playerSkillStartAccept(Message message)
    {
        if (list.TryGetValue(message.GetUShort(), out Player player))
        {
            player.playerSkill.Skill(message.GetInt(), message.GetString());
        }
    }
    [MessageHandler((ushort)ServerToClientId.playerUIUpdate)]
    private static void playerUIUpdateAccept(Message message)
    {
        if (list.TryGetValue(message.GetUShort(), out Player player))
        {
            player.playerUIManager.UIUpdate(message.GetFloat(), message.GetFloat(), message.GetFloat());
        }
    }

}
