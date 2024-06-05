using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Maze : MonoBehaviour {
	public int sizeX,sizeZ;
	public CellCluster cellPrefab;
	public SquareWalls SquareWall;
	public Room1 room;
	private CellCluster[,] cellclusters;
	
	public float generationStepDelay;

	public void Generate () {
		float neighbortriname;
		float wallname;
		int neighbortri;
		float triname;
		sizeX = 15;
		sizeZ = 15;
		cellclusters = new CellCluster[sizeX, sizeZ];
		
		
		// generate cells
		for (int x = 0; x < sizeX; x++) {
			for (int z = 0; z < sizeZ; z++) {
				CreateCell(x, z);
			}
		}
		for (int x = 0; x < sizeX; x++) {
			for (int z = 0; z < sizeZ; z++) {
				for (int tri = 0; tri < 4; tri++) {
					// find target cell
					triname = Mathf.Pow(2, tri);
            		neighbortri = tri ^ 2;
					neighbortriname = Mathf.Pow(2, neighbortri);
            		cellclusters[x, z].transform.Find(""+triname).GetComponent<TriangleCellFloor>().neighbors.Add(cellclusters[x, z].transform.Find(""+neighbortriname).GetComponent<TriangleCellFloor>()); // prints out value with xy flag flipped
            		neighbortri = neighbortri ^ 1;
					neighbortriname = Mathf.Pow(2, neighbortri);
            		cellclusters[x, z].transform.Find(""+triname).GetComponent<TriangleCellFloor>().neighbors.Add(cellclusters[x, z].transform.Find(""+neighbortriname).GetComponent<TriangleCellFloor>()); // prints out xy and first/last flags flipped
            		neighbortriname = Mathf.Pow(2, tri ^ 1);
					try {
						if ((tri & (1U << 1)) != 0) // check xz flag
            			{
							if ((tri & (1U << 0)) == 0) {
								cellclusters[x, z].transform.Find(""+triname).GetComponent<TriangleCellFloor>().neighbors.Add(cellclusters[x + 1, z].transform.Find(""+neighbortriname).GetComponent<TriangleCellFloor>());
							}
							else {
								cellclusters[x, z].transform.Find(""+triname).GetComponent<TriangleCellFloor>().neighbors.Add(cellclusters[x - 1, z].transform.Find(""+neighbortriname).GetComponent<TriangleCellFloor>());
							}
            			}
            			else // triangle is on x axis
            			{
                			if ((tri & (1U << 0)) == 0) {
								cellclusters[x, z].transform.Find(""+triname).GetComponent<TriangleCellFloor>().neighbors.Add(cellclusters[x, z + 1].transform.Find(""+neighbortriname).GetComponent<TriangleCellFloor>());
							}
							else {
								cellclusters[x, z].transform.Find(""+triname).GetComponent<TriangleCellFloor>().neighbors.Add(cellclusters[x, z - 1].transform.Find(""+neighbortriname).GetComponent<TriangleCellFloor>());
							}
            			}
					} catch (Exception) {

					}
					// WIP
        			List<TriangleCellFloor> temp = new List<TriangleCellFloor>();
        			int j = cellclusters[x, z].transform.Find(""+triname).GetComponent<TriangleCellFloor>().neighbors.Count;
        			for (int i = 0; i < j; i++)
        			{
            			temp.Add(cellclusters[x, z].transform.Find(""+triname).GetComponent<TriangleCellFloor>().neighbors[UnityEngine.Random.Range(0, cellclusters[x, z].transform.Find(""+triname).GetComponent<TriangleCellFloor>().neighbors.Count)]);
            			cellclusters[x, z].transform.Find(""+triname).GetComponent<TriangleCellFloor>().neighbors.Remove(temp.Last());
        			}
        			cellclusters[x, z].transform.Find(""+triname).GetComponent<TriangleCellFloor>().neighbors = temp;
				}
			}
		}
		
		for(int i = 0; i < sizeX; i++){
			SquareWalls wall = Instantiate(SquareWall) as SquareWalls;
			wall.x = sizeX;
			wall.z = i;
			wall.transform.localPosition = new Vector3(wall.x - sizeX * 0.5f + 0.5f, 0f, wall.z - sizeZ * 0.5f + 0.5f);
			Destroy(wall.transform.Find("3").gameObject);
		}
		for(int i = 0; i < sizeZ; i++){
			SquareWalls wall = Instantiate(SquareWall) as SquareWalls;
			wall.x = i;
			wall.z = sizeZ;
			wall.transform.localPosition = new Vector3(wall.x - sizeX * 0.5f + 0.5f, 0f, wall.z - sizeZ * 0.5f + 0.5f);
			Destroy(wall.transform.Find("12").gameObject);
		}
		
		// carve maze
		Stack<TriangleCellFloor> work = new Stack<TriangleCellFloor> ();
		List<TriangleCellFloor> visited = new List<TriangleCellFloor> ();
		List<TriangleCellFloor> reserved = new List<TriangleCellFloor> ();

		TriangleCellFloor start = cellclusters[0, 0].transform.Find("2").GetComponent<TriangleCellFloor>();
		TriangleCellFloor end = cellclusters[sizeX - 1, sizeZ - 1].transform.Find("1").GetComponent<TriangleCellFloor>();

		work.Push (start);
		visited.Add (start);
		
		// reserve cells
		//for (int i = 0; i < sizeX - 1; i++)
        //{
			for (int j = 0; j < 4; j++)
			{
				reserved.Add (cellclusters[0, 7].transform.Find("" + Mathf.Pow(2, j)).GetComponent<TriangleCellFloor>());
				reserved.Add (cellclusters[1, 7].transform.Find("" + Mathf.Pow(2, j)).GetComponent<TriangleCellFloor>());
				reserved.Add (cellclusters[0, 8].transform.Find("" + Mathf.Pow(2, j)).GetComponent<TriangleCellFloor>());
				reserved.Add (cellclusters[1, 8].transform.Find("" + Mathf.Pow(2, j)).GetComponent<TriangleCellFloor>());
			}
        //}

		//generate maze
		while(work.Count > 0)
		{
			TriangleCellFloor current = work.Pop ();
			if (current == end) {
				List<TriangleCellFloor> result = current.history;
				// path from start to end node
				foreach(var res in result)
				{
    				Debug.Log(res.transform.parent.GetComponent<CellCluster>().x + ", " + res.transform.parent.GetComponent<CellCluster>().z + " " + res);
				}
			} else {
				for(int i = 0; i < current.neighbors.Count; i++)
				{
					TriangleCellFloor currentChild = current.neighbors [i];
					if(!visited.Contains(currentChild) && !reserved.Contains(currentChild)) {
						work.Push (currentChild);
						visited.Add (currentChild);
						currentChild.history.AddRange (current.history);
						currentChild.history.Add (current);
					}
				}	
			}
		}
		for (int x = 0; x < sizeX; x++)
		{
			for (int z = 0; z < sizeZ; z++)
			{
				for (int tri = 0; tri < 4; tri++)
				{
					triname = Mathf.Pow(2, tri);
					TriangleCellFloor neighbor1 = cellclusters[x, z].transform.Find("" + triname).GetComponent<TriangleCellFloor>();
					if (neighbor1 != start && !reserved.Contains(neighbor1))
					{
						TriangleCellFloor neighbor2 = cellclusters[x, z].transform.Find("" + triname).GetComponent<TriangleCellFloor>().history.Last();
						wallname = float.Parse(neighbor1.name) + float.Parse(neighbor2.name);
						if (neighbor2.transform.parent.GetComponent<CellCluster>().x == x && neighbor2.transform.parent.GetComponent<CellCluster>().z == z) { //clean up later
							Destroy(cellclusters[x, z].transform.Find("" + wallname).gameObject);
						} else if (Mathf.Log(float.Parse(neighbor1.name), 2) % 2 != 0) {
							Destroy(cellclusters[x, z].transform.Find("" + wallname).gameObject);
						} else {
							Destroy(cellclusters[neighbor2.transform.parent.GetComponent<CellCluster>().x, neighbor2.transform.parent.GetComponent<CellCluster>().z].transform.Find("" + wallname).gameObject);
						} //greater one holds walls
					}
				}
			}
		}
		
		// test moving before generation??
		for (int x = 0; x < sizeX; x++)
		{
			for (int z = 0; z < sizeZ; z++)
			{
				for (int tri = 0; tri < 4; tri++)
				{
					if (reserved.Contains(cellclusters[x, z].transform.Find("" + Mathf.Pow(2, tri)).GetComponent<TriangleCellFloor>()))
					{
						Destroy(cellclusters[x, z].transform.Find("" + Mathf.Pow(2, tri)).GetComponent<TriangleCellFloor>().gameObject);
					}
				}
			}
		}
	}
	
	public void Deteriorate (ref Maze maze) {
		Destroy(cellclusters[3, 3].gameObject);
	}

	private void CreateCell (int x, int z) {
		CellCluster newCell = Instantiate(cellPrefab) as CellCluster;
		
		newCell.x = x;
		newCell.z = z;
		cellclusters[x, z] = newCell;
		newCell.name = "Maze Cell " + x + ", " + z;
		newCell.transform.localPosition = new Vector3(x - sizeX * 0.5f + 0.5f, 0f, z - sizeZ * 0.5f + 0.5f);
	}
}