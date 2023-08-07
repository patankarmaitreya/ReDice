using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class PlatformRotate : MonoBehaviour
{

    public event Action onRotationAction;

    public GameObject followCameraGO;
    private FollowCamera followCamera;

    private PlayerInputSys playerInput;
    private float elapsedTime = 0;

    [SerializeField] private float interpolationTime;

    private int rotateTrigg = 0;

    private int rotateAxis;
    private int numberOfRotations;
    private int rotationsCompleted = 0;

    private Vector3 curretRotation;

    bool rotationFin = true;
    private void Awake()
    {
        followCamera = followCameraGO.GetComponent<FollowCamera>();

        playerInput = new PlayerInputSys();
        curretRotation = transform.rotation.eulerAngles;
        rotateAxis = Random.Range(0, 3);
    }

    private void OnEnable()
    {
        playerInput.Platform.Enable();

        playerInput.Platform.Rotate.performed += Rotate_performed;
        followCamera.onZoomOutAction += RotatePlatformTrigg;
    }

    private void RotatePlatformTrigg()
    {
        rotateTrigg = 1;
        numberOfRotations = Random.Range(3, 15);
    }

    private void OnDisable()
    {
        playerInput.Platform.Disable();
    }

    private void Rotate_performed(InputAction.CallbackContext obj)
    {
        if (rotateTrigg == -1)
        {
            rotateAxis = Random.Range(0,3);

            rotateTrigg = 1;
            elapsedTime = 0;
        }
    }

    float minAngleX = 0.0f;
    float maxAngleX = 90.0f;
    float minAngleY = 0.0f;
    float maxAngleY = 90.0f;
    float minAngleZ = 0.0f;
    float maxAngleZ = 90.0f;

    private void Update()
    {
        if(rotateTrigg == 1 && rotationFin)
        {
            StartCoroutine(RotatePlatform());
        }
        else if(rotateTrigg == -1)
        {
            if(rotationsCompleted < numberOfRotations)
            {
                rotateAxis = Random.Range(0, 3);
                rotateTrigg = 1;
                rotationsCompleted++;

            }
            else
            {
                if (onRotationAction != null)
                {
                    onRotationAction();
                }
                rotateTrigg = 0;
                rotationsCompleted = 0;
                justStarted = true;
            }
        }
    }

    bool justStarted = true;
    public IEnumerator RotatePlatform()
    {
        rotationFin = false;

        if (justStarted)
        {
            yield return new WaitForSeconds(1.5f);
            justStarted = false;
        }

        if(elapsedTime < interpolationTime)
        {
            switch(rotateAxis)
            {
                case 0:
                    float angleX = Mathf.LerpAngle(minAngleX, maxAngleX, elapsedTime / interpolationTime);
                    transform.eulerAngles = curretRotation + new Vector3(angleX, 0, 0);
                    break;
                case 1:
                    float angleY = Mathf.LerpAngle(minAngleY, maxAngleY, elapsedTime / interpolationTime);
                    transform.eulerAngles = curretRotation + new Vector3(0, angleY, 0);
                    break;
                case 2:
                    float angleZ = Mathf.LerpAngle(minAngleZ, maxAngleZ, elapsedTime / interpolationTime);
                    transform.eulerAngles = curretRotation + new Vector3(0, 0, angleZ);
                    break;
            }
            

            elapsedTime += Time.deltaTime;
        }
        else
        {
            switch (rotateAxis)
            {
                case 0:
                    curretRotation += new Vector3(90, 0, 0);
                    transform.eulerAngles = curretRotation;
                    elapsedTime = 0;
                    break;
                case 1:
                    curretRotation += new Vector3(0, 90, 0);
                    transform.eulerAngles = curretRotation;
                    elapsedTime = 0;
                    break;
                case 2:
                    curretRotation += new Vector3(0, 0, 90);
                    transform.eulerAngles = curretRotation;
                    elapsedTime = 0;
                    break;
            }
            rotateTrigg = -1;

            playerInput.Platform.Rotate.performed += Rotate_performed;
        }
        rotationFin = true;
        yield return null;
    }
}
