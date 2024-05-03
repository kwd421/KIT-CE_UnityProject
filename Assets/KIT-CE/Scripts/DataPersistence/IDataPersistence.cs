using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistence
{
    void LoadData(GameData data);

    // ref: ���� �Ű������� ���� ��������(return�ʿ� X). �̶� ���� �Ű������� �ʱ�ȭ�� �� ���� �޾ƾ���.
    // out: ���� �Ű������� ���� ��������(return�ʿ� X). �̶� ���� �Ű������� �ʱ�ȭ�� ���� �ʾƵ� ��.
    // >>> Call By Reference <<<
    void SaveData(ref GameData data);
}
