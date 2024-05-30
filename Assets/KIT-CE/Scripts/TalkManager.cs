using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    List<string> talkData;
    string[] talk;

    private void Awake()
    {
        talkData = new List<string>();
        GenerateData();
    }

    void GenerateData()
    {
        talk = new string[] { "�� ���� �����ϴ� �׳� ������ �����ϰ� �����ϱ���.", 
            "�������� �л���� �ùε��� ����ϰ� �ӿ��� �ʸ�", 
            "���� �����Ϸ� �Դ�." };
    }

    public string GetTalk(int talkIndex)
    {
        return talkData[talkIndex];
    }

}
