using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public Maze mazePrefab;
	private Maze mazeInstance;

	private void BeginGame () {
		mazeInstance = Instantiate(mazePrefab) as Maze;
		mazeInstance.Generate();
	}

	private void RestartGame () {
		Destroy(mazeInstance.gameObject);
		BeginGame();
	}
	
	private void Start () {
		BeginGame();
	}
	
	private void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			RestartGame();
		}
		if (Input.GetKeyDown(KeyCode.W)) {
			mazeInstance.Deteriorate(ref mazeInstance);
		}
	}
}
