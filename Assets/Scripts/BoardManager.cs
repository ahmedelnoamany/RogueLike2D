using System.Collections;
using System;																			//allows us to use serializable. allows us to modify how variables will appear in the inspection and also to use or hide them.
using System.Collections.Generic;														//So we can use lists.
using UnityEngine;
using Random = UnityEngine.Random;														//have to specify this because there is a class called random in both the system and unityEngine namespaces.

public class BoardManager : MonoBehaviour 
{
	[Serializable]																		//let's you embed a class with sub properties in the inspector.
	public class Count
	{
		public int minimum;
		public int maximum;

		public Count (int min, int max)													//assignment constructor so we can set min and max when we declare a new count.
		{
			minimum = min;
			maximum = max;
		}
	}

	//Var Declarations....
	public int columns = 8;																//dimensions for gameboard currently 8 x 8
	public int rows = 8;

	public Count wallCount = new Count(5,9);											//random range for how many walls we want to spawn per level. Min of 5, max of 9
	public Count foodCount = new Count(1,5);											//same for food

	public GameObject exit;																//hold exit prefab. single since only 1 exit.

	//Will fill each of these arrays in the inspector. With our prefabs that we made!
	public GameObject[] floorTiles;
	public GameObject[] wallTiles;
	public GameObject[] foodTiles;
	public GameObject[] enemyTiles;
	public GameObject[] outerWallTiles;

	private Transform boardHolder;														//Use to keep hierarchy clean. will child all the gameobjects to boardholder so we can collapse them and keep it clean.
	private List <Vector3> gridPositions = new List<Vector3>();							//use to track all different possible positions on gameboard and to keep track of object spawning in those positions or not.

	//function that clears gridpositions and reinitiallizes them.
	void InitializeList()
	{
		gridPositions.Clear();															//first clearing gridpositions list.

		for (int x = 1; x < columns - 1; x++) {											//filling list with each position on our gameboard as a vector3. xaxis. -1 so we can always have a way through.
			for(int y = 1; y<rows - 1; y++){											//y axis
				gridPositions.Add (new Vector3 (x, y, 0f));								//adding a new vector 3 with x and y values in gridpositions list.
			}
		}
	}

	//sets up outer wall and floor of gameboard.
	void BoardSetup(){
		boardHolder = new GameObject ("Board").transform;								//setting boardHolder to the transform of the new gameobject named board.

		for (int x = -1; x < columns + 1; x++) {										//laying out floor and outer wall tiles. goes to +1 because building an edge.
			for (int y = -1; y < rows + 1; y++) {
				GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];		//choosing floor tile at random. then we will clone the original object (instantiate).
				if(x == -1 || x == columns || y == -1 || y == rows)						//if the index is an outer wall, we will instantiate from outerWallTiles[] instead.
					toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];

				GameObject instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;		//instantiating. to instantiate at a new vector 3 based on current x,y in loop. 0 for z because 2D. Quaternion means instantiated with no rotation. casted as a gameObject.
				instance.transform.SetParent(boardHolder);								//setting parent of newly instantiated gameObject to boardHolder. Putting it in to keep hierarchy clean.
			}
		}
	}

	//generates a random position from the available gridPositions and returns it.
	Vector3 RandomPosition(){
		int randomIndex = Random.Range (0, gridPositions.Count);						//generates a random number within 0 and gridpositiobs.
		Vector3 randomPosition = gridPositions[randomIndex];							//randomPosition is the value at the randomIndexes gridPosition
		gridPositions.RemoveAt(randomIndex);											//removing that gridposition to make sure you dont spawn 2 items at the same position.
		return randomPosition;
	}

	void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum){		//lays out a random array from tile array. min and max used for the objectCount
		int objectCount = Random.Range(minimum, maximum+1);								//generates a random val between the min and max for the number of objects we will have on the board.

		for (int i = 0; i < objectCount; i++) {
			Vector3 randomPosition = RandomPosition ();									//getting a random position on gameboard.
			GameObject tileChoice = tileArray [Random.Range (0, tileArray.Length)];		//getting a random tile from the tile array.
			Instantiate(tileChoice, randomPosition, Quaternion.identity);				//instantiating our random tile at the random position with no rotation.
		}
	}

	public void SetupScene(int level){													//called by gameMAnager when it is time to set up the board.

		BoardSetup ();
		InitializeList ();
		LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);			//laying out wall tiles
		LayoutObjectAtRandom (foodTiles, foodCount.minimum, foodCount.maximum);			//laying out food tiles.
		int enemyCount = (int)Mathf.Log(level, 2f);										//generates number of enemies based on the level. 1 enemy at level 2, 2 at 4, 3 at 8....Ascending difficulty. Mathf.log returns float
		LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);						//laying out enemies.
		Instantiate(exit, new Vector3(columns-1, rows-1, 0F), Quaternion.identity);		//instantiating exit. always in same place so -1 and -1 to be at top right of board. and always the same object.
	}


}
