using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCamera : MonoBehaviour
{
    CinemachineConfiner2D vCam;
    PolygonCollider2D camBorder;

    private void Awake()
    {
        vCam = GetComponent<CinemachineConfiner2D>();
    }
    void Start()
    {
        
    }

    public void MapBorder(GameObject currentStage)
    {
        // �������϶� ī�޶��̵�X
        if (currentStage.name.Contains("Boss"))
        {
            StartCoroutine(CameraMove());
        }
        else
        {
            GetComponent<CinemachineVirtualCamera>().Follow = GameManager.instance.player.transform;
            // PolygonCollider2D�� �ִ� Object�� Cam Border��
            camBorder = currentStage.GetComponentInChildren<PolygonCollider2D>();
            vCam.m_BoundingShape2D = camBorder;
        }
    }
        
    IEnumerator CameraMove()
    {
        // ȭ�� ���� ���� ����
        vCam.m_BoundingShape2D = null;
        // �������� ���� �� ��õ��� �� �����̰� �ϱ�(�ƾ�?)
        GameManager.instance.player.moveSpeed = 0f;
        GameManager.instance.player.jumpPower = 0f;
        yield return new WaitForSeconds(1f);
        GameManager.instance.player.moveSpeed = 4.5f;
        GameManager.instance.player.jumpPower = 22f;
        GetComponent<CinemachineVirtualCamera>().Follow = null;
    }
}
