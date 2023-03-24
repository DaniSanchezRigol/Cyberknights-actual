using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    Action onAttackAnimHits;
    Action onAttackAnimHits2;
    Action onAttackAnimComplete;
    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void PlayAnim2hits(string name, Action onAttackAnimHits, Action onAttackAnimHits2, Action onAttackAnimComplete)
    {
        animator.Play(name);
        this.onAttackAnimHits = onAttackAnimHits;
        this.onAttackAnimHits2 = onAttackAnimHits2;
        this.onAttackAnimComplete = onAttackAnimComplete;
    }
    public void PlayAnim(string name, Action onAttackAnimHits, Action onAttackAnimComplete)
    {
        animator.Play(name);
        this.onAttackAnimHits = onAttackAnimHits;
        this.onAttackAnimComplete = onAttackAnimComplete;
    }
    public void PlayAnimAttack(Action onAttackAnimHits, Action onAttackAnimComplete)
    {
        animator.Play("Attack");
        this.onAttackAnimHits = onAttackAnimHits;
        this.onAttackAnimComplete = onAttackAnimComplete;
    }
    public void PlayAnimBow(Action onAttackAnimHits, Action onAttackAnimComplete)
    {
        animator.Play("BowAttack");
        this.onAttackAnimHits = onAttackAnimHits;
        this.onAttackAnimComplete = onAttackAnimComplete;

    }
    public void PlayAnimBlocking()
    {
        animator.SetTrigger("Block");
    }
    public void AnimationEnds()
    {
        onAttackAnimComplete();
    }
    public void AnimationHits()
    {
        onAttackAnimHits();
    }
    public void AnimationHits2()
    {
        onAttackAnimHits2();
    }
    private IEnumerator AttackTimer()
    {
        yield return new WaitForSeconds(0.7f);
        onAttackAnimComplete();
    }
    public void PlayAnimIdle()
    {
        animator.SetTrigger("Idle");
    }

    public void PlayAnimDamaged()
    {
        animator.Play("Damaged");
    } 
    public void PlayAnimRun()
    {
        animator.Play("Run");
    }
    public void PlayAnimRollAway()
    {
        animator.Play("RollAway");
    }
    public void PlayAnimDeath() 
    {         
        animator.Play("Death");
    }
}
