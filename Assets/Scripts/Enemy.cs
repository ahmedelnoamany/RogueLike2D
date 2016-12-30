using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject {		//inherits from abstract class MovingObject

	public int playerDamage;		//how much damage done to player
	public AudioClip enemyAttack1;
	public AudioClip enemyAttack2;

	private Animator animator;		//reference to animator controller
	private Transform target;		//players position so we can tell the enemy where to move.
	private bool skipMove;		//cause enemy to move every other turn


	protected override void Start () {		//overriding base class
		GameManager.instance.AddEnemyToList(this);	//registering this enemy with the GameMAnager
		animator = GetComponent<Animator>();	//reference to animator component.
		target = GameObject.FindGameObjectWithTag ("Player").transform;		//store players position in target.
		base.Start();		//call base class's Start method.
	}

	protected override void AttemptMove <T>(int xDir, int yDir){

		if (skipMove) {		//if we are meant to be skipping a move
			skipMove = false;	//next time it wont skip
			return;		//return to stop executing.
		}
		base.AttemptMove <T> (xDir, yDir);	//attempt a move from base

		skipMove = true;		//enemy moved so he wont move next turn.
	}

	public void MoveEnemy(){		//called by GameManger when its time to move the enemies

		int xDir = 0;		//for Enemy movement -1 or 1 for movment
		int yDir = 0;

		if (Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon) {		//check position of target against current position of our own transform. if difference of target x position - our x position < small value 
			//is our player and enemy in the same column
			yDir = target.position.y > transform.position.y ? 1 : -1;		//if y coord of our target is greater than ours, move up. if not move down. trying to get closer to player -1 = down
		} else
			xDir = target.position.x > transform.position.x ? 1 : -1;		//if x of target is greater than ours, move right (1) else move left (-1). trying to get closer to player.
		AttemptMove <Player> (xDir, yDir);	//attempt a move with player instead of T  since we might collide with player
	}

	protected override void OnCantMove <T>(T component){
		
		Player hitPlayer = component as Player;
		animator.SetTrigger ("enemyAttack");		//trigger animation
		SoundManager.instance.RandomizeSfx(enemyAttack1,enemyAttack2);
		hitPlayer.LoseFood (playerDamage);		//calls players lose food function when they collide and player looses playerDamage worth of food.
	}
}
