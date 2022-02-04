using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class NavMeshNavigation : MonoBehaviour
{
    private NavMeshAgent navMA;
    [SerializeField] Transform destination;
    [SerializeField] List<SlowField> slows;
    [SerializeField] float baseSpeed;

    private void Awake()
    {
        navMA = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        navMA.destination = destination.position;
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
