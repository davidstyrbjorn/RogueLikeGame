using UnityEngine;
using System.Collections;

public class PlayerAnimation : MonoBehaviour {

    private Animator animator;

    void Start()
    {
        // Get component calls
        animator = GetComponentInChildren<Animator>();
    }

    public void DoCombatAnimation()
    {
        animator.SetTrigger("CombatHit");
    }

}
