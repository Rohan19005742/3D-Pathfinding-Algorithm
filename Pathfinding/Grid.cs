using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	public bool displayGridGizmos, GenerateAllNodes;
	public LayerMask UnWalkableLayerMask;
	public Vector3 GridWorldSize;
	public float NodeRadius;
	public int MaxSize;

	private Node[,,] grid;
	private float NodeSize;
	private int GridSizeX, GridSizeY, GridSizeZ;

	void Awake() {
		transform.position = new Vector3(GridWorldSize.x / 2, GridWorldSize.y / 2, GridWorldSize.z / 2);
		NodeSize = NodeRadius*2;
		GridSizeX = Mathf.RoundToInt(GridWorldSize.x/NodeSize);
		GridSizeY = Mathf.RoundToInt(GridWorldSize.y/NodeSize);
		GridSizeZ = Mathf.RoundToInt(GridWorldSize.z/NodeSize);
		MaxSize = GridSizeX * GridSizeY * GridSizeZ;
		if (GenerateAllNodes) {
			CreateFullGrid();
        }
        else {
			CreateGrid();
		}
	}

	private void CreateFullGrid() {
		grid = new Node[GridSizeX, GridSizeY, GridSizeZ];

		for (int x = 0, id = 0; x < GridSizeX; x ++) {
			for (int y = 0; y < GridSizeY; y ++) {
		    	for (int z = 0; z < GridSizeZ; z++) {
					Vector3 NodeWorldPosition = Vector3.right * (x * NodeSize + NodeRadius) + Vector3.up * (y * NodeSize + NodeRadius) + Vector3.forward * (z * NodeSize + NodeRadius);
					bool Walkable;
					if (y == 0) {
						Walkable = !(Physics.CheckSphere(NodeWorldPosition, NodeRadius - 0.01f, UnWalkableLayerMask));
					}
					else {
						if (Physics.CheckSphere(grid[x, y - 1, z].WorldPosition, NodeRadius - 0.01f, UnWalkableLayerMask) == true && Physics.CheckSphere(NodeWorldPosition, NodeRadius - 0.01f, UnWalkableLayerMask) == false) {
							Walkable = true;
						}
						else {
							Walkable = false;
						}
					}
					grid[x, y, z] = new Node(NodeWorldPosition, Walkable, x, y, z, id);
					id++;
				}
			}
		}
	}

	void CreateGrid() {
		grid = new Node[GridSizeX,GridSizeY,GridSizeZ];

		for (int x = 0, id = 0; x < GridSizeX; x ++) { // three for loops are used to create the grid as it is a 3D grid
			for (int y = 0; y < GridSizeY; y ++) {
				for (int z = 0; z < GridSizeZ; z++) {
					Vector3 NodeWorldPosition = Vector3.right * (x * NodeSize + NodeRadius) + Vector3.up * (y * NodeSize + NodeRadius) + Vector3.forward * (z * NodeSize + NodeRadius); // calculates the world position of the node given the x,y and z coordinates to the position of the grid it is going to be in
					bool Walkable;
					if (y == 0) { // creates all the nodes of the plane at height 0
						Walkable = !(Physics.CheckSphere(NodeWorldPosition, NodeRadius - 0.01f, UnWalkableLayerMask)); // sends a raycast and checks if there is an object in that position
						grid[x, y, z] = new Node(NodeWorldPosition, Walkable, x, y, z, id);
						id++;
					}
					else if((Physics.CheckSphere(NodeWorldPosition, NodeRadius - 0.01f, UnWalkableLayerMask))){ // only creates a node if it is not walkable
						grid[x, y, z] = new Node(NodeWorldPosition, false, x, y, z, id);
						id++;
					}
					else {
						if (grid[x, y - 1, z] != null) { // creates a walkable node above an unwalkable node only if the node above the unwalable node does not interfear with an object
							if (Physics.CheckSphere(grid[x, y - 1, z].WorldPosition, NodeRadius - 0.01f, UnWalkableLayerMask) == true && Physics.CheckSphere(NodeWorldPosition, NodeRadius - 0.01f, UnWalkableLayerMask) == false) {
								Walkable = true;
								grid[x, y, z] = new Node(NodeWorldPosition, Walkable, x, y, z, id);
								id++;
							}
						}
					}
				}
			}
		}
	}

	public bool CheckForObjects(Vector3 Position) {
		return Physics.CheckSphere(Position, NodeRadius - 0.01f, UnWalkableLayerMask); // sends a raycast and checks if there is an object in that position
	}

	public Node GetNode(Vector3 WorldPosition) {
		float X = (WorldPosition.x) / GridWorldSize.x;
		float Y = (WorldPosition.y) / GridWorldSize.y;
		float Z = (WorldPosition.z) / GridWorldSize.z;
		int x = Mathf.RoundToInt((GridSizeX - 1) * X);
		int y = Mathf.RoundToInt((GridSizeY - 1) * Y);
		int z = Mathf.RoundToInt((GridSizeZ - 1) * Z);
		return grid[x, y, z];
	}

	public bool CheckForNode(Vector3 Position) {
        try { Node node = GetNode(Position); }
        catch { return false; }
		return true;
    }

	static int GetDistance(Node Node1, Node Node2) {
		return Mathf.Abs(Node1.GridPositionX - Node2.GridPositionX) + Mathf.Abs(Node1.GridPositionZ - Node2.GridPositionZ);
	}

	public List<Node> GetNeighbours(Node node) {
		List<Node> neighbours = new List<Node>();

		for (int x = -1; x <= 1; x++) { // cycles though all the adjacent neighbours
			for (int y = -1; y <= 1; y++) {
				for (int z = -1; z <= 1; z++) {
					if (x == 0 && y == 0 && z == 0)
						continue;

					int X = node.GridPositionX + x;
					int Y = node.GridPositionY + y;
					int Z = node.GridPositionZ + z;

					if (X >= 0 && X < GridSizeX && Y >= 0 && Y < GridSizeY && Z >= 0 && Z < GridSizeZ) { // checks if the coordinates are out of bounds
						if (grid[X, Y, Z] != null) {
							neighbours.Add(grid[X, Y, Z]);
						}
					}
				}
			}
		}

		return neighbours;
	}

	public Node GetNearestWalkableNode(Node node) {

		for (int x = -1; x <= 1; x++) { // cycles though all the adjacent neighbours
			for (int y = -1; y <= 1; y++) {
				for (int z = -1; z <= 1; z++) {
					if (x == 0 && y == 0 && z == 0)
						continue;

					int X = node.GridPositionX + x;
					int Y = node.GridPositionY + y;
					int Z = node.GridPositionZ + z;

					if (X >= 0 && X < GridSizeX && Y >= 0 && Y < GridSizeY && Z >= 0 && Z < GridSizeZ) { // checks if the coordinates are out of bounds
						if (grid[X, Y, Z] != null && grid[X, Y, Z].Walkable == true) { // checks if the node exists and is walkable
							return grid[X, Y, Z];
						}
					}
				}
			}
		}
		return null;
	}
	
	
	void OnDrawGizmos() {
		Gizmos.DrawWireCube(transform.position,new Vector3(GridWorldSize.x,GridWorldSize.y,GridWorldSize.z));
		if (grid != null && displayGridGizmos) {
			foreach (Node n in grid) {
				if (n != null) {
					Gizmos.color = (n.Walkable) ? Color.white : Color.red;
					Gizmos.DrawCube(n.WorldPosition, Vector3.one * (NodeSize - .1f));
				}
			}
		}
	}
}

public class Node : IHash<Node>{ // uses interface with hashtable

	public int GridPositionX, GridPositionY, GridPositionZ;
	public Vector3 WorldPosition;
	public bool Walkable;
	public int gCost;
	public int hCost;
	public Node Parent;
	public int HeapIndex;
	private int hashIndex;

	public Node(Vector3 WorldPosition, bool Walkable, int GridPositionX, int GridPositionY, int GridPositionZ, int hashIndex) {
		this.WorldPosition = WorldPosition;
		this.Walkable = Walkable;
		this.GridPositionX = GridPositionX;
		this.GridPositionY = GridPositionY;
		this.GridPositionZ = GridPositionZ;
		this.hashIndex = hashIndex;
	}

	public int fCost {
		get { return gCost + hCost; }
	}

	public int HashIndex {
        get { return hashIndex; }
        set { hashIndex = value; }
    }
}