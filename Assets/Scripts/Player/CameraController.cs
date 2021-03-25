using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class CameraController : SingletonPattern<CameraController>
{
    [SerializeField] private CinemachineVirtualCamera playerCam;
    [SerializeField] private VolumeProfile volProf;
    [SerializeField] private float baseCamDist = 12;
    [SerializeField] private float zoomOutCamDist = 25;

    private DepthOfField DoF;

    private void Start()
    {
        volProf.TryGet(out DoF);
    }

    //Increases the camera's distance from the player during a level transition
    public void ZoomOut()
    {
        var framingTransposer = playerCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        framingTransposer.m_CameraDistance = zoomOutCamDist;

        DoF.focalLength.value = 180;
    }

    //Decreases the camera's distance from the player to the normal amt after a level transition
    public void ZoomIn()
    {
        var framingTransposer = playerCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        framingTransposer.m_CameraDistance = baseCamDist;

        DoF.focalLength.value = 215;
    }
}
