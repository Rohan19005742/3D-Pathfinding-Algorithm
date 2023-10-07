using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Pathfinding : MonoBehaviour {

	PathfindingManager Manager; // script that I made
	private Grid grid; // script that i made
	private int MaxCycles = 1000;
	
	void Awake() {
		grid = GetComponent<Grid>();
		Manager = GetComponent<PathfindingManager>();
	}
	
	public void FindPath(Vector3 startPos, Vector3 targetPos) { // the A* pathfinding algorithm

		Vector3[] Path = new Vector3[0];
		bool PathFound = false;
		
		Node startNode = grid.GetNode(startPos);
		Node targetNode = grid.GetNode(targetPos);

		if(startNode == null || targetNode == null) {
			Manager.FinishedProcessingPath(Path, PathFound);
			return;
        }

		if (startNode.Walkable == false) {
			startNode = grid.GetNearestWalkableNode(startNode);
        }

		if (targetNode.Walkable == false) {
			targetNode = grid.GetNearestWalkableNode(targetNode);
		}

		if (startNode.Walkable && targetNode.Walkable) {
			MinHeap openSet = new MinHeap(grid.MaxSize);
			HashTable<Node> closedSet = new HashTable<Node>(grid.MaxSize);
			openSet.Add(startNode);
			int CycleCount = 0;
			
			while (openSet.Size > 0) {
				CycleCount++;
				if (CycleCount > MaxCycles) { // finishes processing the path if the cycle count is reach, i have done this to make the program robust as i stop the while loop going on forever if the path is never found.
					Manager.FinishedProcessingPath(Path, PathFound);
				}
				Node currentNode = openSet.GetMin();
				closedSet.Add(currentNode);
				
				if (currentNode == targetNode) {
					PathFound = true;
					break;
				}
				
				foreach (Node neighbour in grid.GetNeighbours(currentNode)) { // cycles through each neighbour and and the neighbour that is closest to the endpoint is its parent and added to the openlist
					if (!neighbour.Walkable || closedSet.CheckForItem(neighbour)) {
						continue;
					}
					
					int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
					if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = GetDistance(neighbour, targetNode);
						neighbour.Parent = currentNode;
						
						if (!openSet.Contains(neighbour))
							openSet.Add(neighbour);
					}
				}
			}
		}
		if (PathFound) { Path = RetracePath(startNode,targetNode); }
		Manager.FinishedProcessingPath(Path, PathFound); // path is sent back, where it is empty or not
	}
	
	private Vector3[] RetracePath(Node StartNode, Node EndNode) { // starts with the endnode and stores the position of the parents and repeats this until the parent node is the start node
		List<Vector3> Path = new List<Vector3>();
		Node CurrentNode = EndNode;

		while (CurrentNode != StartNode) {
			Path.Add(CurrentNode.WorldPosition);
			CurrentNode = CurrentNode.Parent;
		}
		Vector3[] Waypoints = Path.ToArray();
		Array.Reverse(Waypoints);
		return Waypoints;
	}
	
	private int GetDistance(Node Node1, Node Node2) { // calculates the distance between 2 nodes
		return Mathf.Abs(Node1.GridPositionX - Node2.GridPositionX) + Mathf.Abs(Node1.GridPositionZ - Node2.GridPositionZ);
	}
}
