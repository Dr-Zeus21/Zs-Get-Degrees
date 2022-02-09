using UnityEngine;

public class SlowField : MonoBehaviour
{
    public int SlowAmount;

    public float SlowPercent()
    {
        return (float)SlowAmount / 100;
    }
}
