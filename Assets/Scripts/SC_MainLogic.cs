using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SC_MainLogic : MonoBehaviour
{
    #region Variables
    private List<string> blockList = new(){ "Block_I", "Block_J", "Block_L", "Block_O", "Block_S", "Block_T", "Block_Z" };
    private int lastBlock;
    public delegate void TogglePaused();
    public static event TogglePaused OnTogglePause;
    public delegate void Restart();
    public static event Restart OnRestart;
    private Dictionary<string, GameObject> Messages = new();
    public static int numberOfBlocks = 0;
    #endregion

    #region MonoBehaviour
    void Awake()
    {
        InitVariables();
    }
    void Start()
    {
        CreateNextBlock();
        CreateNextBlock();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Messages["PauseMessage"].SetActive(!Messages["PauseMessage"].activeSelf);
            if (OnTogglePause != null)
            {
                OnTogglePause();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (OnRestart != null)
            {
                foreach (var item in Messages)
                {
                    item.Value.SetActive(false);
                }
                numberOfBlocks = 0;
                OnRestart();
                CreateNextBlock();
                CreateNextBlock();
            }
        }
    }
    #endregion

    #region Logic
    void InitVariables()
    {
        GameObject [] _list = GameObject.FindGameObjectsWithTag("Message"); 
        foreach (GameObject message in _list)
        {
            Messages.Add(message.name, message);
            message.SetActive(!message.activeSelf);
        }
        SC_BlockMovments.OnImDoneFalling += CreateNextBlock;
        SC_BlockMovments.OnGameOver += GameOver;
    }
    void CreateNextBlock()
    {
        int i, attempts = 0;
        do
        {
            i = Random.Range(0, 7); attempts++;
        }
        while (i == lastBlock && attempts < 3);
        lastBlock = i;
        GameObject _nextBlock = Instantiate(Resources.Load("Prefabs/" + blockList.ElementAt(i))) as GameObject;
        _nextBlock.name = blockList.ElementAt(i) + ++numberOfBlocks;
    }
    void GameOver()
    {
        Messages["gameoverMessage"].SetActive(!Messages["gameoverMessage"].activeSelf);
        OnTogglePause();
    }
    #endregion
}
