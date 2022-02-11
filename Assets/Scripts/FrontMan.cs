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

    private void Start()
    {
        Timer.SimpleTimer(function, 5);
    }

    public void function()
    {
        print("lol");
    }
    void Update()
    {
        OnUpdate?.Invoke();
    }
}


