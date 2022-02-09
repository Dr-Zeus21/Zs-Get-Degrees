using System.Collections.Generic;
using UnityEngine;

public class CameraAdjacencyDisappearing : MonoBehaviour
{
    //how far away a building must be before it starts to disappear
    [SerializeField, Range(0f, 10.0f)] float DisappearingRange = 3f;
    //list of adjacent buildings
    [SerializeField, ReadOnly] private List<GameObject> _adjacentBuildings = new List<GameObject>();

    //toggle this to print how far away adjacent buildings are
    public bool PrintDistance = false;

    void Update()
    {
        //takes each building and sets their transparency to be a percent of how close they are to the camera
        foreach (var building in _adjacentBuildings)
        {
            //this float represents the distance from the camera to the edge of the buildings collider
            float fromCamToEdge = Vector3.Distance(transform.position, building.GetComponent<Collider>().ClosestPoint(transform.position));

            //you can print the distance to each adjacent building via the inspector
            if (PrintDistance)
            {
                print(fromCamToEdge + " " + building.name);
                print(building.GetComponent<MeshRenderer>().material.GetColor("_Color"));
                PrintDistance = false;
            }

            //if the building is within disappearing range (as defined in the inspector) then set their transparency to be a percent of how close they are to the camera
            //for example, if the buildings edge is 1 unit away, and the disappearing range is set to 3, the transparency will be 1/3, or .33, which results in the object being 66% see-through
            if (fromCamToEdge < DisappearingRange)
            {
                Material buildingMaterial = building.GetComponent<MeshRenderer>().material;
                Color buildingColor = buildingMaterial.color;
                building.GetComponent<MeshRenderer>().material.color = new Color(buildingColor.r, buildingColor.g, buildingColor.b, fromCamToEdge / DisappearingRange);
            }
        }
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
            Color buildingColor = other.gameObject.GetComponent<MeshRenderer>().material.color;
            other.gameObject.GetComponent<MeshRenderer>().material.color = new Color(buildingColor.r, buildingColor.g, buildingColor.b, 1);
        }


    }
}
