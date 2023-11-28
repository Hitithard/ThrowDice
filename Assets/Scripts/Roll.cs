using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Roll : MonoBehaviour
{
    [SerializeField] LayerMask raycastLayer; //Assign this to Dice Layer, make sure all dices have this layer and only dices.

    public float torqueForce = 1000.0f;  // Adjust this value to control the force of the auto-roll.

    //Throwing Dice
    private Rigidbody rb;
    [SerializeField] float maxThrowForce = 10f;

    [SerializeField] GameObject particleEffect;     // Assign this to VFX Prefab
    [SerializeField] float VFXDestroyTimer = 1f;    // Set timer to destroy VFX
    public RollManager gameManager; // Assign this to GameManager with a RollManager script

    [SerializeField] GameObject spawnDiceParticle;     // Assign this to VFX Prefab 



    private Vector3 initialPosition;
    private Camera mainCamera;
    private bool isHolding = false;
    public bool isRolling = false;
    private float rollCountdown = 0f;
    private float minRollSpeed = 0.1f;   // Minimum speed to consider the roll stops.

    void Start()
    {
        if (gameManager == null)
            gameManager = GameObject.Find("GameManager").GetComponent<RollManager>();

        initialPosition = transform.position;
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
    }   

    void Update()
    {
        //Autoroll after pressing Space
        if (!isRolling && (Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0))
            AutoRollDice();

        if (!isRolling && Input.GetMouseButtonDown(0) && !isHolding)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastLayer) && hit.collider.gameObject == gameObject)
            {
                isHolding = true;
                rb.useGravity = false;
                if (gameManager.dragRoll)
                    rb.isKinematic = true;
                if (gameManager.rollRotation)
                    RotateDice();
            }
        }

        if (isHolding)
            MoveDiceWithMouse();

        if (Input.GetMouseButtonUp(0) && isHolding)     
                ThrowDice();
    }

    void FixedUpdate()
    {
        
        // Check if the dice's velocity is below the minimum roll speed.
        if (isRolling && rb.velocity.magnitude < minRollSpeed)
        {   
            if (rollCountdown <= 0f && rb.velocity.magnitude < 0.01f)
            {
                isRolling = false;
                int result = CalculateResult();
                gameManager.RollResult(result);

                // Spawn praticles on Result
                if (particleEffect != null)
                {
                    GameObject VFX = Instantiate(particleEffect, transform.position, transform.rotation);
                    Destroy(VFX, VFXDestroyTimer);
                }
            } 
            rollCountdown -= Time.deltaTime;
        }

    }

    private void MoveDiceWithMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 newPosition = ray.GetPoint(12.5f);
        rb.MovePosition(ClampPosition(newPosition));
        rb.velocity = (newPosition - transform.position)*80f* Time.deltaTime;

        if (gameManager.dragRoll)
        {                   
            // Rotate the dice based on mouse movement
            transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * 5f, Space.World);
            transform.Rotate(Vector3.right, Input.GetAxis("Mouse Y") * 5f, Space.World);
        }
    }

    private void ThrowDice()
    {              
        rb.isKinematic = false;
        isHolding = false;
        rb.useGravity = true;

        // Calculate the throw velocity based on mouse movement
        float throwPower = Mathf.Clamp(rb.velocity.magnitude/ Time.deltaTime/ gameManager.throwPowerDivision, 0.1f, maxThrowForce);
 
        if (gameManager.dragRoll)
        {
            Vector3 throwDirection = mainCamera.transform.forward;
            rb.AddForce(throwDirection * 1000f, ForceMode.Impulse);
            RotateDice();
        }
        else
        {
            Vector3 throwDirection = mainCamera.transform.forward + mainCamera.transform.up * Input.GetAxis("Mouse Y") +
                                     mainCamera.transform.right * Input.GetAxis("Mouse X");
            rb.velocity = throwDirection * throwPower;
        }

        if (throwPower >= gameManager.minPowerToRoll)
            Rolling();
        else
            Invoke("ResetPosition",0.7f);

    }

    public void AutoRollDice()
    {
        if (!isRolling)
        {
            // Reset the dice to its initial position.
            rb.velocity = Vector3.zero;
            transform.position = initialPosition;
            // Apply a random initial velocity.
            Vector3 initialVelocity = new Vector3(Random.Range(-2, 2), Random.Range(2, 7), Random.Range(-2, 1));
            rb.velocity = initialVelocity;

            RotateDice();
            Rolling();
        }
    }

    private void Rolling()
    {
        rollCountdown = 1f;
        gameManager.Rolling();
        isRolling = true;
    }
    private void ResetPosition()
    {
        rb.useGravity = true;
        isHolding = false;
        rb.isKinematic = false;
        rb.velocity = Vector3.zero;
        transform.position = initialPosition;
        // Spawn praticles on reset
        if (spawnDiceParticle != null)
        {
            GameObject VFX = Instantiate(spawnDiceParticle, transform.position, transform.rotation);
            Destroy(VFX, VFXDestroyTimer);
        }
    }

    int CalculateResult()
    { 
        Dice diceScript = GetComponent<Dice>();
        TextMeshPro[] faceNumberList = diceScript.faceNumberList;

        int highestIndex = -1;
        float highestY = float.MinValue;

        for (int i = 0; i < faceNumberList.Length; i++)
        {
            if (faceNumberList[i].transform.position.y > highestY)
            {
                highestY = faceNumberList[i].transform.position.y;
                highestIndex = i;
            }
        }

        if (highestIndex != -1)
        {
            string result = faceNumberList[highestIndex].text;
            return int.Parse(result);
        }
        else
        {
            return 0; // Return a default value or handle it as needed.
        }
    }
    private void RotateDice()
    {
        // Apply a random torque to make it roll.
        Vector3 torque = new Vector3(Random.Range(-500, 500), Random.Range(-500, 500), Random.Range(-500, 500));
        rb.AddTorque(torque * torqueForce, ForceMode.Impulse);
    }
    private Vector3 ClampPosition(Vector3 position)
    {
        // Ensure the dice stays within the bounding box
        Transform boundingBoxMin = gameManager.boundingBoxMin;
        Transform boundingBoxMax = gameManager.boundingBoxMax;

        float x = Mathf.Clamp(position.x, boundingBoxMin.position.x, boundingBoxMax.position.x);
        float y = Mathf.Clamp(position.y, boundingBoxMin.position.y, boundingBoxMax.position.y);
        float z = Mathf.Clamp(position.z, boundingBoxMin.position.z, boundingBoxMax.position.z);

        return new Vector3(x, y, z);
    }
}
