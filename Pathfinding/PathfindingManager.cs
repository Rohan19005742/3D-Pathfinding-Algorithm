using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
	private Queue<PathRequest> Queue; // using inbuilt queue
	private PathRequest CurrentRequest;
	private Pathfinding pathfinding; // script that i made
	private bool Processing;

	void Awake() {
		pathfinding = GetComponent<Pathfinding>();
		Queue = new Queue<PathRequest>();
	}

	public void RequestPath(Vector3 Start, Vector3 End, Action<Vector3[], bool> CallbackFunction) { // higher order function - takes a function as an arguement
		PathRequest NewRequest = new PathRequest(Start, End, CallbackFunction);
		Queue.Enqueue(NewRequest);
		ProcessNext();
	}

	void ProcessNext() {
		if (!Processing && Queue.Count > 0) {
			CurrentRequest = Queue.Dequeue();
			Processing = true;
			pathfinding.FindPath(CurrentRequest.Start, CurrentRequest.End);
		}
	}

	public void FinishedProcessingPath(Vector3[] Path, bool PathFound) {
		CurrentRequest.CallbackFunction(Path, PathFound); // gives the path to the unit which requested it
		Processing = false;
		ProcessNext();
	}

	private struct PathRequest { // i made this to made things look easier, used to store the the start and end position and the function to call when path is found or not found
		public Vector3 Start;
		public Vector3 End;
		public Action<Vector3[], bool> CallbackFunction;

		public PathRequest(Vector3 Start, Vector3 End, Action<Vector3[], bool> CallbackFunction) {
			this.Start = Start;
			this.End = End;
			this.CallbackFunction = CallbackFunction;
		}

	}
}
