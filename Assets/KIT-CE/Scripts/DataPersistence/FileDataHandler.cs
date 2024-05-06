using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";
    private bool useEncryption = false;
    private readonly string encryptionCodeWord = "word";

    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }

    public GameData Load()
    {
        // Path.Combine은 만약 OS마다 경로 문자열 구분자가 다르다면 발생하는 문제 해결가능
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;
        if(File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                // FileStream을 사용하는 이유: FileMode를 명시
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                if(useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                // Json데이터를 C#에서 사용할 수 있게 직렬화 해제(deserialize)
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch(Exception e)
            {
                Debug.LogError("파일에서 데이터를 불러오지 못했습니다." + fullPath + "\n" + e);
            }
        }
        return loadedData;
    }

    public void Save(GameData data)
    {
        // Path.Combine은 만약 OS마다 경로 문자열 구분자가 다르다면 발생하는 문제 해결가능
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            // 존재하지 않는 경로 지정시 경로 생성해 파일 write
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // C# 게임 데이터를 Json으로 직렬화(serializa)
            string dataToStore = JsonUtility.ToJson(data, true);

            if(useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            // 직렬화된 데이터를 파일에 write
            // using문을 사용하면 사용후 자동으로 정리(Close) 및 해제(Dispose)되므로 메모리 낭비를 줄일 수 있다.
            // (파일을 닫아줄 필요가 없다!)
            // FileStream을 사용하는 이유: FileMode를 명시
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("파일에 데이터를 저장하지 못했습니다." + fullPath + "\n" + e);
        }
    }

    // XOR을 이용한 암호화, 복호화
    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for(int i=0; i<data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }
        return modifiedData;
    }
}
