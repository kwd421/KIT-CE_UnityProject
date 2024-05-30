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
        talk = new string[] { "내 가만 보아하니 네놈 눈에는 마구니가 가득하구나.", 
            "거짓으로 학생들과 시민들을 우롱하고 속여온 너를", 
            "오늘 단죄하러 왔다." };
    }

    public string GetTalk(int talkIndex)
    {
        return talkData[talkIndex];
    }

}
