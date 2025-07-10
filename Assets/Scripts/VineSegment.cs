using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineSegment : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();

        if (player == null || collision.GetComponent<HingeJoint2D>().enabled == true)
        {
            return;
        }

        if (player.canGrabVine)
        {
            Vector3 pos = transform.position;
            pos.z = collision.transform.position.z;
            collision.transform.position = pos;

            player.GetComponent<HingeJoint2D>().enabled = true;
            player.GetComponent<HingeJoint2D>().connectedBody = GetComponent<Rigidbody2D>();

            player.canGrabVine = false;
        }
    }
}
