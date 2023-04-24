using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/Data/LevelData")]
public class LevelDataSO : ScriptableObject
{

    [SerializeField] private int x;
    [SerializeField] private int y;
    [SerializeField] private int[] goalValues;
    [SerializeField] private int moveAmount;


    public int X { get => x; set => x = value; }
    public int Y { get => y; set => y = value; }
    public int[] GoalValues { get => goalValues; set => goalValues = value; }
    public int MoveAmount { get => moveAmount; set => moveAmount = value; }

    public CircleType[] SpawnList;

    public bool IsListRandom;
    public int RandomSpawnAmount;
}

public enum CircleType
{
    One,
    Two,
    Three,
    For,
    Five,
    None,
}
