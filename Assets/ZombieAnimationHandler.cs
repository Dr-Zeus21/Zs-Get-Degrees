using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAnimationHandler : MonoBehaviour
{
    [SerializeField] Animator ZombieAnimator;

    public void Walk()
    {
        ZombieAnimator.Play("Walk");
    }

    public void Idle()
    {
        ZombieAnimator.Play("Idle");
    }

    public void Stun()
    {
        ZombieAnimator.Play("Stun");
    }
}
