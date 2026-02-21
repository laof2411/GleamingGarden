using System.Collections.Generic;
using UnityEngine;

public class LevelDataHolder : MonoBehaviour
{
    [SerializeField] private List<LevelData> levelDatas;

    public LevelData AccessLevelData(string _levelName)
    {
        LevelData selectedData = null;

        foreach (var levelData in levelDatas)
        {
            if(_levelName == levelData.name)
            {
                selectedData = levelData;
            }
        }
        return selectedData;
    }
}
