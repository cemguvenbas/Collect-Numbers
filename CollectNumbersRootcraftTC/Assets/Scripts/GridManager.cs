using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridManager : MonoBehaviour
{
    [Header("Tile Reference")]
    [SerializeField] Circle circle;

    [Header("Spawn Related")]
    public LevelDataSO levelData;
    [SerializeField] CircleType[] spawnList;
    int spawnCounter;
    Transform circlesParent;

    [Header("Current Circles")]
    public List<GameObject> currentCirclesList;
    public GameObject[,] CurrentCircles;

    [Header("Match List")]
    [SerializeField] List<GameObject> matchedCircles;

    public static GridManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    private void Start()
    {
        circlesParent = GameObject.FindGameObjectWithTag("CirclesParent").transform;
        CurrentCircles = new GameObject[levelData.X, levelData.Y];
        SetSpawnList();
        GenerateCircles();
        TryFindMatchCoroutine();
    }

    private void SetSpawnList()
    {
        if (levelData.IsListRandom)
        {
            int randomizer = 0;
            spawnList = new CircleType[levelData.RandomSpawnAmount];
            for (int i = 0; i < spawnList.Length; i++)
            {
                randomizer = Random.Range(0, (int)CircleType.None);
                spawnList[i] = (CircleType)randomizer;
            }
        }
        else spawnList = levelData.SpawnList;
    }

    #region CircleGeneration
    void GenerateCircles()
    {
        spawnCounter = 0;

        for (int x = 0; x < levelData.X; x++)
        {
            for (int y = 0; y < levelData.Y; y++)
            {
                GenerateCircle(x, y, false);
            }
        }
        UpdateList();
        Camera.main.transform.position = new Vector3(levelData.X / 2f - 0.5f, levelData.Y / 2f - 0.5f, -10);
    }

    public void GenerateCircleAtTop(int x, int y) // Spawns at top and saves in passed position
    {
        GenerateCircle(x, y, true);
    }

    private void GenerateCircle(int x, int y, bool generateAtTop)
    {
        Vector3 position = new Vector3();
        if (generateAtTop) position.Set(x, y + levelData.Y, position.z); // If spawn on top sets the position.y to one above the grid's Y
        else position.Set(x, y, position.z); // Else spawns at position

        var spawnedCircle = Instantiate(circle, position, Quaternion.identity);
        if (spawnedCircle.gameObject != null) CurrentCircles[x, y] = spawnedCircle.gameObject;

        spawnedCircle.name = $"Circle {x} {y}";
        spawnedCircle.transform.SetParent(circlesParent);
        spawnedCircle.Init(x, y, spawnList[spawnCounter], generateAtTop);

        spawnCounter++;
    }
    #endregion

    #region CircleMatching
    public void TryFindMatchCoroutine()
    {
        StartCoroutine(TryFindMatch());
    }

    IEnumerator TryFindMatch()
    {
        yield return new WaitForEndOfFrame();
        if (!GameManager.Instance.isLevelFinished)
        {
            TryFindMatchInAllActiveCircles();
            if (matchedCircles.Count > 0) StartCoroutine(DestroyMatches());
        }
    }

    private void TryFindMatchInAllActiveCircles()
    {
        foreach (var circleGO in currentCirclesList)
        {
            circleGO.GetComponent<Circle>().TryFindMatch();
        }
    }

    private void UpdateList()
    {
        currentCirclesList.RemoveRange(0, currentCirclesList.Count);
        for (int i = 0; i < CurrentCircles.GetLength(0); i++)
        {
            for (int j = 0; j < CurrentCircles.GetLength(1); j++)
            {
                if (!currentCirclesList.Contains(CurrentCircles[i, j])) currentCirclesList.Add(CurrentCircles[i, j]);
            }
        }
    }

    public void StoreMatches(GameObject[] circleGOs)
    {
        // Set positions for moving
        foreach (var circleGO in circleGOs)
        {
            if (!matchedCircles.Contains(circleGO)) matchedCircles.Add(circleGO);
        }
    }

    public IEnumerator DestroyMatches()
    {
        foreach (var matchedCircle in matchedCircles)
        {
            UIManager.Instance.UpdateGoalOnDestroyCircle(matchedCircle.GetComponent<Circle>());
            Destroy(matchedCircle, 0.25f);
        }
        matchedCircles.RemoveRange(0, matchedCircles.Count);
        yield return new WaitForSeconds(0.25f);
        yield return new WaitForEndOfFrame();
        RefillBoard();
    }

    void RefillBoard()
    {
        for (int x = 0; x < levelData.X; x++)
        {
            for (int y = 0; y < levelData.Y; y++)
            {
                if (CurrentCircles[x, y] == null) // If there is no GameObject on position
                {
                    bool isReplacementFound = false;

                    for (int i = y; i < levelData.Y; i++) // Try to find a replacement in the existing circles in the range of y - Y
                    {
                        GameObject circleGO = CurrentCircles[x, i];
                        if (circleGO != null) // If a matching circle is found in the [x, i]
                        {
                            // Replace the matching circle to the missing circle's position
                            CurrentCircles[x, y] = circleGO;
                            circleGO.name = $"Circle {x} {y}"; // Rename the circle
                            CurrentCircles[x, i] = null; // Null the replaced position

                            var circle = circleGO.GetComponent<Circle>();
                            circle.Y = y;
                            circle.MoveDown(y); // Move the circle

                            isReplacementFound = true;
                            break;
                        }
                    }
                    if (!isReplacementFound) GenerateCircleAtTop(x, y);
                }
            }
        }
        UpdateList();
        if (!GameManager.Instance.isLevelFinished) Invoke("TryFindMatchCoroutine", 0.5f);
        Invoke("CheckIfGameIsOver", 0.5f);
    }

    private void CheckIfGameIsOver()
    {
        if (UIManager.Instance.MovesLeft == 0) GameManager.Instance.FinishLevel(false);
    }
    #endregion
}
