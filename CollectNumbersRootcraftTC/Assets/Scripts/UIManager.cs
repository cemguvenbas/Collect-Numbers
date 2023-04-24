using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField] LevelDataSO levelData;

    [Header("Panels")]
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject winPanel;
    public GameObject LosePanel { get => losePanel; set => losePanel = value; }
    public GameObject WinPanel { get => winPanel; set => winPanel = value; }

    [Header("Goal Related")]
    [SerializeField] TextMeshProUGUI[] goalTextUGUIs;
    int movesLeft;
    public int MovesLeft { get => movesLeft; set => movesLeft = value; }

    Goal[] goals;

    [Header("Move Related")]
    [SerializeField] private TextMeshProUGUI movesLeftText;

    [HideInInspector] public static UIManager Instance;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        Instance = this;
    }

    private void Start()
    {
        SetGoalValues();
        SetGoalTexts();
        SetMovesLeft();
    }

    private void SetGoalValues()
    {
        goals = new Goal[levelData.GoalValues.Length];
        for (int i = 0; i < levelData.GoalValues.Length; i++)
        {
            goals[i].Value = levelData.GoalValues[i];
        }
    }

    private void SetGoalTexts()
    {
        for (int i = 0; i < goalTextUGUIs.Length; i++)
        {
            goals[i].TextUGUI = goalTextUGUIs[i];
            UpdateTextInteger(goals[i].TextUGUI, goals[i].Value);
        }
    }

    private void SetMovesLeft()
    {
        movesLeft = levelData.MoveAmount;
        UpdateTextInteger(movesLeftText, movesLeft);
    }

    public void UpdateMovesLeftOnClick()
    {
        if (!GameManager.Instance.isLevelFinished)
        {
            movesLeft--;
            if (movesLeft <= 0)
            {
                movesLeft = 0;
                GameManager.Instance.FinishLevel(false);
            }
            UpdateTextInteger(movesLeftText, movesLeft);
        }
    }

    private void UpdateTextInteger(TextMeshProUGUI text, int amount)
    {
        text.text = amount.ToString();
    }

    public void UpdateGoalOnDestroyCircle(Circle sender)
    {
        switch (sender.currentType)
        {
            case CircleType.One:
                goals[0].Value--;
                if (goals[0].Value <= 0) goals[0].Value = 0;
                UpdateTextInteger(goals[0].TextUGUI, goals[0].Value);
                break;
            case CircleType.Two:
                goals[1].Value--;
                if (goals[1].Value <= 0) goals[1].Value = 0;
                UpdateTextInteger(goals[1].TextUGUI, goals[1].Value);
                break;
            case CircleType.Three:
                goals[2].Value--;
                if (goals[2].Value <= 0) goals[2].Value = 0;
                UpdateTextInteger(goals[2].TextUGUI, goals[2].Value);
                break;
            case CircleType.For:
                goals[3].Value--;
                if (goals[3].Value <= 0) goals[3].Value = 0;
                UpdateTextInteger(goals[3].TextUGUI, goals[3].Value);
                break;
            case CircleType.Five:
                goals[4].Value--;
                if (goals[4].Value <= 0) goals[4].Value = 0;
                UpdateTextInteger(goals[4].TextUGUI, goals[4].Value);
                break;
            default:
                break;
        }

        CheckLevelCompletion();
    }

    private void CheckLevelCompletion()
    {
        bool isAllGoalsCompleted = true;
        for (int i = 0; i < goals.Length; i++)
        {
            if (goals[i].Value != 0) isAllGoalsCompleted = false;
        }

        if (isAllGoalsCompleted && !GameManager.Instance.isLevelFinished)
        {
            GameManager.Instance.FinishLevel(true);
        }
    }

    public void OnClickRestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    [System.Serializable]
    public struct Goal
    {
        public TextMeshProUGUI TextUGUI;
        public int Value;
    }
}
