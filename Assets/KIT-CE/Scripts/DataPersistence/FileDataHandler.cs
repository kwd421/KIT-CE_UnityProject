using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load()
    {
        // Path.Combine�� ���� OS���� ��� ���ڿ� �����ڰ� �ٸ��ٸ� �߻��ϴ� ���� �ذᰡ��
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;
        if(File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                // FileStream�� ����ϴ� ����: FileMode�� ���
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // Json�����͸� C#���� ����� �� �ְ� ����ȭ ����(deserialize)
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch(Exception e)
            {
                Debug.LogError("���Ͽ��� �����͸� �ҷ����� ���߽��ϴ�." + fullPath + "\n" + e);
            }
        }
        return loadedData;
    }

    public void Save(GameData data)
    {
        // Path.Combine�� ���� OS���� ��� ���ڿ� �����ڰ� �ٸ��ٸ� �߻��ϴ� ���� �ذᰡ��
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            // �������� �ʴ� ��� ������ ��� ������ ���� write
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // C# ���� �����͸� Json���� ����ȭ(serializa)
            string dataToStore = JsonUtility.ToJson(data, true);

            // ����ȭ�� �����͸� ���Ͽ� write
            // using���� ����ϸ� ����� �ڵ����� ����(Close) �� ����(Dispose)�ǹǷ� �޸� ���� ���� �� �ִ�.
            // (������ �ݾ��� �ʿ䰡 ����!)
            // FileStream�� ����ϴ� ����: FileMode�� ���
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
            Debug.LogError("���Ͽ� �����͸� �������� ���߽��ϴ�." + fullPath + "\n" + e);
        }
    }
}
