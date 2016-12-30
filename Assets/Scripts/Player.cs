using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MovingObject {	//inherits from our abstract class MovingObject.

	public int wallDamage = 1;		//the damage that the player will apply to walls when he chops them.
	public int pointsPerFood = 10;		//number of points added to players score when they pick up food or soda
	public int pointsPerSoda = 20;
	public float restartLevelDelay = 1f;		
	public Text foodText;
	public AudioClip moveSound1;
	public AudioClip moveSound2;
	public AudioClip eatSound1;
	public AudioClip eatSound2;
	public AudioClip drinkSound1;
	public AudioClip drinkSound2;
	public AudioClip gameOverSound;

	private Animator animator;		//reference to animator component
	private int food;		//store players score during level before passing it to gamemanager when we change level.

	// Use this for initialization
	protected override void Start () {		//protected override because different than MovingObject's start function.

		animator = GetComponent<Animator> ();
		food = GameManager.instance.playerFoodPoints;		//setting food to the value from game manager. this is so we can store it in the gamemanager as we change levels.
		foodText.text = "Food: " + food;
		base.Start ();		//calling start gunction of MovingObject
	}

	private void OnDisable(){		//called when player gameObject is disabled. used to store value of food in the game manager when we change levels.
		GameManager.instance.playerFoodPoints = food;	
	}

	void Update(){
		if (!GameManager.instance.playersTurn)		//if its not the player's turn, return so no code is executed after this.
			return;		
		
		int horizontal = 0;		//variables to hold direction we are moving along the horizontal and vertical axis. 1 or -1
		int vertical = 0;

		horizontal = (int)(Input.GetAxisRaw ("Horizontal"));		//get input from input manager. Casting and storing as int from float.
		vertical = (int)(Input.GetAxisRaw ("Vertical"));

		if (horizontal != 0)		//if we are moving horizontal, set vertical to 0 to prevent diagonal movement.
			vertical = 0;

		if(horizontal != 0 || vertical != 0)		//if we are trying to move.
			AttemptMove<Wall>(horizontal, vertical);	//passing in wall because we are expecting the player to interact with it. This is T! for the player woo
	}
	protected override void AttemptMove <T>(int xDir, int yDir){	//takes a generic param T

		food--;	//subtract 1 food because the player is moving
		foodText.text = "Food: "+ food;
		base.AttemptMove<T> (xDir, yDir);		//call base class's attempt move function

		RaycastHit2D hit;		//allow us to reference the result of the linecast done in move.
		if(Move (xDir,yDir,out hit)){
			SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);
		}
			
		CheckIfGameOver ();		//since we moved, check if game obver
		GameManager.instance.playersTurn = false;		//set player's turn to false
	}

	private void OnTriggerEnter2D (Collider2D other){		//entered on trigger from the soda food and exit trigger colliders.

		if(other.tag == "Exit"){		//if we collided with exit, invoke restart and pass in a delay of 1 second. soo restart is called 1 seond after trigger.
			Invoke ("Restart", restartLevelDelay);
			enabled = false;	//level is now over.
		}
		else if(other.tag == "Food"){		//if we collided with food
			food += pointsPerFood;		//add points to food
			foodText.text = "+" + pointsPerFood + "Food: "+ food;
			SoundManager.instance.RandomizeSfx (eatSound1, eatSound2);
			other.gameObject.SetActive (false);		//get rid of food object 
		}
		else if(other.tag == "Soda"){		//if we collided with Soda
			food += pointsPerSoda;		//add points to food
			foodText.text = "+" + pointsPerSoda + "Food: "+ food;
			SoundManager.instance.RandomizeSfx (drinkSound1, drinkSound2);
			other.gameObject.SetActive (false);		//get rid of soda object
		}
		
	}

	protected override void OnCantMove <T>(T component){		//take an action when there is a wall and we are blocked by it.

		Wall hitWall = component as Wall;		//the component that was passed in as a paramter while casting it as a wall.
		hitWall.DamageWall(wallDamage);		//calling DamageWall function of wall we hit. passing in how much damage player will do to wall.
		animator.SetTrigger("playerChop");		//triggering animation for chop.
	}

	private void Restart(){		//reloading the level. Called when the player collides with the exit object.
		Application.LoadLevel(Application.loadedLevel);		//load las scene that was loaded which is main. Restarting our main scene because levels are being created procedurally.
	}

	public void LoseFood (int loss){
		animator.SetTrigger ("playerHit");		//trigger hit animation
		food -= loss;		//subtracting from food total
		foodText.text = "-" +  loss + " Food: " + food;
		CheckIfGameOver();	//checking if gameover
	}

	private void CheckIfGameOver(){
		if (food <= 0) {
			SoundManager.instance.PlaySingle (gameOverSound);
			SoundManager.instance.musicSource.Stop ();
			GameManager.instance.GameOver ();		//callingt the gameOVer function from the base class.
		}
	}
}
