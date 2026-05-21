using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueAnimate : MonoBehaviour
{
    [SerializeField] private float topY = 12f;
    [SerializeField] private float bottomY = 2f;
    [SerializeField] private float cycleDuration = 0.8f;

    private RectTransform rect;
    private float timer;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        timer = 0f;
        SetY(topY);
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = Mathf.PingPong(timer / (cycleDuration * 0.5f), 1f);
        float eased = Mathf.SmoothStep(0f, 1f, t);
        float y = Mathf.Lerp(topY, bottomY, eased);
        SetY(y);
    }

    void SetY(float y)
    {
        Vector2 p = rect.anchoredPosition;
        p.y = y;
        rect.anchoredPosition = p;
    }
}
