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
        // PolygonCollider2D�� �ִ� Object�� Cam Border��
        camBorder = currentStage.GetComponentInChildren<PolygonCollider2D>();
        vCam.m_BoundingShape2D = camBorder;
        Debug.Log(camBorder.name);
    }
}
