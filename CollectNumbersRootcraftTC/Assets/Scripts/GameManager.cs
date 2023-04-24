using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isLevelFinished;


    public static GameManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        Instance = this;
    }

    public void FinishLevel(bool hasWon)
    {
        if (hasWon)
        {
            isLevelFinished = true;
            UIManager.Instance.WinPanel.SetActive(true);
        }
        else
        {
            isLevelFinished = true;
            UIManager.Instance.LosePanel.SetActive(true);
        }
    }

    // Send LevelDataSO to relevant places on level select
    // State structure, etc.
}
