using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Serialized Fields
    [SerializeField] static float cameraVerticalOffset = 2f;
    [SerializeField] static float cameraHorizontalOffset = 6f;

    // Public Variables
    public Player player;
    public float targetAngle; // stores angle camera is moving toward
    public int looper = 0; // stores how many turns take place (1 or 2)
    public float multiplier = 1; // used to double speed for 180 degree rotation
    public Vector3[] offsets = new Vector3[4];
    public float[] angles = new float[4];

    // Private Variables

    // Offsets hold camera offset from player
    private Vector3 offset0 = new Vector3(0, cameraVerticalOffset, -cameraHorizontalOffset);
    private Vector3 offset1 = new Vector3(-cameraHorizontalOffset, cameraVerticalOffset, 0);
    private Vector3 offset2 = new Vector3(0, cameraVerticalOffset, cameraHorizontalOffset);
    private Vector3 offset3 = new Vector3(cameraHorizontalOffset, cameraVerticalOffset, 0);

    // Angles store camera angles in relative offset
    private float angle0 = 0;
    private float angle1 = 90;
    private float angle2 = 180;
    private float angle3 = -90;
    private int angleTracker, offsetTracker = 0;

    // currentPosition used to store desired camera position during a transition
    // endPosition used to store desired camera position after a transition
    private Vector3 playerPosition, currentPosition, endPosition;
    private float panSpeed = 40f; // speed of camera pan

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();

        // Put offsets and angles into arrays
        offsets[0] = offset0;
        offsets[1] = offset1;
        offsets[2] = offset2;
        offsets[3] = offset3;
        angles[0] = angle0;
        angles[1] = angle1;
        angles[2] = angle2;
        angles[3] = angle3;
    }

    // Update is called once per frame
    void Update()
    {
        // Use TurnCamera() for 90 and 180 turn
        if (player.turning)
        {
            TurnCamera();
            StopAllCoroutines();
            StartCoroutine(CameraCoroutine(targetAngle));
        }
        // Use ReverseCamera() for -90 turn
        if (player.reversing)
        {
            ReverseCamera();
            StopAllCoroutines();
            StartCoroutine(CameraCoroutine(targetAngle));
        }

        // Camera tracking relative to motion axis
        if ((!player.turning && !player.reversing) && player.turned == 0 || player.turned == 2)
        {
            currentPosition = transform.position;
            currentPosition.x = player.transform.position.x;
            transform.position = currentPosition;
        }
        if ((!player.turning && !player.reversing) && player.turned == 1 || player.turned == 3)
        {
            currentPosition = transform.position;
            currentPosition.z = player.transform.position.z;
            transform.position = currentPosition;
        }
    }

    // Function to turn the camera CW either 90 or 180 degrees
    public void TurnCamera()
    {
        // increments tracker to turn CW
        if (looper != 0)
        {
            if (offsetTracker < 3)
                offsetTracker++;
            else
                offsetTracker = 0;

            if (angleTracker < 3)
                angleTracker++;
            else
                angleTracker = 0;
            looper--;
        }

        // sets end position and target angle appropriately
        endPosition = player.transform.position + offsets[offsetTracker];
        targetAngle = angles[angleTracker];

        // camera movement speed * relative multiplier
        var step = panSpeed * multiplier * Time.deltaTime;

        // keep moving to final destination, once there reset player.turning
        if (transform.position != endPosition)
            transform.position =
                Vector3.MoveTowards(transform.position, endPosition, step);
        else
        {
            player.turning = false;
            multiplier = 1;
        }
    }

    // Function to turn camera CCW 90 degrees
    public void ReverseCamera()
    {
        // decrements trackers to turn CCW
        if (looper != 0)
        {
            if (offsetTracker > 0)
                offsetTracker--;
            else
                offsetTracker = 3;

            if (angleTracker > 0)
                angleTracker--;
            else
                angleTracker = 3;
            looper--;
        }

        // sets end position and target angle appropriately
        endPosition = player.transform.position + offsets[offsetTracker];
        targetAngle = angles[angleTracker];

        // camera movement speed
        var step = panSpeed * Time.deltaTime;

        // keep moving to final destination, once there reset player.reversing
        if (transform.position != endPosition)
            transform.position =
                Vector3.MoveTowards(transform.position, endPosition, step);
        else
            player.reversing = false;
    }

    // Coroutine to make a smooth rotation
    IEnumerator CameraCoroutine(float targetAngle)
    {
        // Updates camera's y rotation each update using Slerp, which rotates it about a sphere
        while (transform.rotation.y != targetAngle)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, targetAngle, 0f), 15f * Time.deltaTime);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        yield return null;
    }
}
