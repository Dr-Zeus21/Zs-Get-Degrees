using DG.Tweening;
using System;
using UnityEngine;

public class Syringe : MonoBehaviour
{
    [SerializeField] Transform pusher;
    Vector3 ogPusherPos;
    [SerializeField] Transform pusherEndPoint;
    [SerializeField] GameObject interiorLiquid;
    public float FillAmount = 1;
    Material _mat;
    public float injectionTime = 2;

    public bool startInjection = false;
    public bool reset = false;
    private void Awake()
    {
        _mat = interiorLiquid.GetComponent<Renderer>().material;
        ogPusherPos = pusher.position;
    }
    public void StartInjection(float time)
    {
        StartInjection(time, () => { });
    }

    public void StartInjection(float time, Action onComplete)
    {
        pusher.DOLocalMove(pusherEndPoint.localPosition, time).SetEase(Ease.Linear);
        DOTween.To(() => FillAmount, (x) =>
        {
            _mat.SetFloat("Fill_amount", x);
            FillAmount = x;
        }
            , 0, time).SetEase(Ease.Linear).OnComplete(() => onComplete.Invoke());
    }

    public void ResetInjection()
    {
        FillAmount = 1;
        _mat.SetFloat("Fill_amount", 1);
        pusher.position = ogPusherPos;
    }

    private void Update()
    {
        if (startInjection)
        {
            startInjection = false;
            StartInjection(injectionTime);
        }
        if (reset)
        {
            reset = false;
            ResetInjection();
        }
    }
}
