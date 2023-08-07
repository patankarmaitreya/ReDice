using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class PlayerController : MonoBehaviour
{
    //Input System              #Start

    private PlayerInputSys playerInput;
    internal EventSystem currentEventSys;

    private float xAxis;
    private float zAxis;

    public bool isMoving;

    //Input System              #End

    //Movement Data             #Start

    [Tooltip("Enter in the order of increasing speed , but they must perfectly devide 90")]
    public int[] rollSpeeds;

    internal int currentSpeed;

    //Movement Data             #End

    //Dice Sides                #Start

    private Transform[] sides;

    //Dice Sides                #End

    public float collidedGOScore;
    internal int[,] playerOccupiedPos;
    private Transform groundGO;
    int multiplier = 1;

    [SerializeField]
    private GameObject deathVFX;
    public PlayerController Init(EventSystem eventSystem)
    {
        currentEventSys = eventSystem;

        return this;
    }

    private void Awake()
    {
        playerInput = new PlayerInputSys();
        playerOccupiedPos = new int[1, 2];
        sides = new Transform[6];
        currentSpeed = rollSpeeds[0];

        for (int i = 0; i < transform.childCount; i++)
        {
            sides[i] = transform.GetChild(i);
        }
    }

    private void Update()
    {
        for (int i = 0; i < sides.Length; i++)
        {
            bool isGrounded = false;
            CheckEnvironment(sides[i], out isGrounded);
            if (isGrounded)
            {
                groundGO = sides[i];
                switch (groundGO.name)
                {
                    case "1":
                        multiplier = 6;
                        break;

                    case "2":
                        multiplier = 5;
                        break;

                    case "3":
                        multiplier = 4;
                        break;

                    case "4":
                        multiplier = 3;
                        break;

                    case "5":
                        multiplier = 2;
                        break;

                    case "6":
                        multiplier = 1;
                        break;
                }
                break;
            }
        }
    }

    bool playerFallen = false;
    bool isDead = false;
    void FixedUpdate()
    {
        if(!playerFallen)
        {
            if (!CheckForFall())
            {
                if (currentEventSys != null)
                {
                    if (isMoving)
                        return;

                    if (xAxis != 0)
                    {
                        if (xAxis < 0)
                        {
                            bool isObstacle = (Physics.Raycast(this.transform.position,
                                 Vector3.left
                                 , 21, 1 << LayerMask.NameToLayer("Obstacle")));
                            if (!isObstacle)
                            {
                                //left
                                TurnLeft();
                            }
                        }
                        else if (xAxis > 0)
                        {
                            bool isObstacle = (Physics.Raycast(this.transform.position,
                                 Vector3.right
                                 , 21, 1 << LayerMask.NameToLayer("Obstacle")));
                            if (!isObstacle)
                            {
                                //right
                                TurnRight();
                            }
                        }
                    }
                    if (zAxis != 0)
                    {
                        //StartCoroutine(RotatePlatform_ZAxis(zAxis));

                        if (zAxis < 0)
                        {
                            bool isObstacle = (Physics.Raycast(this.transform.position,
                                 Vector3.back
                                 , 21, 1 << LayerMask.NameToLayer("Obstacle")));
                            if (!isObstacle)
                            {
                                //left
                                TurnBackward();
                            }
                        }
                        else if (zAxis > 0)
                        {
                            bool isObstacle = (Physics.Raycast(this.transform.position,
                                 Vector3.forward
                                 , 21, 1 << LayerMask.NameToLayer("Obstacle")));
                            if (!isObstacle)
                            {
                                //right
                                TurnForward();
                            }
                        }
                    }
                }
            }
        }
        else if(playerFallen && !isDead)
        {
            ScoreSystem.GetInstance().UpdateLife(3);

            StartCoroutine(FallAction());

            playerFallen = false;
            isDead = true;
        }
    }

    //===================================================================================================================//
    //Input System          #Start

    private void OnEnable()
    {
        playerInput.Movement.Enable();

        playerInput.Movement.XZMovement.performed += OnClick_XMovement;
        playerInput.Movement.XZMovement.canceled += OnCanceled_XMovement;

        playerInput.Movement.XZMovement.performed += OnClick_ZMovement;
        playerInput.Movement.XZMovement.canceled += OnCanceled_ZMovement;
    }

    private void OnDisable()
    {
        playerInput.Movement.Disable();
    }

    private void OnClick_XMovement(InputAction.CallbackContext context)
    {
        xAxis = context.ReadValue<Vector2>().x;
        if (xAxis != 0)
            zAxis = 0;
    }

    private void OnCanceled_XMovement(InputAction.CallbackContext context)
    {
        xAxis = 0;
    }

    private void OnClick_ZMovement(InputAction.CallbackContext context)
    {
        zAxis = context.ReadValue<Vector2>().y;
        if (zAxis != 0)
            xAxis = 0;
    }

    private void OnCanceled_ZMovement(InputAction.CallbackContext context)
    {
        zAxis = 0;
    }

    //Input System          #End
    //===================================================================================================================//


    //===================================================================================================================//
    //Dice Movement          #Start

    private Vector3 CalculateAnchor(float dir1, float dir2, float dir3)
    {
        float diceSide = transform.localScale.x;
        Vector3 anchor = transform.position + new Vector3(dir1 * diceSide, dir2 * diceSide, dir3 * diceSide);

        return anchor;
    }

    public void TurnLeft()
    {
        Vector3 anchor = CalculateAnchor(-0.5f, -0.5f, 0);

        //Axis to rotate around;
        Vector3 axis = Vector3.Cross(Vector3.up, Vector3.left);
        StartCoroutine(RollingMovement(anchor, axis, currentSpeed));
    }

    public void TurnRight()
    {
        Debug.Log("in3");
        Vector3 anchor = CalculateAnchor(0.5f, -0.5f, 0);

        //Axis to rotate around;
        Vector3 axis = Vector3.Cross(Vector3.up, Vector3.right);
        StartCoroutine(RollingMovement(anchor, axis, currentSpeed));
    }

    public void TurnForward()
    {
        Vector3 anchor = CalculateAnchor(0, -0.5f, 0.5f);

        //Axis to rotate around;
        Vector3 axis = Vector3.Cross(Vector3.up, Vector3.forward);
        StartCoroutine(RollingMovement(anchor, axis, currentSpeed));
    }

    public void TurnBackward()
    {
        Vector3 anchor = CalculateAnchor(0, -0.5f, -0.5f);

        //Axis to rotate around;
        Vector3 axis = Vector3.Cross(Vector3.up, Vector3.back);
        StartCoroutine(RollingMovement(anchor, axis, currentSpeed));
    }

    private IEnumerator RollingMovement(Vector3 anchor, Vector3 axis, float rollSpeed)
    {
        isMoving = true;
        Vector2 prevPos = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        int x = 0, z = 0;

        currentEventSys.setSurroundingGrid(2, false);
        for (int i = 0; i < (90 / rollSpeed); i++)
        {
            Debug.Log(rollSpeed);
            transform.RotateAround(anchor, axis, rollSpeed);
            yield return new WaitForSeconds(1f/10000000);
        }
        //Previous Pos

        currentEventSys.calculateCell(prevPos, out x, out z);
        currentEventSys.Grid[x, z] = false;


        //Next Pos
        Vector3 nextPos = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));

        currentEventSys.calculateCell(nextPos, out x, out z);
        if (CheckForFall())
        {
            yield break;
        }

        currentEventSys.Grid[x, z] = true;

        playerOccupiedPos[0, 0] = x;
        playerOccupiedPos[0, 1] = z;

        currentEventSys.setSurroundingGrid(2, true);

        this.GetComponent<AudioSource>().Play();

        isMoving = false;

        if (collidedGOScore != 0)
        {
            if (ScoreSystem.GetInstance().currentScore + ((int)collidedGOScore * multiplier) > 0)
                ScoreSystem.GetInstance().UpdateScores((int)collidedGOScore * multiplier);
            else
                ScoreSystem.GetInstance().UpdateScores(-ScoreSystem.GetInstance().currentScore);
        }
        collidedGOScore = 0;
    }

    private IEnumerator FallAction()
    {
        for (float i=0;i<=1;i+=0.1f)
        {
            transform.position += new Vector3(0, 5f, 0);
            yield return new WaitForSeconds(0.02f);
        }
        for (float i = 0; i <= 3; i += 0.1f)
        {
            transform.RotateAround(this.transform.position, Vector3.up, 30f);
            yield return new WaitForSeconds(0.03f);
        }

        GetComponent<MeshRenderer>().enabled = false;

        GameObject DeathVFX=Instantiate(deathVFX);
        DeathVFX.transform.position = transform.position;

        yield return new WaitUntil(() => !DeathVFX.GetComponent<ParticleSystem>().isPlaying);

        ScoreSystem.GetInstance().visualLeft = true;
        ScoreSystem.GetInstance().UpdateLife(0);
    }

    public bool CheckForFall()
    {
        Vector3 currentPos = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        int x, z;
        currentEventSys.calculateCell(currentPos, out x, out z);
        if(x<0 || x>14 || z<0 || z>14)
        {
            playerFallen = true;
            return true;
        }
        return false;
    }

    //Dice Movement          #End
    //===================================================================================================================//


    //Raycast From all 6 faces          #Start
    //===================================================================================================================//

    private void CheckEnvironment(Transform side, out bool isGrounded)
    {
        float extraHeight = 1f;

        isGrounded = Physics.Raycast(side.position,
            new Vector3(side.position.x - transform.position.x, side.position.y - transform.position.y,
            side.position.z - transform.position.z),
            extraHeight, 1 << LayerMask.NameToLayer("Platform"));
        //Debug.DrawRay(sides[0].position, new Vector3(side.position.x - transform.position.x, side.position.y - transform.position.y,
        //    side.position.z - transform.position.z) * 10, Color.red);
    }
}
