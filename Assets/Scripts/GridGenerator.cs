using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] Transform BottomLeftPoint;
    [SerializeField] float Height;
    [SerializeField] float GridSize;
    [SerializeField] float xWidth;
    [SerializeField] float zLength;
    [SerializeField] bool GenerateGrid;
    [SerializeField] Dictionary<Vector2, int> gridCosts = new Dictionary<Vector2, int>();
    [SerializeField] bool DisplayGrid;

    private void OnValidate()
    {
        if (GenerateGrid)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            gridCosts = new Dictionary<Vector2, int>();
            GenerateGrid = false;
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            float topOfGrid = BottomLeftPoint.position.y + Height;
            float gridSizeX = xWidth * (1 / GridSize);
            float gridSizeZ = zLength * (1 / GridSize);
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    //adding half the grid size ensures that the ray is cast in the middle of the grid
                    float realX = x * GridSize;
                    float realZ = z * GridSize;
                    float gridMidX = realX + (GridSize / 2);
                    float gridMidZ = realZ + (GridSize / 2);
                    RaycastHit[] hitObjects = Physics.RaycastAll(new Vector3(gridMidX, topOfGrid, gridMidZ), Vector3.down, Height);
                    int obstacleDifficulty = 0;
                    //checks what kind of obstacle is in the grid
                    //if there is a building, the obstqacle is impossible, so difficulty is set to int.maxValue
                    //if there is a slow field, get the slow
                    if (hitObjects.Any(obstacle => obstacle.transform.gameObject.tag == "Building")) obstacleDifficulty = int.MaxValue;
                    else if (hitObjects.Any(obstacle => obstacle.transform.gameObject.tag == "Slow"))
                        obstacleDifficulty = hitObjects.Where(obstacle => obstacle.transform.gameObject.tag == "Slow").First().transform.GetComponent<SlowField>().SlowAmount;

                    //displayCube.GetComponent<MeshRenderer>().materials[0].SetColor("_Color", Color.Lerp(Color.green, Color.red, obstacleDifficulty / 100));

                    gridCosts.Add(new Vector2(realX, realZ), obstacleDifficulty);

                }
            }
            stopWatch.Stop();
            print("done: " + stopWatch.Elapsed);
            print(gridCosts.Count);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a semitransparent blue cube at the transforms position

        if (!DisplayGrid) return;
        foreach (var pos in gridCosts)
        {
            Gizmos.color = Color.Lerp(Color.green, Color.red, (float)pos.Value / 100);
            Gizmos.DrawCube(new Vector3(pos.Key.x, BottomLeftPoint.transform.position.y + (Height / 2), pos.Key.y), new Vector3(GridSize, Height, GridSize));
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hitObjects = Physics.RaycastAll(ray, 9999);
            int obstacleDifficulty = 0;
            //checks what kind of obstacle is in the grid
            //if there is a building, the obstqacle is impossible, so difficulty is set to int.maxValue
            //if there is a slow field, get the slow
            if (hitObjects.Any(obstacle => obstacle.transform.gameObject.tag == "Building")) obstacleDifficulty = int.MaxValue;
            else if (hitObjects.Any(obstacle => obstacle.transform.gameObject.tag == "Slow")) obstacleDifficulty = hitObjects.Where(obstacle => obstacle.transform.gameObject.tag == "Slow").First().transform.GetComponent<SlowField>().SlowAmount;
            foreach (var hit in hitObjects)
            {
                print(hit.transform.gameObject.name + " " + hit.transform.gameObject.tag);
            }
            print(obstacleDifficulty);
        }
    }
}
