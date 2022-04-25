using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    [SerializeField] Animator PlayerAnimator;

    public void Walk()
    {
        PlayerAnimator.Play("Walk");
    }

    public void Idle()
    {
        PlayerAnimator.Play("Idle");
    }

    public void Slash()
    {
        PlayerAnimator.Play("Slash");
    }
}
