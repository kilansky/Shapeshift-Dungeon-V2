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
    [SerializeField] private float minZoom = 9;
    [SerializeField] private float maxZoom = 15;
    [SerializeField] private float zoomSpeed = 1f;
    [SerializeField] private float shadowAdjustAmt = -0.012f;

    private DepthOfField DoF;
    private ShadowsMidtonesHighlights shadMidHigh;
    private float shadowValue = 1;
    private float currentZoom;
    private bool canAdjustZoom = true;

    private void Start()
    {
        volProf.TryGet(out DoF);
        volProf.TryGet(out shadMidHigh);
        SetShadows();
    }


    //Increases the camera's distance from the player during a level transition
    public void ZoomOutLevelTransition()
    {
        canAdjustZoom = false;

        var framingTransposer = playerCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        currentZoom = framingTransposer.m_CameraDistance;
        framingTransposer.m_CameraDistance = zoomOutCamDist;

        DoF.focalLength.value = 185;
    }

    //Decreases the camera's distance from the player to the normal amt after a level transition
    public void ZoomInLevelTransition()
    {
        canAdjustZoom = true;

        var framingTransposer = playerCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        framingTransposer.m_CameraDistance = currentZoom;

        DoF.focalLength.value = 215;
    }

    //Increases the camera's distance from the player - called from player controller w/ player input
    public void ZoomOut()
    {
        if (!canAdjustZoom) //Prevent player from zooming camera during level transitions
            return;

        //Set the amt to zoom based on if the player is using a scroll wheel or joystick
        float zoomAmt = (PlayerController.Instance.IsUsingMouse ? 0.5f : 0.1f) * zoomSpeed;

        var framingTransposer = playerCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        framingTransposer.m_CameraDistance = Mathf.Clamp(framingTransposer.m_CameraDistance += zoomAmt, minZoom, maxZoom);
    }

    //Decreases the camera's distance from the player - called from player controller w/ player input
    public void ZoomIn()
    {
        if (!canAdjustZoom) //Prevent player from zooming camera during level transitions
            return;

        //Set the amt to zoom based on if the player is using a scroll wheel or joystick
        float zoomAmt = (PlayerController.Instance.IsUsingMouse ? 0.5f : 0.1f) * zoomSpeed;

        var framingTransposer = playerCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        framingTransposer.m_CameraDistance = Mathf.Clamp(framingTransposer.m_CameraDistance -= zoomAmt, minZoom, maxZoom);
    }

    //Increase the darkness of the post processing slightly each time this is called
    public void SetShadows()
    {
        shadowValue += shadowAdjustAmt;
        shadMidHigh.shadows.SetValue(new Vector4Parameter(new Vector4(1f, shadowValue, shadowValue, -0.4f)));
    }
}
