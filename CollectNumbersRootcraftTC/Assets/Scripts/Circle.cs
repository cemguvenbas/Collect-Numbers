using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Linq;

public class Circle : MonoBehaviour
{
    [HideInInspector] public int X, Y;
    [HideInInspector] public CircleType currentType = CircleType.None;
    [HideInInspector] public bool hasCompletedInit = false;

    [Header("Circle UI Variables")]
    SpriteRenderer spriteRenderer;
    [SerializeField] Color32[] circleColors;
    [SerializeField] private TextMeshPro circleNumber;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(int x, int y, CircleType type, bool isDroppingToY)
    {
        X = x;
        Y = y;

        switch (type)
        {
            case CircleType.One:
                spriteRenderer.color = circleColors[0];
                circleNumber.text = "1";
                currentType = CircleType.One;
                break;
            case CircleType.Two:
                spriteRenderer.color = circleColors[1];
                circleNumber.text = "2";
                currentType = CircleType.Two;
                break;
            case CircleType.Three:
                spriteRenderer.color = circleColors[2];
                circleNumber.text = "3";
                currentType = CircleType.Three;
                break;
            case CircleType.For:
                spriteRenderer.color = circleColors[3];
                circleNumber.text = "4";
                currentType = CircleType.For;
                break;
            case CircleType.Five:
                spriteRenderer.color = circleColors[4];
                circleNumber.text = "5";
                currentType = CircleType.Five;
                break;
            default:
                break;
        }

        if (isDroppingToY) MoveDown(y);
        hasCompletedInit = true;
    }

    public void MoveDown(int y)
    {
        transform.DOMoveY(y, 0.5f);
    }

    public void UpdateCircle()
    {
        switch (currentType)
        {
            case CircleType.One:
                UpdateProperties(2);
                break;
            case CircleType.Two:
                UpdateProperties(3);
                break;
            case CircleType.Three:
                UpdateProperties(4);
                break;
            case CircleType.For:
                UpdateProperties(5);
                break;
            default:
                break;
        }
    }

    private void UpdateProperties(int value)
    {
        spriteRenderer.color = circleColors[value - 1];
        circleNumber.text = $"{value}";
        currentType = (CircleType)(value - 1);
    }

    public void TryFindMatch()
    {
        if (hasCompletedInit && !GameManager.Instance.isLevelFinished)
        {
            if (X > 0 && X < GridManager.Instance.levelData.X - 1) // Checks for right1 and left1
            {
                GetCirclePositionsForMatching(X + 1, Y, X - 1, Y);
            }

            if (X > 1) // Checks for left1 and left2
            {
                GetCirclePositionsForMatching(X - 1, Y, X - 2, Y);
            }

            if (X < GridManager.Instance.levelData.X - 2) // Checks for right1 and right2
            {
                GetCirclePositionsForMatching(X + 1, Y, X + 2, Y);
            }

            // UP AND DOWN
            if (Y > 0 && Y < GridManager.Instance.levelData.Y - 1) // Checks for up1 and down1
            {
                GetCirclePositionsForMatching(X, Y + 1, X, Y - 1);
            }

            if (Y < GridManager.Instance.levelData.Y - 2) // Checks for up1 and up2
            {
                GetCirclePositionsForMatching(X, Y + 1, X, Y + 2);
            }

            if (Y > 1) // Checks for down1 and down2
            {
                GetCirclePositionsForMatching(X, Y - 1, X, Y - 2);
            }
        }
    }
    private void GetCirclePositionsForMatching(int x1, int y1, int x2, int y2)
    {
        int[] circlePos1 = new int[] { x1, y1 };
        int[] circlePos2 = new int[] { x2, y2 };
        CheckMatch(circlePos1, circlePos2);
    }

    private void CheckMatch(int[] circlePos1, int[] circlePos2)
    {
        GameObject circle1GO = GridManager.Instance.CurrentCircles[circlePos1[0], circlePos1[1]];
        GameObject circle2GO = GridManager.Instance.CurrentCircles[circlePos2[0], circlePos2[1]];
        Circle circle1 = circle1GO.GetComponent<Circle>();
        Circle circle2 = circle2GO.GetComponent<Circle>();

        if (circle1.currentType == currentType && circle2.currentType == currentType)
        {
            GameObject[] circleGOs = new GameObject[] { circle1GO, circle2GO, gameObject };
            GridManager.Instance.StoreMatches(circleGOs);
        }
    }
}