using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    [SerializeField] private float bounceForce = 40f;
    [SerializeField] private float horizontalBounceForce = 1f;
    [SerializeField] private string defaultAnimation;
    [SerializeField] private string onCollideAnimation;

    private Animator m_anim;

    private void Start()
    {
        m_anim = GetComponent<Animator>();
        GameManager.Instance.onPlayerReset += Reset;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            collision.collider.GetComponent<PlayerController>().Bounce(bounceForce, horizontalBounceForce);
            m_anim.Play(onCollideAnimation);
        }
    }

    private void Reset()
    {
        m_anim.Play(defaultAnimation);
    }

    private void OnDisable()
    {
        GameManager.Instance.onPlayerReset -= Reset;
    }
}
