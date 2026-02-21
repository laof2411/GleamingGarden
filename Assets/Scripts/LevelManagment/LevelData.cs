using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    public string levelName;
    public int levelID;

    public int levelWidth;
    public int levelHeight;
    public int levelMoves;
    public int levelObjective;
    public float levelTime;
    public ArrayLayout levelCellLayout;
}
