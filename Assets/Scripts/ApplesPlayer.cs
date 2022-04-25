using UnityEngine;

[DisallowMultipleComponent]
public class ApplesPlayer : MonoBehaviour
{
    // Public Variables
    public float MovementSpeed = 15;
    [ReadOnly] public bool turnable = false;
    [ReadOnly] public Vector3 MovementAxis = new Vector3(1, 0, 0);  //Player starts on the x axis.  if this changes, change this vector3

    //the location of the current intersection
    Vector3 _currentIntersectionLoc;
    Rigidbody _rb;

    //material of the player
    Material _mat;

    //if the player cant move, this is true
    public bool canMove = true;


    [SerializeField] PlayerAnimationHandler PAH;
    [SerializeField] float movingAngle = 20;



    private void Awake()
    {
        //grabbing the players rigidbody component
        _rb = GetComponent<Rigidbody>();

        //grabbing the material of the player
        //_mat = GetComponent<Renderer>().material;


    }


    private void FixedUpdate()
    {
        //leftward movement
        //If the player is moving a horizontal movement key (like A or D or left/right arrows, or a controller stick), then Input.GetAxis("Horizontal") will go from 0 to 1, (or a number in between for a controller stick), thus making the entire calculation non-zero
        //then it takes the axis its on (the x axis, z axis, -x axis, etc.) then multiplies it by the players movement speed
        //them multiplies by Time.deltaTime
        //then it adds the horizontal velocity, as otherwise horizontal velocity would stop when player movement stops.
        _rb.velocity = ((Input.GetAxis("Horizontal") * MovementAxis * MovementSpeed * Time.deltaTime) + new Vector3(0, _rb.velocity.y, 0)) * canMove.Bool2Int();
    }

    private void Update()
    {
        if (!canMove) return;
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            transform.eulerAngles = new Vector3(0, (Mathf.Atan2(MovementAxis.z, -MovementAxis.x) * Mathf.Rad2Deg) + 90 + (Input.GetAxisRaw("Horizontal") == -1 ? 0 - movingAngle : 180 + movingAngle), 0);
            PAH.Walk();
        }
        else PAH.Idle();
    }

    public void ChangeAxis(Vector3 newAxis)
    {
        //if at an intersection, move to the center of the intersection
        if (turnable) transform.position = new Vector3(_currentIntersectionLoc.x, transform.position.y, _currentIntersectionLoc.z);
        MovementAxis = newAxis;
        transform.eulerAngles = new Vector3(0, (Mathf.Atan2(MovementAxis.z, -MovementAxis.x) * Mathf.Rad2Deg)+90, 0);
    }

    //Checks if the player is at an intersection
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Intersection")
        {
            turnable = true;
            _currentIntersectionLoc = other.transform.position;
        }
    }

    // exiting an intersection blocks the player from turning
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Intersection") turnable = false;
    }
}
