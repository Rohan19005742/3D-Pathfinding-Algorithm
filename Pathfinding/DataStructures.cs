using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HashTable<Item> where Item : IHash<Item> { // using interface so that anything with a hashindex can use this
    private Item[] Table;
    private int Size;
    private int SpaceUsed;

    public HashTable(int Size) {
        this.Size = Size;
        Table = new Item[Size];
    }

    private int GetKey(Item item) {
        return item.HashIndex % Size;
    }

    private bool CheckForSpace(int key) {
		if (Table[key] == null ) { return true; }
		else { return false; }
    }

    public void Remove(Item item) {
        int key = GetKey(item);
		SpaceUsed -= 1;
    }

    public void Add(Item item) {
        int key = GetKey(item);
		if (CheckForSpace(key)) { Table[key] = item; SpaceUsed += 1; }
    }

	public Item GetItem(int HashIndex) {
		int key = HashIndex % Size;
		return Table[key];
    }

    public bool CheckForItem(Item item) {
        int key = GetKey(item);
        if (Equals(Table[key], item)){ return true; }
		return false;
    }

	public static int GetHashIndexForString(string String) {
		int HashIndex = 0;
		foreach (char character in String) {HashIndex += System.Convert.ToInt32(character);}
		return HashIndex;
	}
}

public interface IHash<Item> { // interface for hash table
	int HashIndex { get; set; }
}

public class MinHeap { // used for optimising A* pathfinding as it can quickly sort the nodes by thier fcosts

	private Node[] Heap;
	private int size;

	public MinHeap(int Size) {
		Heap = new Node[Size];
	}

	public void Add(Node node) {
		node.HeapIndex = size;
		Heap[size] = node;
		SortUp(node);
		size++;
	}

	public Node GetMin() {
		Node Min = Heap[0];
		size--;
		Heap[0] = Heap[size];
		Heap[0].HeapIndex = 0;
		SortDown(Heap[0]);
		return Min;
	}

	void SortDown(Node node) {
		while (true) {
			int LeftChildIndex = node.HeapIndex * 2 + 1;
			int RightChildIndex = node.HeapIndex * 2 + 2;
			int SwapNodesIndex = 0;

			if (LeftChildIndex < size) {
				SwapNodesIndex = LeftChildIndex;

				if (RightChildIndex < size) {
					if (Heap[LeftChildIndex].fCost > Heap[RightChildIndex].fCost || (Heap[LeftChildIndex].fCost == Heap[RightChildIndex].fCost && Heap[LeftChildIndex].hCost > Heap[RightChildIndex].hCost) ) {
						SwapNodesIndex = RightChildIndex;
					}
				}

				if (node.fCost > Heap[SwapNodesIndex].fCost || (node.fCost == Heap[SwapNodesIndex].fCost && node.hCost > Heap[SwapNodesIndex].hCost)) {
					SwapNodes(node, Heap[SwapNodesIndex]);
				}
				else {
					return;
				}

			}
			else {
				return;
			}

		}
	}

	void SortUp(Node node) {
		int ParentIndex = (node.HeapIndex - 1) / 2;

		while (true) {
			Node ParentNode = Heap[ParentIndex];
			if (node.fCost < Heap[ParentIndex].fCost || (node.fCost == Heap[ParentIndex].fCost && node.hCost < Heap[ParentIndex].hCost)) {
				SwapNodes(node, ParentNode);
			}
			else {
				break;
			}

			ParentIndex = (node.HeapIndex - 1) / 2;
		}
	}

	public int Size {
		get {
			return size;
		}
	}

	public bool Contains(Node node) {
		return Equals(Heap[node.HeapIndex], node);
	}

	private void SwapNodes(Node node1, Node node2) {
		Heap[node1.HeapIndex] = node2;
		Heap[node2.HeapIndex] = node1;
		int Node1Index = node1.HeapIndex;
		int Node2Index = node2.HeapIndex;
		node1.HeapIndex = Node2Index;
		node2.HeapIndex = Node1Index;
	}
}

public static class MergeSort { // Merge sort used to sort the leaderboard scores

	public static List<ScoreClass> Sort(List<ScoreClass> Items) {
		if (Items.Count == 1) {
			return Items;
		}

		List<ScoreClass> Half_1 = new List<ScoreClass>();
		List<ScoreClass> Half_2 = new List<ScoreClass>();

		for (int x = 0; x < Items.Count; x++) { // splits the list into 2
			if (x >= Items.Count / 2) {
				Half_2.Add(Items[x]);
			}
			else {
				Half_1.Add(Items[x]);
			}
		}

		// recursively splits until there is one element in the list then it calls the merge function
		Half_1 = Sort(Half_1);  
		Half_2 = Sort(Half_2);

		return Merge(Half_1, Half_2); // the recursive function unwinds and returns a list with the elements in order.
	}

	private static List<ScoreClass> Merge(List<ScoreClass> List1, List<ScoreClass> List2) {
		List<ScoreClass> SortedList = new List<ScoreClass>();

		while (List1.Count > 0 && List2.Count > 0) { // check which number is bigger and stores it accordingly in the sorlist list local variable
			if (List1[0].Number > List2[0].Number) {
				SortedList.Add(List2[0]);
				List2.RemoveAt(0);
			}
			else {
				SortedList.Add(List1[0]);
				List1.RemoveAt(0);
			}
		}

		while (List1.Count > 0) {
			SortedList.Add(List1[0]);
			List1.RemoveAt(0);
		}

		while (List2.Count > 0) {
			SortedList.Add(List2[0]);
			List2.RemoveAt(0);
		}

		return SortedList;
	}
}