using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private bool pingPong;
    [SerializeField] private List<Transform> waypoints;

    private int target;
    private bool isMovingInReverse;
    private Vector2 startPos;

    private void Start()
    {
        startPos = transform.position;
        GameManager.Instance.onPlayerReset += Reset;
    }

    private void Reset()
    {
        transform.position = startPos;
        target = 0;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, waypoints[target].position, moveSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (pingPong == true && transform.position == waypoints[target].position)
        {
            if (target == waypoints.Count - 1)
            {
                isMovingInReverse = true;
            }
            else if (target == 0)
            {
                isMovingInReverse = false;
            }

            if (isMovingInReverse)
            {
                target -= 1;
            }
            else
            {
                target += 1;
            }

            return;
        }

        if (transform.position == waypoints[target].position)
        {
            if (target == waypoints.Count - 1)
            {
                target = 0;
            }
            else
            {
                target += 1;
            }
        }
    }


}
