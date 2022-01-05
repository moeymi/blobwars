using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AI_Controller : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    async public void AIMakeMove()
    {
        float stTime = (Time.realtimeSinceStartup);
        Debug.LogWarning("Start searching ...");

        Node selectedNode = null;
        await Task.Run( async () =>
        {
            selectedNode = await MinMaxSolver(3, new Node (GameManager.CurrentGamestate , null), true);
            return 0;
        });
        if (selectedNode != null)
            GameManager.RunIntoState(selectedNode.state);

        Debug.LogWarning("FINISH");
        Debug.LogWarning("Time = " + (Time.realtimeSinceStartup - stTime));
    }

    async Task<Node> MinMaxSolver(int depth , Node currentState , bool AI_Trun )
    {
        if (depth == 0)
            return null;
        return await Task.Run(async () =>
       {
           List<GameState> nextStates = currentState.state.getNextStates();
           List<Node> nextNodes = new List<Node>();
           foreach (GameState state in nextStates)
           {
               Node newNode = new Node(state, currentState);
               Node optimalState = await MinMaxSolver(depth - 1, newNode, !AI_Trun);
               if (optimalState == null)
               {
                   newNode.state.calculateCost();
               }
               else
               {
                   newNode.cost = optimalState.cost;
               }

               nextNodes.Add(newNode);
           }

           Node selectedNode = new Node(null, null);
           int compareValue = AI_Trun ? int.MinValue : int.MaxValue;
           foreach (Node node in nextNodes)
           {
               if (AI_Trun)
               {
                   if (compareValue > node.cost)
                   {
                       selectedNode = node;
                       compareValue = selectedNode.cost;
                   }
               }
               else
               {
                   if (compareValue < node.cost)
                   {
                       selectedNode = node;
                       compareValue = selectedNode.cost;
                   }
               }
           }
           return selectedNode;
       });
    }
    public class Node
    {
        public GameState state;
        public Node prevoiusNode;
        public int cost = 0; 
        public Node (GameState currentState , Node prevoiusNode)
        {
            this.state = currentState;
            this.prevoiusNode = prevoiusNode; 
        }
    } 
}
