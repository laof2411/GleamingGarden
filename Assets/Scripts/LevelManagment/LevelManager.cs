using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("External References")]
    [SerializeField] private Board boardReference;
    [SerializeField] private LevelCanvas levelCanvasReference;

    [Header("Level Data")]
    [SerializeField] private LevelData levelData;

    public int goal;
    public int moves;
    public int points;
    public float time;

    public bool isGameEnded;
    public bool isPaused;

    private void Start()
    {        
        InitializeGame();
    }

    public void InitializeGame()
    {
        levelData = GameManager.instance.currentLevelData;
        InsertLevelData();
        boardReference.IntroduceLevelData(levelData);

        levelCanvasReference.IntroduceInitialValues();

        if(GameManager.instance.currentLevelData.levelTime > 0)
        {
            StartCoroutine(TimerCoroutine(GameManager.instance.currentLevelData.levelTime));
        }
    }

    public void InsertLevelData()
    {
        points = 0;
        goal = levelData.levelObjective;
        moves = levelData.levelMoves;
        time = levelData.levelTime;
    }

    public void ProcessTurn(int _pointsToGain, bool _substractMoves)
    {
        points += _pointsToGain;
        levelCanvasReference.UpdatePoints();
        if (_substractMoves)
        {
            levelCanvasReference.UpdateLevelMoves();
            moves--;
        }

        if (points >= goal)
        {
            isGameEnded = true;
            levelCanvasReference.OpenVictoryScreen();
            AudioManager.instance.PlayAudio("VictorySound");
            GameManager.instance.levelsCompleted[levelData.levelID] = true;
            return;
        }
        else if (moves <= 0)
        {
            isGameEnded = true;
            AudioManager.instance.PlayAudio("DefeatSound");
            levelCanvasReference.OpenDefeatScreen();
            return;
        }
    }

    private IEnumerator TimerCoroutine(float _time)
    {
        while(_time > 0)
        {
            if (isGameEnded)
            {
                yield return null;
            }

            if (!isPaused)
            {
                _time -= Time.deltaTime;
                float showTime = Mathf.FloorToInt(_time);
                if (showTime < 0)
                {
                    showTime = 0;
                }
                time = showTime;
                levelCanvasReference.UpdateTime();
            }
            yield return new WaitForSeconds(0.001f);
        }
        isGameEnded = true;
        levelCanvasReference.OpenDefeatScreen();
    }
}
