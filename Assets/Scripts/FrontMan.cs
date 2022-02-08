using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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


