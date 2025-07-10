using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // The target object to follow
    public float smoothSpeed = 0.125f; // The speed at which the camera follows the target
    public float lookaheadDistance = 2f;
    public float minX; // Minimum x-axis value
    public float maxX; // Maximum x-axis value

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        Camera cam = Camera.main;
        float camWidth = cam.orthographicSize * cam.aspect;

        // Calculate the desired camera position
        float targetX = Mathf.Clamp(target.position.x + lookaheadDistance, minX + camWidth, maxX - camWidth);
        Vector3 desiredPosition = new Vector3(targetX, transform.position.y, transform.position.z);

        // Smoothly move the camera towards the desired position
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
    }
}
