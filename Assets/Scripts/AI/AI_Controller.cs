using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AI_Controller
{
    async public void AIMakeMove(GameState state)
    {
        float stTime = (Time.realtimeSinceStartup);
        await Task.Delay(670);
        Debug.LogWarning("Start searching ...");
        Node selectedNode = null;
        await Task.Run( async () =>
        {
            selectedNode = await MinMaxSolver(2, new Node (state), true);
        });
        if (selectedNode != null)
            GameManager.RunIntoState(selectedNode.nextState);

        Debug.LogWarning("FINISH");
        Debug.LogWarning("Time = " + (Time.realtimeSinceStartup - stTime));

    }

    async Task<Node> MinMaxSolver(int depth , Node currentNode , bool AI_Trun )
    {
        return await Task.Run(async () =>
        {
            List<GameState> nextStates = currentNode.state.getNextStates();
            if (depth == 0 || nextStates.Count == 0)
            {
                currentNode.cost = currentNode.state.evaluate();
                return currentNode;
            }
            currentNode.cost = AI_Trun ? int.MinValue : int.MaxValue;
            foreach (GameState state in nextStates)
            {
                Node newNode = new Node(state);
                Node nextNode = await MinMaxSolver(depth - 1, newNode, !AI_Trun);
                if (depth == 3)
                {
                    ;
                }
                if (AI_Trun)
                {
                    if (currentNode.cost < nextNode.cost)
                    {
                        currentNode.nextState = nextNode.state;
                        currentNode.cost = nextNode.cost;
                    }
                }
                else
                {
                    if (currentNode.cost > nextNode.cost)
                    {
                        currentNode.nextState = nextNode.state;
                        currentNode.cost = nextNode.cost;
                    }
                }
            }
            if(depth == 3)
            {
                ;
            }
            return currentNode;
       });
    }
    public class Node
    {
        public GameState state;
        public GameState nextState;
        public int cost = 0; 
        public Node() { }
        public Node (GameState currentState)
        {
            this.state = currentState;
        }
    } 
}
