using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    // get�� public(�ƹ��������� get����) set�� private(DataPersistenceManager������ ���� ����)
    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("�̹� DataManager�� �����մϴ�.");
        }
        instance = this;
    }

    private void Start()
    {
        // persistentDataPath�� Ư���� ������ ���� ��� �ٲٱ⸦ �������� ����(Unity �⺻ Path�� ������)
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        this.gameData = dataHandler.Load();

        if(this.gameData == null)
        {
            Debug.Log("�����͸� ã�� ���߽��ϴ�. ���ο� �����͸� �����մϴ�.");
            NewGame();
        }

        // �ε�� �����͸� �ʿ��� ������Ʈ�鿡 �Է�
        foreach(IDataPersistence dataPersistenceObj in dataPersistenceObjects) 
        {
            dataPersistenceObj.LoadData(gameData);
        }

        Debug.Log("Loaded HP count = " + gameData.health);
    }

    public void SaveGame()
    {
        // ��� IDataPersistence�� ��ӵ� ������Ʈ�鿡 gameData�� �������� �����Ͽ� ����
        // ����鿡�� ���� ���� ���������ϰ� �����޴� ����?
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        Debug.Log("Saved HP count = " + gameData.health);
        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
