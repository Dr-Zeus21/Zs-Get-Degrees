using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowField : MonoBehaviour
{
    public int SlowAmount;

    public float SlowPercent()
    {
        return (float)SlowAmount / 100;
    }
}
