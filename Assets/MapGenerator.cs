using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] List<GameObject> OneByOneBuildings = new List<GameObject>();
    [SerializeField] List<GameObject> TwoByOneBuildings = new List<GameObject>();
    [SerializeField] List<GameObject> Parks = new List<GameObject>();
    [SerializeField] bool CenteralArea = false;
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] float gap;
    [SerializeField] float buildingSize = 12;
    [SerializeField] int amountOfTwoWides;

    [SerializeField] bool GenerateWorld = false;

    [SerializeField] List<Vector2Int> OpenSpots;
    [SerializeField] List<Vector2Int> Intersections;
    [SerializeField] GameObject OpenSpotCube;
    [SerializeField] bool spawnCubes;
    [SerializeField] GameObject Intersection;

    [SerializeField] bool SpawnZombiers;
    [SerializeField] GameObject Zombie;
    [SerializeField] GameObject van;
    [SerializeField] Transform Player;



    private void Update()
    {
        if (GenerateWorld)
        {
            GenerateWorld = false;

            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            OpenSpots = new List<Vector2Int>();
            Intersections = new List<Vector2Int>();
            float totalSize = buildingSize + gap;
            System.Random rand = new System.Random();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    OpenSpots.Add(new Vector2Int(i, j));
                }
            }
            for (int i = 0; i < width-1; i++)
            {
                for (int j = 0; j < height-1; j++)
                {
                    Intersections.Add(new Vector2Int(i, j));
                }
            }

            if (CenteralArea)
            {
                if(width%2==0 && height % 2 == 0)
                {
                    Instantiate(Parks[2], new Vector3(totalSize * ((width / 2) - .5f), 0, totalSize * ((height / 2)-.5f)), Quaternion.Euler(0, rand.Next(4) * 90, 0), transform);
                    //Instantiate(Parks[2], new Vector3(transform.position.x, 2.4f, transform.position.z), Quaternion.identity, transform);

                    OpenSpots.Remove(new Vector2Int((width / 2), (height / 2)));
                    OpenSpots.Remove(new Vector2Int((width / 2)-1, (height / 2)));
                    OpenSpots.Remove(new Vector2Int((width / 2), (height / 2)-1));
                    OpenSpots.Remove(new Vector2Int((width / 2)-1, (height / 2)-1));

                    Intersections.Remove(new Vector2Int((width / 2) - 1, (height / 2) - 1));
                }
                else if ((width % 2 == 0 && height % 2 == 1))
                {
                    Instantiate(Parks[1], new Vector3(totalSize * ((width / 2) - .5f), 0, totalSize * (((height - 1) / 2))), Quaternion.Euler(0, rand.Next(2) * 180, 0), transform);
                    OpenSpots.Remove(new Vector2Int((width / 2), ((height - 1) / 2)));
                    OpenSpots.Remove(new Vector2Int((width / 2)-1, ((height-1) / 2)));

                }
                else if ((width % 2 == 1 && height % 2 == 0))
                {
                    Instantiate(Parks[1], new Vector3(totalSize * (((width-1) / 2)), 0, totalSize * ((height / 2) - .5f)), Quaternion.Euler(0, (rand.Next(2) * 180)+90, 0), transform);
                    OpenSpots.Remove(new Vector2Int(((width-1) / 2), (height / 2)));
                    OpenSpots.Remove(new Vector2Int(((width - 1) / 2), (height / 2)-1));

                }
                else if ((width % 2 == 1 && height % 2 == 1))
                {
                    Instantiate(Parks[0], new Vector3(totalSize * (((width - 1) / 2)), 0, totalSize * (((height - 1) / 2))), Quaternion.Euler(0, rand.Next(4) * 90, 0), transform);
                    OpenSpots.Remove(new Vector2Int(((width - 1) / 2), ((height-1) / 2)));
                }
            }
            if (spawnCubes)
            {
                foreach (var space in OpenSpots)
                {
                    Instantiate(OpenSpotCube, new Vector3(space.x* totalSize, 0, space.y* totalSize), Quaternion.Euler(0, rand.Next(4) * 90, 0), transform);
                }
            }

            int largeBuildingsToSpawn = amountOfTwoWides;
            
            while (largeBuildingsToSpawn > 0)
            {
                //0= freeAbove = false;
                //1= freeRight = false;
                //2= freeLeft = false;
                //3= freeDown = false;
                bool[] FreeRotations = new bool[4];
                Vector2Int newSpot = OpenSpots[rand.Next(OpenSpots.Count)];
                FreeRotations[0] = OpenSpots.Contains(new Vector2Int(newSpot.x, newSpot.y + 1));
                FreeRotations[1] = OpenSpots.Contains(new Vector2Int(newSpot.x+1, newSpot.y));
                FreeRotations[2] = OpenSpots.Contains(new Vector2Int(newSpot.x-1, newSpot.y));
                FreeRotations[3] = OpenSpots.Contains(new Vector2Int(newSpot.x, newSpot.y - 1));
                int rot;
                if (FreeRotations.Contains(true))
                {
                    int index;
                    do
                    {
                        index = rand.Next(FreeRotations.Length);
                    } while (!FreeRotations[index]);
                    GameObject Building;
                    switch (index)
                    {
                        case 0:
                            Building= Instantiate(TwoByOneBuildings[rand.Next(TwoByOneBuildings.Count)], new Vector3((newSpot.x)*totalSize, 0, (newSpot.y+.5f)*totalSize), Quaternion.Euler(0,90,0), transform);
                            OpenSpots.Remove(newSpot);
                            OpenSpots.Remove(new Vector2Int(newSpot.x, newSpot.y+1));
                            print(Building.name);
                            break;
                        case 1:
                            Building = Instantiate(TwoByOneBuildings[rand.Next(TwoByOneBuildings.Count)], new Vector3((newSpot.x + .5f) * totalSize, 0, (newSpot.y) * totalSize), Quaternion.identity, transform);
                            OpenSpots.Remove(newSpot);
                            OpenSpots.Remove(new Vector2Int(newSpot.x + 1, newSpot.y));
                            print(Building.name);
                            break;
                        case 2:
                            Building = Instantiate(TwoByOneBuildings[rand.Next(TwoByOneBuildings.Count)], new Vector3((newSpot.x - .5f) * totalSize, 0, (newSpot.y) * totalSize), Quaternion.identity, transform);
                            OpenSpots.Remove(newSpot);
                            OpenSpots.Remove(new Vector2Int(newSpot.x - 1, newSpot.y));
                            print(Building.name);
                            break;
                        case 3:
                            Building = Instantiate(TwoByOneBuildings[rand.Next(TwoByOneBuildings.Count)], new Vector3((newSpot.x) * totalSize, 0, (newSpot.y - .5f) * totalSize), Quaternion.Euler(0, 90, 0), transform);
                            OpenSpots.Remove(newSpot);
                            OpenSpots.Remove(new Vector2Int(newSpot.x, newSpot.y - 1));
                            print(Building.name);
                            break;
                    }
                    largeBuildingsToSpawn--;
                    
                    print(newSpot);
                    print(index);
                }
            }
            foreach (var space in OpenSpots)
            {
                Instantiate(OneByOneBuildings[rand.Next(OneByOneBuildings.Count)], new Vector3((space.x) * totalSize, 0, (space.y) * totalSize), Quaternion.Euler(0, rand.Next(4)*90, 0), transform);
            }
            int startSpot = rand.Next(height-1);
            Vector2Int startVex = new Vector2Int(0, startSpot);
            GameObject spawnedVan = Instantiate(van, new Vector3(((startVex.x * totalSize) + (totalSize / 2))-(totalSize*1.5f), 0, (startVex.y * totalSize) + (totalSize / 2)), Quaternion.identity, transform);
            Player.position = spawnedVan.transform.position + new Vector3(4, 0, 0);
            foreach (var space in Intersections)
            {
                Instantiate(Intersection, new Vector3((space.x * totalSize)+(totalSize/2), 0, (space.y * totalSize) + (totalSize / 2)), Quaternion.identity, transform);
                if (SpawnZombiers && startVex!= space)
                {
                    for (int i = 0; i < rand.Next(4); i++)
                    {
                        Instantiate(Zombie, new Vector3(((space.x * totalSize) + (totalSize / 2))+UnityEngine.Random.insideUnitCircle.x, 2, ((space.y * totalSize) + (totalSize / 2))+ UnityEngine.Random.insideUnitCircle.y), Quaternion.Euler(0,rand.Next(360),0), transform);
                    }
                }
            }
        }
    }
}
