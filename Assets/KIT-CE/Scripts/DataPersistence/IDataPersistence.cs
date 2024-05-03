using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistence
{
    void LoadData(GameData data);

    // ref: 받은 매개변수를 직접 수정가능(return필요 X). 이때 받은 매개변수는 초기화가 된 것을 받아야함.
    // out: 받은 매개변수를 직접 수정가능(return필요 X). 이때 받은 매개변수는 초기화가 되지 않아도 됨.
    // >>> Call By Reference <<<
    void SaveData(ref GameData data);
}
