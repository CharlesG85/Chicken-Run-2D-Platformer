using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageScroller : MonoBehaviour
{
    [SerializeField] private RawImage _img;
    [SerializeField] private float scrollMultiplier = 1;

    private Camera mainCam;
    private RectTransform canvasRectTransform;

    private float previousX;

    private void Start()
    {
        mainCam = Camera.main;
        canvasRectTransform = GetComponent<RectTransform>();
        previousX = mainCam.transform.position.x;
    }

    void Update()
    {
        float deltaX = mainCam.transform.position.x - previousX;
        float offset = deltaX * (_img.uvRect.width / canvasRectTransform.rect.width) * scrollMultiplier;
        _img.uvRect = new Rect(_img.uvRect.position + new Vector2(offset, 0f), _img.uvRect.size);

        previousX = mainCam.transform.position.x;
    }
}
