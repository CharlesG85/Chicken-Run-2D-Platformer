using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name != "Player") return;

        GameManager.Instance.LevelComplete();
        anim.SetTrigger("TriggerGoal");
    }

    public void Reset()
    {
        anim.SetTrigger("Reset");
    }
}
