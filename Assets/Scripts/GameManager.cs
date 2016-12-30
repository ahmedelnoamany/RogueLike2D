using System.Collections;
using System.Collections.Generic;		//to use lists
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public float levelStartDelay = 2f;
	public float turnDelay = .1f;		//how long GameManager waits between turns.
	public static GameManager instance = null;																//public means accessible outside class.static belongs to this class instead of instance. now we can open from any scipt
	public BoardManager boardScript;
	public int playerFoodPoints = 100;			//food points for the player script.
	[HideInInspector] public bool playersTurn = true;		//hidden from inspector.

	private Text levelText;		//display level
	private GameObject levelImage;		//refernce to the level image so we can activate and deactivate.
	private int level = 1;
	private List<Enemy> enemies;		//keep track of enemies in a list.
	private bool enemiesMoving;
	private bool doingSetup;
	// Use this for initialization
	void Awake () {																							//called when the scipt instance is being loaded. called only once in the lifetime of the instance. every time you load a scene.
		if (instance == null)																				//can only be 1. if its null means 1st, set it to this. if its not equal to this then wth destroy it.
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
		DontDestroyOnLoad (gameObject);																		//when you load a new scene, dont destroy gameObjects .
		enemies = new List<Enemy>();		//enemies is a new list of type Enemy
		boardScript = GetComponent<BoardManager>();															//getting a reference to out board manager script.
		InitGame();																							//cakks function to initialize the game.
	}

	//This is called each time a scene is loaded.
	void OnLevelWasLoaded(int index)
	{
		//Add one to our level number.
		level++;
		//Call InitGame to initialize our level.
		InitGame();
	}

	void InitGame(){
		doingSetup = true;
		levelImage = GameObject.Find ("LevelImage");
		levelText = GameObject.Find ("LevelText").GetComponent<Text> ();
		levelText.text = "Day " + level;
		levelImage.SetActive (true);
		Invoke ("HideLevelImage", levelStartDelay);

		enemies.Clear ();		//clear enemies from last lavel
		boardScript.SetupScene (level);																		//calls setupScene passes level so we can spawn enemies.

	}

	private void HideLevelImage(){
		levelImage.SetActive (false);
		doingSetup = false;
	}

	public void GameOver(){			//disable the gamemanager.
		levelText.text = "After " + level + " days, you starved.";
		levelImage.SetActive (true);
		enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (playersTurn || enemiesMoving || doingSetup)		//if its the players turn or an enemy is moving, skip
			return;
		//otherwise
		StartCoroutine(MoveEnemies());	//move the enemies
	}

	public void AddEnemyToList(Enemy script){		//enemies register with GameManager so GameManger can tell them what to do.
		enemies.Add(script);		//add enemies script to the list
	}

	IEnumerator MoveEnemies(){		//move enemies 1 at a time
		enemiesMoving = true;	
		yield return new WaitForSeconds (turnDelay);		//yield and wait turnDelay 0.1 seconds.
		if(enemies.Count == 0){		//if no enemies spawned
			yield return new WaitForSeconds (turnDelay);	//add an additional yield to make player wait even though there are no enemies to wait for.
		}

		for(int i = 0; i<enemies.Count; i++){
			enemies [i].MoveEnemy ();		//move enemy i
			yield return new WaitForSeconds (enemies[i].moveTime);	//wait the enemies move time before calling the next 1.
		}

		playersTurn = true;		//players turn boiii
		enemiesMoving = false;
	}
}
