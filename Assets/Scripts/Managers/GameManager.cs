using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Attributes
    static Dictionary<Vector2Int, BlobController> blobs = new Dictionary<Vector2Int, BlobController>();
    static Dictionary<int, GameObject> blobPrefabs = new Dictionary<int, GameObject>();
    static GameState currentGameState;
    static AI_Controller ai_Controller = new AI_Controller(); 
    static int n = 8, m = 8;
    #endregion
    private void Start()
    {
        WorldGenerator.GenerateWorld(n, m);
        blobPrefabs.Add(1, Resources.Load<GameObject>("Prefabs/Blob"));
        blobPrefabs.Add(-1, Resources.Load<GameObject>("Prefabs/EnemyBlob"));
        currentGameState = new GameState(n, m);
        //First Blob
        GameObject firstBlob = Instantiate(blobPrefabs[1]);
        Vector2Int position = new Vector2Int(n - 1, 0);
        BlobController controller = firstBlob.GetComponent<BlobController>();
        controller.Initialize(position);
        blobs.Add(position, controller);

        //Second Blob
        GameObject enemyBlob = Instantiate(blobPrefabs[-1]);
        position = new Vector2Int(0, m-1);
        controller = enemyBlob.GetComponent<BlobController>();
        controller.Initialize(position);
        blobs.Add(position, controller);
    }

    //For Human
    public static void TryMove(Vector2Int originalPos, Vector2Int destinationPos)
    {
        if (currentGameState.CheckMoveAllowed(originalPos, destinationPos))
        {
            GameState newState = currentGameState.MakeMove(originalPos, destinationPos, false);
            RunIntoState(newState);
        }
    }

    public static void RunIntoState(GameState state)
    {
        Vector2Int originalPos = state.FromPosition;
        Vector2Int destinationPos = state.ToPosition;
        currentGameState = state;
        if (state.LastAction == BlobAction.Copy)
        {
            //Squash Original Blob
            blobs[state.FromPosition].MakeMove(state.FromPosition, state.LastAction);
        }
        else
        {
            //Squash and move Original Blob
            blobs[state.FromPosition].MakeMove(state.ToPosition, state.LastAction);
            blobs.Add(state.ToPosition, blobs[state.FromPosition]);
            blobs.Remove(state.FromPosition);
        }
    }
    public static void ShowAvailableMoves(Vector2Int position)
    {
        List<Vector2Int> positions = currentGameState.GetNextMoves(position);
        WorldGenerator.ShowAvailableMoves(positions);
    }
    public static void NormalizeTiles()
    {
        WorldGenerator.NormalizeTiles();
    }
    public static void CopyBlob()
    {
        GameObject newBlob = Instantiate(blobPrefabs[currentGameState.GetBlobAtPosition(currentGameState.ToPosition)]);
        BlobController controller = newBlob.GetComponent<BlobController>();
        controller.Initialize(currentGameState.ToPosition);
        blobs.Add(currentGameState.ToPosition, controller);
        Debug.Log(currentGameState.ToPosition);
    }

    public static void TakeOver()
    {
        List<Vector2Int> changedBlobs = currentGameState.ChangedBlobs;
        foreach (Vector2Int pos in changedBlobs)
        {
            if (blobs.ContainsKey(pos))
            {
                Debug.Log("Here : " + pos.ToString());
                Destroy(blobs[pos].gameObject);
                GameObject newBlob = Instantiate(blobPrefabs[currentGameState.GetBlobAtPosition(currentGameState.ToPosition)]);
                BlobController controller = newBlob.GetComponent<BlobController>();
                controller.Initialize(pos);
                blobs[pos] = controller;
            }
        }
    }

    public static GameState CurrentGamestate
    {
        get { return currentGameState; }
    }
    public static AI_Controller AIController
    {
        get { return ai_Controller; }
    }
}