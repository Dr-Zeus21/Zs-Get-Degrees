using System;
using UnityEngine;

public class FrontMan : MonoBehaviour
{
    public Action OnUpdate;
    public static FrontMan FM;

    private void Awake()
    {
        FM = this;
    }
    void Update()
    {
        OnUpdate?.Invoke();
    }
}


