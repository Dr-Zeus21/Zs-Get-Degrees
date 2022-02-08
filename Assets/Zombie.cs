using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Zombie : MonoBehaviour
{
    public int health = 5;

    public int conversionThreshhold = 5;
    //turns true if the zombie is able to be converted or not
    public bool convertible = false;

    Material _mat;
    public float injectionTime = 2;
    [SerializeField] float ConversionPercent = 1;

    public bool startInjection = false;
    public bool reset = false;
    public void StartConversion(float time)
    {
        DOTween.To(() => ConversionPercent, (x) =>
        {
            _mat.SetFloat("ConversionRate", x);
            ConversionPercent = x;
        }
            , 0, time);
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
        if (health <= conversionThreshhold) convertible = true;
        return false;
    }
}
