using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Board : MonoBehaviour
{
    public int width = 5;
    public int height = 6;

    public float spacing;

    public GameObject[] flowerPrefabs;
    public GameObject poofVFXPrefab;
    public GameObject cellPrefab;

    private Cell[,] board;
    public GameObject boardZone;
    public GameObject highlight;

    public List<ISwitchable> switchablesToDestroy = new();
    public List<ISwitchable> switchablesToRemove = new();

    [SerializeField] private ISwitchable selectedSwitchable;
    [SerializeField] public bool isProcessingMove;

    public ArrayLayout cellLayout;

    [Header("External References")]
    [SerializeField] private LevelManager levelManager;

    public void IntroduceLevelData(LevelData _levelData)
    {
        width = _levelData.levelWidth;
        height = _levelData.levelHeight;
        cellLayout = _levelData.levelCellLayout;

        InitializeBoard(true);
    }

    public void InitializeBoard(bool _firstime)
    {
        DestroySwitchables();
        board = new Cell[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float yValue;
                float xValue;
                if (height % 2 == 0)
                {
                    yValue = ((y * spacing) + (spacing / 2)) - (height / 2 * spacing);
                }
                else
                {
                    yValue = (y * spacing) - (height / 2 * spacing);
                }

                if (width % 2 == 0)
                {
                    xValue = ((x * spacing) + (spacing / 2)) - (width / 2 * spacing);
                }
                else
                {
                    xValue = (x * spacing) - (width / 2 * spacing);
                }

                Vector3 position = new Vector3(xValue, yValue, 0);

                if (cellLayout.rows[y].row[x])
                {
                    board[x, y] = new Cell(false, null, null, position);
                }
                else
                {
                    int randomIndex = Random.Range(0, flowerPrefabs.Length);

                    if (_firstime)
                    {
                        GameObject cell = Instantiate(cellPrefab, position + new Vector3(0, 0, 1f), Quaternion.identity);
                        cell.transform.SetParent(boardZone.transform);
                    }

                    ParticleSystem poofEffect = Instantiate(poofVFXPrefab, position, Quaternion.identity).GetComponent<ParticleSystem>();
                    poofEffect.transform.Rotate(new Vector3(-90, 0, 0));
                    ISwitchable flower = Instantiate(flowerPrefabs[randomIndex], position, Quaternion.identity).GetComponent<ISwitchable>();
                    flower.SetIndicacies(x, y);

                    flower.GetGameObject().transform.SetParent(boardZone.transform);
                    poofEffect.gameObject.transform.SetParent(boardZone.transform);

                    board[x, y] = new Cell(true, flower, poofEffect, position);
                    switchablesToDestroy.Add(flower);
                }
            }
        }

        if (CheckBoard())
        {
            InitializeBoard(false);
        }
    }

    public bool CheckBoard()
    {
        if (levelManager.isGameEnded)
        {
            return false;
        }
        bool hasMatched = false;

        switchablesToRemove = new();

        foreach (Cell cell in board)
        {
            if (cell.switchable != null)
            {
                cell.switchable.isMatched = false;
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!board[x, y].isUsable)
                {
                    continue;
                }
                if (board[x,y].switchable.isMatched)
                {
                    continue;
                }

                MatchResult matchedSwitchables = IsConnected(board[x, y].switchable);

                if (matchedSwitchables.connectedSwitchables.Count >= 3)
                {
                    MatchResult specialMatch = SuperMatch(matchedSwitchables);
                    switchablesToRemove.AddRange(specialMatch.connectedSwitchables);

                    foreach (ISwitchable switcha in specialMatch.connectedSwitchables)
                    {
                        switcha.isMatched = true;
                    }
                    hasMatched = true;
                }
            }
        }
        return hasMatched;
    }

    public void RemoveAndRefill(List<ISwitchable> switchables)
    {
        foreach (ISwitchable switchable in switchables)
        {
            int xIndex = switchable.xIndex;
            int yIndex = switchable.yIndex;

            switchable.DisableSwitchable();

            board[xIndex, yIndex].switchable = null;
            board[xIndex, yIndex].dissapearEffect.Play();
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (board[x, y].switchable == null && board[x, y].isUsable)
                {
                    RefillSwitchable(x, y);
                }
            }
        }
    }

    public void RefillSwitchable(int x, int y)
    {
        int yOffset = 1;

        while (y + yOffset < height && board[x, y + yOffset].switchable == null)
        {
            yOffset++;
        }

        if (y + yOffset < height && board[x, y + yOffset].switchable != null)
        {
            ISwitchable switchAbove = board[x, y + yOffset].switchable;

            Vector3 targetPos = board[x, y].switchablePosition;
            switchAbove.MoveToTarget(targetPos);
            switchAbove.SetIndicacies(x, y);

            board[x, y].switchable = switchAbove;
            board[x, y + yOffset].switchable = null;
        }
        else if (y + yOffset == height)
        {
            SpawnAtTop(x);
        }
    }

    private void SpawnAtTop(int x)
    {
        int index = FindIndexOfLowestNull(x);

        int randomIndex = Random.Range(0, flowerPrefabs.Length);
        ISwitchable newFlower = Instantiate(flowerPrefabs[randomIndex], board[x, index].switchablePosition + new Vector3(0, height + (index * spacing), 0), Quaternion.identity).GetComponent<ISwitchable>();
        newFlower.GetGameObject().transform.SetParent(boardZone.transform);

        newFlower.SetIndicacies(x, index);
        board[x, index].switchable = newFlower;

        newFlower.MoveToTarget(board[x, index].switchablePosition);
    }

    private int FindIndexOfLowestNull(int x)
    {
        int lowestNull = 100;

        for (int y = height - 1; y >= 0; y--)
        {
            //Debug.Log(x + "," + y);
            if (board[x, y].switchable == null && board[x, y].isUsable)
            {
                lowestNull = y;
            }
        }
        return lowestNull;
    }

    public MatchResult SuperMatch(MatchResult matchedSwitchables)
    {
        if (matchedSwitchables.matchType == MatchType.Horizontal)
        {
            foreach (ISwitchable switcha in matchedSwitchables.connectedSwitchables)
            {
                List<ISwitchable> extraSwitchables = new();

                CheckDirection(switcha, new Vector2Int(0, 1), extraSwitchables);
                CheckDirection(switcha, new Vector2Int(0, -1), extraSwitchables);

                if (extraSwitchables.Count >= 2)
                {
                    extraSwitchables.AddRange(matchedSwitchables.connectedSwitchables);

                    return new MatchResult
                    {
                        connectedSwitchables = extraSwitchables,
                        matchType = MatchType.Super,
                    };
                }
            }
            return new MatchResult
            {
                connectedSwitchables = matchedSwitchables.connectedSwitchables,
                matchType = matchedSwitchables.matchType,
            };
        }
        else if (matchedSwitchables.matchType == MatchType.Vertical)
        {
            foreach (ISwitchable switcha in matchedSwitchables.connectedSwitchables)
            {
                List<ISwitchable> extraSwitchables = new();

                CheckDirection(switcha, new Vector2Int(1, 0), extraSwitchables);
                CheckDirection(switcha, new Vector2Int(-1, 0), extraSwitchables);

                if (extraSwitchables.Count >= 2)
                {
                    extraSwitchables.AddRange(matchedSwitchables.connectedSwitchables);

                    return new MatchResult
                    {
                        connectedSwitchables = extraSwitchables,
                        matchType = MatchType.Super,
                    };
                }
            }
            return new MatchResult
            {
                connectedSwitchables = matchedSwitchables.connectedSwitchables,
                matchType = matchedSwitchables.matchType,
            };
        }
        return null;
    }

    MatchResult IsConnected(ISwitchable switchable)
    {
        List<ISwitchable> connectedSwitchables = new();
        SwitchableType flowerType = switchable.type;

        connectedSwitchables.Add(switchable);

        CheckDirection(switchable, new Vector2Int(1, 0), connectedSwitchables);
        CheckDirection(switchable, new Vector2Int(-1, 0), connectedSwitchables);

        if (connectedSwitchables.Count >= 3)
        {
            switch (connectedSwitchables.Count)
            {
                case 3:
                    {
                        return new MatchResult
                        {
                            connectedSwitchables = connectedSwitchables,
                            matchType = MatchType.Horizontal,
                        };
                    }
                case 4:
                    {
                        return new MatchResult
                        {
                            connectedSwitchables = connectedSwitchables,
                            matchType = MatchType.Horizontal,
                        };
                    }
                case 5:
                    {
                        return new MatchResult
                        {
                            connectedSwitchables = connectedSwitchables,
                            matchType = MatchType.Horizontal,
                        };
                    }
                default:
                    {
                        return new MatchResult
                        {
                            connectedSwitchables = connectedSwitchables,
                            matchType = MatchType.Horizontal,
                        };
                    }
            }
        }
        connectedSwitchables.Clear();
        connectedSwitchables.Add(switchable);

        CheckDirection(switchable, new Vector2Int(0, 1), connectedSwitchables);
        CheckDirection(switchable, new Vector2Int(0, -1), connectedSwitchables);

        if (connectedSwitchables.Count >= 3)
        {
            switch (connectedSwitchables.Count)
            {
                case 3:
                    {
                        return new MatchResult
                        {
                            connectedSwitchables = connectedSwitchables,
                            matchType = MatchType.Vertical,
                        };
                    }
                case 4:
                    {
                        return new MatchResult
                        {
                            connectedSwitchables = connectedSwitchables,
                            matchType = MatchType.Vertical,
                        };
                    }
                case 5:
                    {
                        return new MatchResult
                        {
                            connectedSwitchables = connectedSwitchables,
                            matchType = MatchType.Vertical,
                        };
                    }
                default:
                    {
                        return new MatchResult
                        {
                            connectedSwitchables = connectedSwitchables,
                            matchType = MatchType.Vertical,
                        };
                    }
            }
        }
        else
        {
            return new MatchResult
            {
                connectedSwitchables = connectedSwitchables,
                matchType = MatchType.None,
            };
        }
    }

    public void CheckDirection(ISwitchable switchable, Vector2Int direction, List<ISwitchable> connectedSwitchables)
    {
        SwitchableType type = switchable.type;
        int x = switchable.xIndex + direction.x;
        int y = switchable.yIndex + direction.y;

        while (x >= 0 && x < width && y >= 0 && y < height)
        {
            if (board[x, y].isUsable)
            {
                ISwitchable neighbourSwitchable = board[x, y].switchable;

                if (!neighbourSwitchable.isMatched && neighbourSwitchable.type == type)
                {
                    connectedSwitchables.Add(neighbourSwitchable);

                    x += direction.x;
                    y += direction.y;
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }
    }

    public void DestroySwitchables()
    {
        if (switchablesToDestroy != null)
        {
            foreach (ISwitchable switchable in switchablesToDestroy)
            {
                switchable.DisableSwitchable();
            }
            switchablesToDestroy.Clear();
        }
    }

    #region Swapping Potions
    public void SelectSwitchable(ISwitchable switchable)
    {
        if (selectedSwitchable == null)
        {
            selectedSwitchable = switchable;
            highlight.transform.position = new Vector3(switchable.GetGameObject().transform.position.x, switchable.GetGameObject().transform.position.y, 0.5f);
            highlight.GetComponent<ParticleSystem>().Play();
        }
        else if (selectedSwitchable == switchable)
        {
            selectedSwitchable = null;
            highlight.GetComponent<ParticleSystem>().Stop();
        }
        else if (selectedSwitchable != switchable)
        {
            SwapSwitchable(selectedSwitchable, switchable);
            selectedSwitchable = null;
            highlight.GetComponent<ParticleSystem>().Stop();
        }
    }

    public void SwapSwitchable(ISwitchable currentSwitchable, ISwitchable targetSwitchable)
    {
        if (!IsAdjacent(currentSwitchable, targetSwitchable))
        {
            return;
        }

        DoSwap(currentSwitchable, targetSwitchable);
        isProcessingMove = true;

        StartCoroutine(ProcessMatches(currentSwitchable, targetSwitchable));
    }
    private bool IsAdjacent(ISwitchable currentSwitchable, ISwitchable targetSwitchable)
    {
        return Mathf.Abs(currentSwitchable.xIndex - targetSwitchable.xIndex) + Mathf.Abs(currentSwitchable.yIndex - targetSwitchable.yIndex) == 1;
    }

    private void DoSwap(ISwitchable currentSwitchable, ISwitchable targetSwitchable)
    {
        ISwitchable temp = board[currentSwitchable.xIndex, currentSwitchable.yIndex].switchable;

        board[currentSwitchable.xIndex, currentSwitchable.yIndex].switchable = board[targetSwitchable.xIndex, targetSwitchable.yIndex].switchable;
        board[targetSwitchable.xIndex, targetSwitchable.yIndex].switchable = temp;

        int tempX = currentSwitchable.xIndex;
        int tempY = currentSwitchable.yIndex;

        currentSwitchable.SetIndicacies(targetSwitchable.xIndex, targetSwitchable.yIndex);
        targetSwitchable.SetIndicacies(tempX, tempY);

        currentSwitchable.MoveToTarget(targetSwitchable.GetGameObject().transform.position);
        targetSwitchable.MoveToTarget(currentSwitchable.GetGameObject().transform.position);
        
        AudioManager.instance.PlayAudio("Woosh");
    }

    private IEnumerator ProcessMatches(ISwitchable currentSwitchable, ISwitchable targetSwitchable)
    {
        yield return new WaitForSeconds(0.2f);

        if (CheckBoard())
        {
            StartCoroutine(ProcessTurnOnMatch(true));
        }
        else
        {
            DoSwap(currentSwitchable, targetSwitchable);
        }

        isProcessingMove = false;
    }


    private IEnumerator ProcessTurnOnMatch(bool subtractMoves)
    {
        foreach (ISwitchable switchableToRemove in switchablesToRemove)
        {
            switchableToRemove.isMatched = false;
        }
        RemoveAndRefill(switchablesToRemove);

        if(GameManager.instance.currentLevelData.levelMoves > 0)
        {
            levelManager.ProcessTurn(switchablesToRemove.Count, subtractMoves);
        }
        else
        {
            levelManager.ProcessTurn(switchablesToRemove.Count, false);
        }
        

        yield return new WaitForSeconds(0.4f);

        if (CheckBoard())
        {
            StartCoroutine(ProcessTurnOnMatch(false));
        }
    }

    private IEnumerator TurnOffEffects(List<VisualEffect> _visualEffects)
    {
        yield return new WaitForSeconds(0.3f);

        foreach (VisualEffect effect in _visualEffects)
        {
            effect.enabled = false;
        }
    }

    #endregion Swapping Potions
}

public class MatchResult
{
    public List<ISwitchable> connectedSwitchables;
    public MatchType matchType;
}

public enum MatchType
{
    None,
    Vertical,
    Horizontal,
    Square,
    Super,
}


