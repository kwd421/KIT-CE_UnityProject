using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public int health;

    // 기본 생성자 안의 값은 Default 생성값
    // 로드할 데이터가 없을 시 Default값으로 시작
    public GameData()
    {
        this.health = 3;
    }
}
