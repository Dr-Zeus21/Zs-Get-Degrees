using System;
using UnityEngine;

public class FrontMan : MonoBehaviour
{
    public Action OnUpdate;
    public static FrontMan FM;

    float timer = 5;

    private void Awake()
    {
        FM = this;
    }
    void Update()
    {
        OnUpdate?.Invoke();
    }
}


