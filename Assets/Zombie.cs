using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class Zombie : MonoBehaviour
{
    public int health = 5;

    public int conversionThreshhold = 5;
    //turns true if the zombie is able to be converted or not
    public bool convertible = false;

    Material _mat;
    NavMeshNavigation _navMnav;
    public float injectionTime = 2;
    [SerializeField, ReadOnly] float ConversionPercent = 0;

    public bool startInjection = false;
    public bool reset = false;

    [SerializeField] GameObject Syringe;
    [SerializeField] Transform syringStartPos;
    [SerializeField] Transform syringEndPos;

    [SerializeField] Transform van;

    private void Awake()
    {
        _mat = GetComponent<Renderer>().material;
        _navMnav = GetComponent<NavMeshNavigation>();
    }
    public void StartConversion(float time)
    {
        _navMnav.canMove = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        DOTween.To(() => ConversionPercent, (x) =>
        {
            _mat.SetFloat("ConversionRate", x);
            ConversionPercent = x;
        }
            , 1, time).OnComplete(()=>
            {
                _navMnav.canMove = true;
                Syringe.SetActive(false);
                _mat.SetInt("Convertible", 0);
            });
        Syringe.SetActive(true);
        Syringe.transform.DOMove(syringEndPos.position, time * .1f).SetEase(Ease.InQuad).
            OnComplete(() => Syringe.GetComponent<Syringe>().
            StartInjection(time * .8f, () =>
            {
                gameObject.tag = "Converted";
                Syringe.transform.DOMove(syringStartPos.position, time * .1f);
                _navMnav.destination = van;
                _navMnav.baseSpeed = 2.5f;
            }));
    }

    public void ResetConversion()
    {
        ConversionPercent = 1;
        _mat.SetFloat("ConversionRate", 1);
    }

    private void Update()
    {
        if (startInjection)
        {
            startInjection = false;
            StartConversion(injectionTime);
        }
        if (reset)
        {
            reset = false;
            ResetConversion();
        }
    }
    public bool AttemptConvert()
    {
        if (convertible)
        {
            StartConversion(injectionTime);
            return true;
        }
        return false;
    }

    //use this function to damage zombies
    //returns true is this attack killed the zombie
    public bool Damage(int amount)
    {
        health--;
        if(health <= 0)
        {
            Destroy(gameObject);
            return true;
        }
        if (health <= conversionThreshhold)
        {
            convertible = true;
            _mat.SetInt("Convertible", 1);
        }
        return false;
    }
}
