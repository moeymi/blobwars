using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    #region Attributes
    int n, m;
    int[,] gameGrid;
    Vector2Int fromPosition, toPosition;
    BlobAction lastAction;
    List<Vector2Int> changedBlobs;
    #endregion

    public GameState(int n, int m)
    {
        this.n = n;
        this.m = m;
        gameGrid = new int[n, m];
        gameGrid[n - 1, 0] = 1;
        gameGrid[0, m - 1] = -1;

    }

    public GameState(GameState state)
    {
        n = state.n;
        m = state.m;
        gameGrid = (int[,])state.gameGrid.Clone();
        lastAction = state.lastAction;
        fromPosition = state.fromPosition;
        toPosition = state.toPosition;
    }

    public bool CheckMoveAllowed(Vector2Int originalPos, Vector2Int destinationPos)
    {
        int difference = Mathf.Max(
                Mathf.Abs(originalPos.x - destinationPos.x),
                Mathf.Abs(originalPos.y - destinationPos.y)
            );
        return destinationPos.x >= 0 && destinationPos.x < n
            && destinationPos.y >= 0 && destinationPos.y < m
            && GetBlobAtPosition(destinationPos) == 0 && difference <= 2;
    }
    public GameState MakeMove(Vector2Int originalPos, Vector2Int destinationPos, bool isEnemy)
    {
        GameState newState = new GameState(this);
        newState.fromPosition = originalPos;
        newState.toPosition = destinationPos;
        int difference = Mathf.Max(
                Mathf.Abs(originalPos.x - destinationPos.x),
                Mathf.Abs(originalPos.y - destinationPos.y)
            );
        if (difference >= 2)
        {
            newState.SetBlob(destinationPos, isEnemy);
            newState.RemoveBlob(originalPos);
            newState.lastAction = BlobAction.Move;
        }
        else if(difference == 1)
        {
            newState.SetBlob(destinationPos, isEnemy);
            newState.lastAction = BlobAction.Copy;
        }
        newState.changedBlobs = new List<Vector2Int>();
        for (int i = destinationPos.x - 2; i <= destinationPos.x + 2; i++)
        {
            for (int j = destinationPos.y - 2; j <= destinationPos.y + 2; j++)
            {
                if (CheckMoveAllowed(destinationPos, new Vector2Int(i,j)) && gameGrid[i, j] != 0 && gameGrid[i, j] != gameGrid[destinationPos.x, destinationPos.y])
                {
                    gameGrid[i, j] = gameGrid[destinationPos.x, destinationPos.y];
                    changedBlobs.Add(new Vector2Int(i, j));
                }
            }
        }
        return newState;
    }
    public int GetBlobAtPosition(Vector2Int pos)
    {
        return gameGrid[pos.x, pos.y];
    }
    public void SetBlob(Vector2Int position, bool isEnemy)
    {
        gameGrid[position.x, position.y] = isEnemy ? -1 : 1;
    }
    public void RemoveBlob(Vector2Int position)
    {
        gameGrid[position.x, position.y] = 0;
    }
    public List<Vector2Int> GetNextMoves(Vector2Int position)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        for(int i = position.x - 2; i <= position.x + 2; i++)
        {
            for (int j = position.y - 2; j <= position.y + 2; j++)
            {
                if (i == position.x && j == position.y)
                    continue;
                Vector2Int newPos = new Vector2Int(i, j);
                if (CheckMoveAllowed(position, newPos))
                {
                    positions.Add(newPos);
                }
            }
        }
        return positions;
    }
    public string GetStringHashCode()
    {
        string str = "";
        for(int i = 0; i < n; i++)
        {
            for(int j = 0;j < m; j++)
            {
                str += gameGrid[i, j].ToString();
            }
        }
        return str;
    }
    public BlobAction LastAction
    {
        get { return lastAction; }
    }
    public Vector2Int FromPosition
    {
        get { return fromPosition; }
    }
    public Vector2Int ToPosition
    {
        get { return toPosition; }
    }
    public int[,] GameGrid
    {
        get { return gameGrid; }
    }
    public List<Vector2Int> ChangedBlobs
    {
        get { return changedBlobs; }
    }
}
