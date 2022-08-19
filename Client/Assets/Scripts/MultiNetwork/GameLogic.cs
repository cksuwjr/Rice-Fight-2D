using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    private static GameLogic _singleton;
    public static GameLogic Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(GameLogic)} �ν��Ͻ��� �̹� �����մϴ�. ���� �ı��Ұ�");
                Destroy(value);
            }
        }
    }


    public GameObject LocalPlayerPrefab => localPlayerPrefab;
    public GameObject PlayerPrefab => playerPrefab;
    public GameObject OtherPlayerUI => otherPlayerUI;
    [Header("Prefabs")]
    [SerializeField] private GameObject localPlayerPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject otherPlayerUI;

    public GameObject Lection => lection;
    public GameObject Kara => kara;
    public GameObject Crollo => crollo;
    [Header("Characters")]
    [SerializeField] private GameObject lection;
    [SerializeField] private GameObject kara;
    [SerializeField] private GameObject crollo;

    private void Awake()
    {
        Singleton = this;
    }

}
