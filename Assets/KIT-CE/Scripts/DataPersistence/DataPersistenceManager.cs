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

    // get은 public(아무곳에서나 get가능) set은 private(DataPersistenceManager에서만 수정 가능)
    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("이미 DataManager가 존재합니다.");
        }
        instance = this;
    }

    private void Start()
    {
        // persistentDataPath는 특별한 이유가 없을 경우 바꾸기를 권장하지 않음(Unity 기본 Path로 설정됨)
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
            Debug.Log("데이터를 찾지 못했습니다. 새로운 데이터를 생성합니다.");
            NewGame();
        }

        // 로드된 데이터를 필요한 컴포넌트들에 입력
        foreach(IDataPersistence dataPersistenceObj in dataPersistenceObjects) 
        {
            dataPersistenceObj.LoadData(gameData);
        }

        Debug.Log("Loaded HP count = " + gameData.health);
    }

    public void SaveGame()
    {
        // 모든 IDataPersistence가 상속된 컴포넌트들에 gameData를 참조시켜 수정하여 저장
        // 사람들에게 종이 돌려 설문조사하고 돌려받는 느낌?
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
