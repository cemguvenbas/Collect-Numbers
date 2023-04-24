using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class CircleInteractor : MonoBehaviour
{
    private Circle circle;

    private void Start()
    {
        circle = GetComponent<Circle>();
    }

    private void OnMouseDown()
    {
        if (InteractionController.Instance.TryClick() && !GameManager.Instance.isLevelFinished)
        {
            transform.DOPunchScale(Vector2.one * 0.05f, 0.5f, 10, 0.5f);
            if (circle.currentType != CircleType.Five)
            {
                UIManager.Instance.UpdateMovesLeftOnClick();
                circle.UpdateCircle();
                GridManager.Instance.TryFindMatchCoroutine();
            }
        }
    }
}
