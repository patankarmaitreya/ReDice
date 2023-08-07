using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class FollowCamera : MonoBehaviour
{

    public event Action onZoomOutAction;

    public GameObject color;
    public GameObject eventSystem;
    public GameObject plaformOrigin;

    private PlatformColor platformColor;
    private PlatformChange platformChange;
    private PlatformRotate platformRotate;

    private PlayerInputSys playerInput;
    private Transform cameraTransform;

    private Vector3 initialCameraPosition;

    private Vector3 finalCameraPosition;
    private Vector3 cameraRestPosition;

    private Quaternion finalCameraRotaion;
    private Quaternion cameraRestRotation;

    [SerializeField] private Transform cameraRotation;

    [SerializeField] private float interpolationTime;
    private float elapsedTime = 0;

    private Transform target;
    private float zoomTrigg = -1;

    private int rotateAxis;

    private void Awake()
    {
        platformChange = eventSystem.GetComponent<PlatformChange>();
        platformRotate = plaformOrigin.GetComponent<PlatformRotate>();
        platformColor = color.GetComponent<PlatformColor>();    

        target = platformChange.GetDice.transform;

        playerInput = new PlayerInputSys();
        cameraTransform = transform.GetChild(0);
        initialCameraPosition = cameraTransform.localPosition;

        finalCameraPosition = new Vector3(255, 150, -285);
        finalCameraRotaion = cameraRotation.rotation;

        cameraRestRotation = cameraTransform.rotation;
    }

    private void OnEnable()
    {
        playerInput.Camera.Enable();
        playerInput.Camera.ZoomOut.performed += Camera_performed;
        platformRotate.onRotationAction += ZoomBackTrigg;
    }

    private void ZoomBackTrigg()
    {
        zoomTrigg = 0;
        elapsedTime = 0;
        StartCoroutine(PlatformChange.GetInstance().ChangePlatform());
        EventSystem.GetInstance().pauseSpawning = false;
    }

    private void OnDisable()
    {
        playerInput.Camera.Disable();
    }
    private void Camera_performed(InputAction.CallbackContext context)
    {
        //this if statement check if dice roll can be used
        if(EventSystem.GetInstance().RollAvailable)
        {
            EventSystem.GetInstance().RollAvailable = false;
            if (zoomTrigg == -1)
            {
                target.GetComponent<PlayerController>().enabled = false;

                zoomTrigg = 1;
                elapsedTime = 0;
            }
            else if (zoomTrigg == 1)
            {
                zoomTrigg = 0;
                elapsedTime = 0;
            }
        }
    
    }
    public void Camera_performed()
    {
        //this if statement check if dice roll can be used
        if (EventSystem.GetInstance().RollAvailable)
        {
            EventSystem.GetInstance().RollAvailable = false;
            if (zoomTrigg == -1)
            {
                target.GetComponent<PlayerController>().enabled = false;

                zoomTrigg = 1;
                elapsedTime = 0;
            }
            else if (zoomTrigg == 1)
            {
                zoomTrigg = 0;
                elapsedTime = 0;
            }
        }

    }

    void LateUpdate()
    {
        if(target!=null)
        {
            transform.position = target.position;
            cameraRestPosition = initialCameraPosition * 20 + transform.position;
        }
        else
        {
            if(GameObject.FindGameObjectWithTag("Player"))
            {
                target = platformChange.GetDice.transform;
            }
        }

        if (zoomTrigg == 1)
        {
            playerInput.Camera.ZoomOut.performed -= Camera_performed;
            StartCoroutine(ZoomOperation());
        }
        else if (zoomTrigg == 0)
        {
            playerInput.Camera.ZoomOut.performed -= Camera_performed;
            ZoomBack();
        }
    }

    private IEnumerator ZoomOperation()
    {
        EventSystem.GetInstance().pauseSpawning = true;
        if (elapsedTime < interpolationTime)
        {
            cameraTransform.position = Vector3.Lerp(cameraRestPosition, finalCameraPosition, elapsedTime / interpolationTime);
            cameraTransform.rotation = Quaternion.Lerp(cameraRestRotation, finalCameraRotaion, elapsedTime / interpolationTime);

            elapsedTime += Time.deltaTime;
        }
        else
        {
            cameraTransform.position = finalCameraPosition;
            cameraTransform.rotation = finalCameraRotaion;

            if(onZoomOutAction != null)
            {
                onZoomOutAction();
            }
            zoomTrigg = 2;
            playerInput.Camera.ZoomOut.performed += Camera_performed;
        }

        yield return new WaitForSeconds(1.5f);

        EventSystem.GetInstance().RemoveExsistingObstacleMinesOrbs();

        if (GameObject.FindGameObjectWithTag("Player"))
        {
            Destroy(GameObject.FindGameObjectWithTag("Player"));
        }
    }
    private void ZoomBack()
    {
        if (elapsedTime < interpolationTime)
        {
            cameraTransform.position = Vector3.Lerp(finalCameraPosition, cameraRestPosition, elapsedTime / interpolationTime);
            cameraTransform.rotation = Quaternion.Lerp(finalCameraRotaion, cameraRestRotation, elapsedTime / interpolationTime);

            elapsedTime += Time.deltaTime;
        }
        else
        {
            cameraTransform.position = cameraRestPosition;
            cameraTransform.rotation = cameraRestRotation;
            zoomTrigg = -1;

            playerInput.Camera.ZoomOut.performed += Camera_performed;
            target.GetComponent<PlayerController>().enabled = true;
        }
    }
}
