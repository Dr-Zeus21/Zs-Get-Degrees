using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ApplesCameraController : MonoBehaviour
{
    [SerializeField, Range(0f, 30.0f)] float CameraDistance = 6;
    [SerializeField, Range(0f, 10.0f)] float CameraHeight = 1.5f;
    [SerializeField] float CameraRotationTime = .3f; //Time for the camera to rotat in seconds
    [SerializeField] float CameraFlipTime = .3f; //Time for the camera to rotat in seconds
    [SerializeField] ApplesPlayer PlayerController;
    [SerializeField, Range(0f, 10.0f)] float DisappearingRange = 3f;
    [SerializeField, ReadOnly] bool moving = false;

    [SerializeField, ReadOnly] private List<GameObject> _adjacentBuildings = new List<GameObject>();

    public bool PrintDistance = false;
    private void Update()
    {
        //CW rotation
        if (Input.GetKey(KeyCode.Q))  Rotate(true);

        //CCW rotation
        if (Input.GetKey(KeyCode.E)) Rotate(false);

        //moving over the player
        if (Input.GetKey(KeyCode.R)) JumpOver();


        //takes each building and sets their transparency to be a percent of how close they are to the camera
        foreach (var building in _adjacentBuildings)
        {
            //this float represents the distance from the camera to the edge of the buildings collider
            float fromCamToEdge = Vector3.Distance(transform.position, building.GetComponent<Collider>().ClosestPoint(transform.position));

            //you can print the distance to each adjacent building via the inspector
            if (PrintDistance)
            {
                print(fromCamToEdge + " " + building.name);
                PrintDistance = false;
            }

            //if the building is within disappearing range (as defined in the inspector) then set their transparency to be a percent of how close they are to the camera
            //for example, if the buildings edge is 1 unit away, and the disappearing range is set to 3, the transparency will be 1/3, or .33, which results in the object being 66% see-through
            if (fromCamToEdge < DisappearingRange){
                Material buildingMaterial = building.GetComponent<MeshRenderer>().materials[0];
                Color buildingColor = buildingMaterial.color;
                building.GetComponent<MeshRenderer>().materials[0].SetColor("_Color", new Color(buildingColor.r, buildingColor.g, buildingColor.b, fromCamToEdge / DisappearingRange));
            }
        }
    }


    //The rotate function.
    //The bool input decides weather the rotation is Clockwise or CounterClockwise.
    //True = CW, False = CCW
    private void Rotate(bool CWorCCW)
    {
        //prevents rotation if player is not at an intersection
        if (!PlayerController.turnable) return;

        //prevents a rotation if the camera is already moving
        if (moving) return;
        moving = true;

        //Gets the rotational position to the front left or front right (depending on CWorCCW)
        Vector3 newPos = (transform.forward * CameraDistance) + ((CWorCCW ? -transform.right : transform.right) * CameraDistance);

        //floors the rotation, as DORotate will leave it at .00000001 degrees sometimes.
        float CameraRotation = Mathf.Floor(transform.eulerAngles.y);

        //performs a smooth movement from its current x,z to its new x,z defined by newPos.
        //the ease attribute controls the speed of the movement.  With x or z being InQuad and the other being OutQuad creates the movement of a circular arc
        //the ease swaps between x and z depending on the rotation of the camera
        transform.DOLocalMoveX(newPos.x + transform.localPosition.x, CameraRotationTime).SetEase((CameraRotation % 180 == 0) ? Ease.OutQuad : Ease.InQuad);
        transform.DOLocalMoveZ(newPos.z + transform.localPosition.z, CameraRotationTime).SetEase((CameraRotation % 180 == 0) ? Ease.InQuad : Ease.OutQuad);

        //This rotates the camera iether 90 degrees or -90 degrees, depending on CWorCCW
        //once the rotation is complete, the moving bool is turned back to false, allowing for another movement to be made
        transform.DORotate(new Vector3(0, transform.eulerAngles.y + (CWorCCW ? 90 : -90), 0), CameraRotationTime).SetEase(Ease.Linear).OnComplete(() => moving = false);

        //Rotates the players movement axis to align with the new rotation
        PlayerController.ChangeAxis(CWorCCW ? -transform.forward : transform.forward);
    }

    private void JumpOver()
    {
        //prevents a flip if the camera is already moving
        if (moving) return;
        moving = true;

        //The original position, used as a base for movement
        Vector3 oldPos = transform.localPosition;

        //the aimed-for middle position above the player
        float midHeight = (CameraHeight*3f);
        Vector3 midPos = transform.forward * CameraDistance;

        //The aimed-for new height, position, and rotation of the camera
        float newHeight = (transform.up * CameraHeight).y;
        Vector3 newPos = transform.forward * (CameraDistance * 2);
        float newRot = transform.eulerAngles.y+180;

        //moves the camera to the player
        transform.DOLocalMoveX((midPos + transform.localPosition).x, CameraFlipTime).SetEase(Ease.InQuad);
        transform.DOLocalMoveZ((midPos + transform.localPosition).z, CameraFlipTime).SetEase(Ease.InQuad);
        //rotates the camera down
        transform.DORotate(new Vector3(90, newRot - 180, 0), CameraFlipTime).SetEase(Ease.InQuad);

        //moves the camera above the player.  Once this movement is complete, it initiats the next set of movements towards the opposite side of the player
        transform.DOMoveY(midHeight + oldPos.y, CameraFlipTime).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            
            //This block of code occurs once the camera is over the player

            transform.DOLocalMoveY(oldPos.y, CameraFlipTime).SetEase(Ease.InQuad);
            transform.DOLocalMove(newPos+ oldPos, CameraFlipTime).SetEase(Ease.OutQuad).OnComplete(() => moving = false);

            //an instant 180 rotation
            transform.DORotate(new Vector3(90, newRot, 0), 0);

            //rotates the camera back towards the player
            transform.DORotate(new Vector3(0, newRot, 0), CameraFlipTime).SetEase(Ease.OutQuad);
            
        });

        //Rotates the players movement axis to align with the new rotation
        PlayerController.ChangeAxis(PlayerController.MovementAxis *= -1);
    }

    private void OnValidate()
    {
        if (moving) return;
        //Allows the sliders to affect camera pos in real time
        transform.localPosition = new Vector3(-(transform.forward * CameraDistance).x, CameraHeight, -(transform.forward * CameraDistance).z);
    }


    private void OnTriggerEnter(Collider other)
    {
        //when a building becomes adjacent, add them to the list
        if (other.gameObject.tag == "Building") _adjacentBuildings.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Building")
        {
            //when a building is no longer adjacent, remove them from the list and set their transparency to 1
            _adjacentBuildings.Remove(other.gameObject);
            Color buildingColor = other.gameObject.GetComponent<MeshRenderer>().materials[0].color;
            other.gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_Color", new Color(buildingColor.r, buildingColor.g, buildingColor.b, 1));
        }
            
            
    }

    /*  THIS BLOCK MAKES THE BUILDINGS DISAPPEAR COMPLETELY
    //These functions turn off a buildings ability to be seen, and cause them to only create shadows
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Building") other.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Building") other.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }*/
}
