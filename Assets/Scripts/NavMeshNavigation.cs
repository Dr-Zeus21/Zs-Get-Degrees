using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshNavigation : MonoBehaviour
{
    NavMeshAgent navMA;
    public Transform destination;
    [SerializeField] List<SlowField> slows;
    public bool canMove = true;
    public float baseSpeed;

    private void Awake()
    {
        navMA = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if(destination) navMA.destination = destination.position;
        if (!canMove)
        {
            navMA.speed = 0;
            return;
        }
        if (slows.Count > 0) navMA.speed = (baseSpeed * slows.OrderBy(slow => slow.SlowPercent()).First().SlowPercent());
        
        else navMA.speed = baseSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        SlowField slowF = other.GetComponent<SlowField>();
        if (slowF) slows.Add(slowF);
    }

    private void OnTriggerExit(Collider other)
    {
        SlowField slowF = other.GetComponent<SlowField>();
        if (slowF) slows.Remove(slowF);
    }


}
