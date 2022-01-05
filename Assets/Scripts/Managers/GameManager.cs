using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Attributes
    static Dictionary<Vector2Int, BlobController> blobs = new Dictionary<Vector2Int, BlobController>();
    static Dictionary<int, GameObject> blobPrefabs = new Dictionary<int, GameObject>();
    static GameState currentGameState;
    static AI_Controller ai_Controller = new AI_Controller(); 
    static int n = 6, m = 6;
    static bool canMove = true;
    static Transform blobsParent;
    #endregion
    private void Start()
    {
        WorldGenerator.GenerateWorld(n, m); 
        blobPrefabs = new Dictionary<int, GameObject>();
        blobs = new Dictionary<Vector2Int, BlobController>();
        blobPrefabs.Add(1, Resources.Load<GameObject>("Prefabs/Blob"));
        blobPrefabs.Add(-1, Resources.Load<GameObject>("Prefabs/EnemyBlob"));
        currentGameState = new GameState(n, m);
        blobsParent = GameObject.FindGameObjectWithTag("BlobsParent").transform;

        //First Blob
        GameObject firstBlob = Instantiate(blobPrefabs[1]);
        firstBlob.transform.SetParent(blobsParent);
        Vector2Int position = new Vector2Int(n - 1, 0);
        BlobController controller = firstBlob.GetComponent<BlobController>();
        controller.Initialize(position);
        blobs.Add(position, controller);

        //Second Blob
        GameObject enemyBlob = Instantiate(blobPrefabs[-1]);
        enemyBlob.transform.SetParent(blobsParent);
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
            AIController.AIMakeMove(currentGameState);
        }
    }

    public static void RunIntoState(GameState state)
    {
        if (state == null)
        {
            Endgame();
            return;
        }
        canMove = false;
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
        GameObject.FindGameObjectWithTag("PlayerBlobs").GetComponent<TextMeshProUGUI>().text = currentGameState.GetBlobNum(false).ToString();
        GameObject.FindGameObjectWithTag("AIBlobs").GetComponent<TextMeshProUGUI>().text = currentGameState.GetBlobNum(true).ToString();
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
        newBlob.transform.SetParent(blobsParent);
        BlobController controller = newBlob.GetComponent<BlobController>();
        controller.Initialize(currentGameState.ToPosition);
        blobs.Add(currentGameState.ToPosition, controller);
    }
    public static void TakeOver()
    {
        List<Vector2Int> changedBlobs = currentGameState.ChangedBlobs;
        foreach (Vector2Int pos in changedBlobs)
        {
            if (blobs.ContainsKey(pos))
            {
                Destroy(blobs[pos].gameObject);
                GameObject newBlob = Instantiate(blobPrefabs[currentGameState.GetBlobAtPosition(currentGameState.ToPosition)]);
                BlobController controller = newBlob.GetComponent<BlobController>();
                controller.Initialize(pos);
                blobs[pos] = controller;
            }
        }
    }

    public static void FinishAI()
    {
        canMove = true;
        if (currentGameState.getNextStates().Count == 0)
            Endgame();
    }

    public static GameState CurrentGamestate
    {
        get { return currentGameState; }
    }
    public static AI_Controller AIController
    {
        get { return ai_Controller; }
    }
    public static bool CanMove
    {
        get { return canMove; }
    }

    public static void Endgame()
    {
        int difference = currentGameState.evaluate();
        UIManager.EndGame(difference);
    }
}