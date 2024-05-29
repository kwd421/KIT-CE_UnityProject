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
        // 보스전일때 카메라이동X
        if (currentStage.name.Contains("Boss"))
        {
            StartCoroutine(CameraMove());
        }
        else
        {
            GetComponent<CinemachineVirtualCamera>().Follow = GameManager.instance.player.transform;
            // PolygonCollider2D가 있는 Object는 Cam Border뿐
            camBorder = currentStage.GetComponentInChildren<PolygonCollider2D>();
            vCam.m_BoundingShape2D = camBorder;
        }
    }
        
    IEnumerator CameraMove()
    {
        // 화면 제한 영역 제거
        vCam.m_BoundingShape2D = null;
        // 스테이지 시작 시 잠시동안 못 움직이게 하기(컷씬?)
        GameManager.instance.player.moveSpeed = 0f;
        GameManager.instance.player.jumpPower = 0f;
        yield return new WaitForSeconds(1f);
        GameManager.instance.player.moveSpeed = 4.5f;
        GameManager.instance.player.jumpPower = 22f;
        GetComponent<CinemachineVirtualCamera>().Follow = null;
    }
}
