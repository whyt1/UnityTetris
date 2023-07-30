using System.Collections;
using UnityEngine;

public class SC_BlockMovments : MonoBehaviour
{
    #region Variables
    public delegate void ImDoneFalling();
    public static event ImDoneFalling OnImDoneFalling;

    public delegate void GameOver();
    public static event GameOver OnGameOver;
        
    public bool isNext = true;
    public bool isCurrent = false;
    public bool isFalling = false;
    public int speedUpAfter = 20;
    public float fallSpeed = 0.01f;
    public float defultFallSpeed = 0.01f;
    private Vector3 Velocity;
    public float rotationSpeed = -0.1f;
    public int stepSize = 1;
    private Vector3 StepLeft;
    private Vector3 StepRight;
    #endregion

    #region MonoBehaviour
    void Awake()
    {
        InitVariables();
        if (SC_MainLogic.numberOfBlocks == 0)
        {
            StartFalling();
        }
    }
    void OnEnable()
    {
        OnImDoneFalling += StartFalling;
        SC_MainLogic.OnRestart += OnRestart;
    }
    void OnDisable()
    {
        OnImDoneFalling -= StartFalling;
        SC_MainLogic.OnTogglePause -= OnTogglePause;
        SC_MainLogic.OnRestart -= OnRestart;
        SC_CubeLogic.OnRowDone -= OnRowDone;
    }
    void OnTriggerEnter()
    {
        if (isFalling)
        {
            isFalling = false;
            transform.position = new Vector3(transform.position.x, Mathf.RoundToInt(transform.position.y-0.5f)+0.5f, transform.position.z);
        }
        if (isCurrent)
        {
            FinishFalling();
        }
    }
    void Update()
    {
        if (isNext)
        {
            transform.Rotate((transform.up) * rotationSpeed);
        }
        if (isCurrent) 
        {
            HandleInputs();
        }
        if (isFalling)
        {
            transform.Translate(Velocity);
        }
    }
    #endregion

    #region Logic
    private void InitVariables()
    {
        fallSpeed += 0.005f * SC_MainLogic.numberOfBlocks/speedUpAfter;
        Velocity = new(0, -fallSpeed, 0);
        StepLeft = new(-stepSize, 0, 0);
        StepRight = new(stepSize, 0, 0);
    }
    private void StartFalling()
    {
        isNext = false;
        isCurrent = true;
        isFalling = true;

        transform.rotation = new Quaternion(0, 0, 0, 0);
        transform.position = new Vector3(Random.Range(-1, 1) + 0.5f,18,0);


        SC_CubeLogic.OnRowDone += OnRowDone;
        SC_MainLogic.OnTogglePause += OnTogglePause;
    }
    private void FinishFalling()
    {
        isCurrent = false;
        isFalling = false;
        SC_MainLogic.OnTogglePause -= OnTogglePause;
        Velocity = Velocity.normalized * defultFallSpeed;
        
        OnImDoneFalling -= StartFalling;
        if (OnImDoneFalling != null)
        {
            Debug.Log(name + " is done falling");
            OnImDoneFalling();
        }
        
        if (CheckGameOver() && OnGameOver != null)
        {
            OnGameOver();
        }
    }
    private bool CheckForCollision(Vector3 direction)
    {
        RaycastHit hit;
        foreach (Transform cube in GetComponentsInChildren<Transform>())
        {
            if (!Physics.Raycast(cube.position, direction, out hit, 0.5f)) {
                continue;
            }
            if (hit.transform == transform) {
                continue;
            }
            Debug.Log("Future collision detected with " + hit.transform.name);
            return true;
        }
        return false;           
    }
    private void HandleInputs()
    {
        // Sideways movement 
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (CheckForCollision(Vector3.right)) { return; }
            transform.Translate(StepRight);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (CheckForCollision(Vector3.left)) { return; }
            transform.Translate(StepLeft);
        }
        // rotations 
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (CheckForCollision(Vector3.right)) { return; }
                RotateClockwise();
            }
            else
            {
                if (CheckForCollision(Vector3.left)) { return; }
                RotateCounterClockwise();
            }
        }
        // Go down fast!! 
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Velocity = Velocity.normalized * fallSpeed * 10;
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            Velocity = Velocity.normalized * fallSpeed;
        }
    }
    private void RotateCounterClockwise()
    {
        transform.Rotate(new(0, 0, 90));
        Velocity.Set(Velocity.y, -Velocity.x, 0);
        StepLeft.Set(StepLeft.y, -StepLeft.x, 0);
        StepRight.Set(StepRight.y, -StepRight.x, 0);
    }
    private void RotateClockwise()
    {
        transform.Rotate(new(0, 0, -90));
        Velocity.Set(-Velocity.y, Velocity.x, 0);
        StepLeft.Set(-StepLeft.y, StepLeft.x, 0);
        StepRight.Set(-StepRight.y, StepRight.x, 0);
    }
    private void OnRowDone(float row)
    {
        if (transform.childCount == 0)
        {
            Destroy(gameObject);
            return;
        }
        foreach (Transform cube in gameObject.GetComponentsInChildren<Transform>())
        {
            Debug.Log(name + "has cube in " + (cube.position.x, cube.position.y));
            int y = Mathf.RoundToInt(cube.position.y);
            if (row <= y && !isNext){
                StartCoroutine(FallDelay());
                return;
            }
        }
        
    }
    private void OnTogglePause()
    {
        isCurrent = !isCurrent;
        isFalling = !isFalling;
    }
    private bool CheckGameOver()
    {
        foreach (Transform cube in GetComponentsInChildren<Transform>())
        {
            if (cube.position.y >= 14)
            {
                return true;
            }
        }
        return false;
    }
    private void OnRestart()
    {
        Destroy(gameObject);
    }
    IEnumerator FallDelay()
    {
        yield return new WaitForFixedUpdate();
        isFalling = true;
    }
    #endregion
}
