using UnityEngine;

public class Player : MonoBehaviour
{

    // Serialized Fields
    [SerializeField] float moveSpeed = 5f;

    // Private Variables
    private Vector3 move;
    private Rigidbody rigidBody;
    private CameraController cameraController;

    // Public Variables
    public bool turnable = false;
    public bool turning = false;
    public bool reversing = false;
    public int turned = 0;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = this.GetComponent<Rigidbody>();
        cameraController = FindObjectOfType<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        // Registers for left and right movement
        var moveRight = 0;
        var moveLeft = 0;

        // Inputs for left and right movement
        if (Input.GetKey(KeyCode.D))
            moveRight = 1;
        if (Input.GetKey(KeyCode.A))
            moveLeft = 1;

        // Inputs to turn the camera (90 or -90) or flip the camera (180)
        if (turnable && !turning && !reversing && Input.GetKeyDown(KeyCode.Alpha1))
        {
            cameraController.looper = 1;
            if (turned < 3)
                turned++;
            else
                turned = 0;
            turning = true;
        }
        if (turnable && !reversing && !turning && Input.GetKeyDown(KeyCode.Alpha2))
        {
            cameraController.looper = 1;
            if (turned > 0)
                turned--;
            else
                turned = 3;
            reversing = true;
        }
        if (!turning && !reversing && Input.GetKeyDown(KeyCode.Alpha3))
        {
            cameraController.looper = 2;
            cameraController.multiplier = 3f;
            if (turned < 3)
                turned++;
            else
                turned = 0;
            if (turned < 3)
                turned++;
            else
                turned = 0;
            turning = true;
        }

        // Calculate and store change in X velocity
        var deltaX = (moveRight - moveLeft) * moveSpeed;
        if (turned == 1 || turned == 2)
            deltaX = -deltaX;

        // Checks if player is on X or Z axis, applies change to move appropriately
        if (turned == 0 || turned == 2)
            move = new Vector3(deltaX, 0, 0);
        if (turned == 1 || turned == 3)
            move = new Vector3(0, 0, deltaX);

        // Change player's rigidBody velocity to move
        rigidBody.velocity = move;
    }

}
