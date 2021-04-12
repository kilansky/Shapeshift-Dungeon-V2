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
    [SerializeField] private float shadowAdjustAmt = -0.012f;


    private DepthOfField DoF;
    private ShadowsMidtonesHighlights shadMidHigh;
    private float shadowValue = 1;

    private void Start()
    {
        volProf.TryGet(out DoF);
        volProf.TryGet(out shadMidHigh);
        SetShadows();
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

    public void SetShadows()
    {
        shadowValue += shadowAdjustAmt;
        shadMidHigh.shadows.SetValue(new Vector4Parameter(new Vector4(1f, shadowValue, shadowValue, -0.4f)));
    }
}
