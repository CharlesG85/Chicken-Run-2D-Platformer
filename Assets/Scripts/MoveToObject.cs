using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToObject : MonoBehaviour
{
    private Rigidbody2D m_player;

    private Vector3 previousPosition;
    private Vector2 platformVelocity;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            m_player = collision.GetComponent<Rigidbody2D>();
        }
    }

    private void FixedUpdate()
    {
        platformVelocity = (transform.position - previousPosition) / Time.fixedDeltaTime;
        previousPosition = transform.position;

        if (m_player == null) return;

        m_player.MovePosition(m_player.position + (m_player.velocity + platformVelocity) * Time.fixedDeltaTime);
    }

private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            m_player.velocity += platformVelocity;
            m_player = null;
        }
    }
}
