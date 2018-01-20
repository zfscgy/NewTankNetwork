using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {


    public float distance = 5;
    public const float maxDistance = 15;
    public const float minDistance = -3;


    private float rotateSpeed;
    public float rotateSpeed1 = 2;
    public float rotateSpeed2 = 0.5f;

    public float verticalMax = 0;
    public float verticalMin = -90;
    public float magnifyFov = 10;
    private float defaultFov = 65;
    public float zoomspeed = 1f;

    public TankControl tank;
    public Transform cameraPos;
    public Transform cameraHolder;
    // Use this for initialization


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rotateSpeed = rotateSpeed1;
    }
	// Update is called once per frame
	void Update () {
        ControlZoom();
        ControlCamera();
        ControlCursor();
	}

    //通过滚轮进行相机距离改变
    public void ControlZoom()
    {
        //向下滚动滚轮
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (distance < maxDistance)
            {
                distance += zoomspeed;
            }
        }
        //向上滚动滚轮
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (distance > minDistance)
            {
                distance -= zoomspeed;
            }

        }
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            if (distance < minDistance + 1)
            {
                tank.cameraState = 1;
                GetComponent<Camera>().fieldOfView = magnifyFov;
                rotateSpeed = rotateSpeed2;
                transform.localPosition = new Vector3(0, 0f, -distance);
            }
            else
            {
                tank.cameraState = 0;
                rotateSpeed = rotateSpeed1;
                GetComponent<Camera>().fieldOfView = defaultFov;
                transform.localPosition = new Vector3(0, 2f, -distance);
            }
            
        }

    }
    public void ControlCamera()
    {
        float mouseX = -Input.GetAxis("Mouse Y");
        float mouseY = Input.GetAxis("Mouse X");
        if ((cameraHolder.localEulerAngles.x - 360f) > -verticalMax || cameraHolder.localEulerAngles.x < -verticalMin)
        {
            cameraHolder.localEulerAngles += new Vector3(rotateSpeed * mouseX, 0, 0);
        }

        if (cameraHolder.localEulerAngles.x < 100f && cameraHolder.localEulerAngles.x >= -verticalMin)
        {
            cameraHolder.localEulerAngles = new Vector3(-verticalMin - 0.1f, cameraHolder.localEulerAngles.y, 0);
        }
        else if (cameraHolder.localEulerAngles.x >= 100f && (cameraHolder.localEulerAngles.x - 360f) <= -verticalMax)
        {
            cameraHolder.localEulerAngles = new Vector3(360f - verticalMax + 0.1f, cameraHolder.localEulerAngles.y, 0);
        }
        cameraPos.localEulerAngles += new Vector3(0, rotateSpeed * mouseY, 0);
    }
    private byte cursorMode = 0;
    private void ControlCursor()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (cursorMode == 0)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                cursorMode = 1;
            }
            else if(cursorMode==1)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                cursorMode = 0;
            }
        }
    }
}
