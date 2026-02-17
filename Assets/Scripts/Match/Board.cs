using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width = 5;
    public int height = 6;

    public float spacingX;
    public float spacingY;
    [SerializeField] private float spacingParameter;

    public GameObject[] flowerPrefabs;

    private Cell[,] board;
    public GameObject boardZone;

    public List<ISwitchable> switchablesToDestroy = new();
    public List<ISwitchable> switchablesToRemove = new();

    [SerializeField] private ISwitchable selectedSwitchable;
    [SerializeField] public bool isProcessingMove;

    public ArrayLayout arrayLayout;
    public static Board instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitializeBoard();
    }

    private void InitializeBoard()
    {
        DestroySwitchables();
        board = new Cell[width, height];

        spacingX = (float)(width - 1) / spacingParameter;
        spacingY = (float)(width - 1) / spacingParameter;
        //spacingY = (float)((height - 1) / spacingParameter) + 1;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 position = new Vector2(x - spacingX, y - spacingY);

                if (arrayLayout.rows[y].row[x])
                {
                    board[x, y] = new Cell(false, null);
                }
                else
                {
                    int randomIndex = Random.Range(0, flowerPrefabs.Length);

                    ISwitchable flower = Instantiate(flowerPrefabs[randomIndex], position, Quaternion.identity).GetComponent<ISwitchable>();
                    flower.SetIndicacies(x, y);
                    flower.GetGameObject().transform.SetParent(boardZone.transform);
                    board[x, y] = new Cell(true, flower);
                    switchablesToDestroy.Add(flower);
                }
            }
        }

        if (CheckBoard())
        {
            InitializeBoard();
        }
    }

    public bool CheckBoard()
    {
        if (GameManager.instance.isGameEnded)
        {
            return false;
        }
        bool hasMatched = false;

        switchablesToRemove = new();

        foreach(Cell cell in board)
        {
            if(cell.switchable != null)
            {
                cell.switchable.isMatched = false;
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (board[x, y].isUsable)
                {
                    ISwitchable switchable = board[x, y].switchable;

                    if (!switchable.isMatched)
                    {
                        MatchResult matchedSwitchables = IsConnected(switchable);

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

            switchable.DestroySwitchable();

            board[xIndex, yIndex] = new Cell(true, null);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (board[x, y].switchable == null)
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

            Vector3 targetPos = new Vector3(x - spacingX, y - spacingY, switchAbove.GetGameObject().transform.position.z);
            switchAbove.MoveToTarget(targetPos);
            switchAbove.SetIndicacies(x, y);
            board[x, y] = board[x, y + yOffset];

            board[x, y + yOffset] = new Cell(true, null);
        }

        if (y + yOffset == height)
        {
            SpawnAtTop(x);
        }
    }

    private void SpawnAtTop(int x)
    {
        int index = FindIndexOfLowestNull(x);
        int locationToMove = height - index;

        int randomIndex = Random.Range(0, flowerPrefabs.Length);
        ISwitchable newFlower = Instantiate(flowerPrefabs[randomIndex], new Vector2(x - spacingX, height - spacingY), Quaternion.identity).GetComponent<ISwitchable>();
        newFlower.GetGameObject().transform.SetParent(boardZone.transform);

        newFlower.SetIndicacies(x, index);
        board[x, index] = new Cell(true, newFlower);

        Vector3 targetPosition = new Vector3(newFlower.GetGameObject().transform.position.x, newFlower.GetGameObject().transform.position.y - locationToMove, newFlower.GetGameObject().transform.position.z);
        newFlower.MoveToTarget(targetPosition);
    }

    private int FindIndexOfLowestNull(int x)
    {
        int lowestNull = 100;

        for(int y = height - 1; y >= 0; y--)
        {
            //Debug.Log(x + "," + y);
            if (board[x,y].switchable == null)
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
                switchable.DestroySwitchable();
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
        }
        else if (selectedSwitchable == switchable)
        {
            selectedSwitchable = null;
        }
        else if (selectedSwitchable != switchable)
        {
            SwapSwitchable(selectedSwitchable, switchable);
            selectedSwitchable = null;
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
        GameManager.instance.ProcessTurn(switchablesToRemove.Count, subtractMoves);

        yield return new WaitForSeconds(0.4f);

        if (CheckBoard())
        {
            StartCoroutine(ProcessTurnOnMatch(false));
        }
    }

    private bool IsAdjacent(ISwitchable currentSwitchable, ISwitchable targetSwitchable)
    {
        return Mathf.Abs(currentSwitchable.xIndex - targetSwitchable.xIndex) + Mathf.Abs(currentSwitchable.yIndex - targetSwitchable.yIndex) == 1;
    }
    #endregion Swapping Potions

    #region Cascading Potions

    #endregion Cascading Potions
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


