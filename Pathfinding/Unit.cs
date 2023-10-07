using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour { // add to any object that will neeed to path find

	[HideInInspector]
	public float Speed;
	[HideInInspector]
	public bool IsMoving;
	[HideInInspector]
	public float ReactionTime;
	[HideInInspector]
	public Transform Target;

	private Vector3[] Path;
	private int PathIndex;
	private PathfindingManager Manager; // script that i made
	private float TurningSpeed = 5f;

	void Awake() {
		Manager = GameObject.Find("A*").GetComponent<PathfindingManager>();
	}

	void Start() {
		StartCoroutine("UpdatePath");
    }

	public void Stop() {
		StopCoroutine("FollowPath");
		IsMoving = false;
    }

	public void StopChasing() {
		StopCoroutine("UpdatePath");
    }

	public void OnPathFound(Vector3[] Path, bool PathFound) {
		if (PathFound && Path.Length > 0) {
			this.Path = Path;
			PathIndex = 0;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}

	private void FaceDirectionOfMovement() { // calculates and sets the rotation of this enemy by using its path
		Quaternion Rotation = Quaternion.LookRotation(Path[PathIndex] - transform.position);
		transform.rotation = Quaternion.Lerp(transform.rotation, Rotation, Time.deltaTime * TurningSpeed);
	}

	IEnumerator UpdatePath() {
		yield return new WaitForSeconds(ReactionTime);
		Manager.RequestPath(transform.position, Target.position, OnPathFound);
		Vector3 TargetsLastPosition = Target.position;

		while (true) {
			yield return new WaitForSeconds(ReactionTime); // waits for reaction time then tries to update the path
			if (Target != null) {
				if ((Target.position - TargetsLastPosition).sqrMagnitude > 0.1f) { // checks if the player has move a certain distance and only then requests a new path
					Manager.RequestPath(transform.position, Target.position, OnPathFound);
					TargetsLastPosition = Target.position;
				}
			}
		}
	}

	private IEnumerator FollowPath() {
        Vector3 CurrentWaypoint = Path[0];
		while (true) {
			IsMoving = true;
			if (transform.position == CurrentWaypoint) {
				PathIndex ++;
				if (PathIndex >= Path.Length) {
					IsMoving = false;
					yield break;
				}
				CurrentWaypoint = Path[PathIndex];
			}
			FaceDirectionOfMovement();
			transform.position = Vector3.MoveTowards(transform.position, CurrentWaypoint, Speed * Time.deltaTime);
			yield return null;
		}
	}

	public void OnDrawGizmos() {
		if (Path != null) {
			for (int i = PathIndex; i < Path.Length; i ++) {
				Gizmos.color = Color.black;
				Gizmos.DrawCube(Path[i], Vector3.one);

				if (i == PathIndex) {
					Gizmos.DrawLine(transform.position, Path[i]);
				}
				else {
					Gizmos.DrawLine(Path[i-1],Path[i]);
				}
			}
		}
	}
}
