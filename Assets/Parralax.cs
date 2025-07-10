using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parralax : MonoBehaviour
{
    Camera mainCam;

    public float parallaxMultiplier;

    Vector3 previousPos;

    private void Start()
    {
        mainCam = Camera.main;
        previousPos = mainCam.transform.position;
    }

    private void Update()
    {
        Vector3 deltaPosition = mainCam.transform.position - previousPos;

        transform.position += deltaPosition * parallaxMultiplier;

        previousPos = mainCam.transform.position;
    }
}
