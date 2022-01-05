using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

public class AI_Controller
{
    async public void AIMakeMove(GameState state)
    {
        ClearLogs();
        Debug.Log("Thinking..");
        float stTime = (Time.realtimeSinceStartup);
        await Task.Delay(670);
        Node selectedNode = null;
        await Task.Run( async () =>
        {
            selectedNode = await MinMaxSolver(3, new Node (state, null), true, true);
        });
        if (selectedNode != null)
            GameManager.RunIntoState(selectedNode.nextState);
        Debug.Log("Last move with " + (Time.realtimeSinceStartup - stTime) + " seconds.");

    }

    async Task<Node> MinMaxSolver(int depth , Node currentNode , bool AI_Trun , bool cutAlphaBeta)
    {
        return await Task.Run(async () =>
        {
            List<GameState> nextStates = currentNode.state.getNextStates();
            if (depth == 0 || nextStates.Count == 0)
            {
                currentNode.value = currentNode.state.evaluate();
                return currentNode;
            }
            currentNode.value = AI_Trun ? int.MinValue : int.MaxValue;
            foreach (GameState state in nextStates)
            {
                Node newNode = new Node(state, currentNode);
                Node nextNode = await MinMaxSolver(depth - 1, newNode, !AI_Trun, cutAlphaBeta);
                if (depth == 3)
                {
                    ;
                }
                if (AI_Trun)
                {
                    if (currentNode.value < nextNode.value)
                    {
                        currentNode.nextState = nextNode.state;
                        currentNode.value = nextNode.value;
                    }
                    if (cutAlphaBeta && currentNode.prevNode != null && currentNode.prevNode.value <= nextNode.value)
                    {
                        return currentNode;
                    }
                }
                else
                {
                    if (currentNode.value > nextNode.value)
                    {
                        currentNode.nextState = nextNode.state;
                        currentNode.value = nextNode.value;
                    }
                    if (cutAlphaBeta && currentNode.prevNode != null && currentNode.prevNode.value >= nextNode.value)
                    {
                        return currentNode;
                    }
                }
            }
            return currentNode;
       });
    }
    public class Node
    {
        public GameState state;
        public GameState nextState;
        public Node prevNode;
        public int value = 0; 
        public Node() { }
        public Node (GameState currentState,Node prevNode)
        {
            this.state = currentState;
            this.prevNode = prevNode;
        }
    }

    void ClearLogs()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
}
