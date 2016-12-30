using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

	public Sprite dmgSprite;		//Sprite to be displayed once the player has hit the wall.
	public int hp = 4; 		//hit points
	public AudioClip chopSound1;
	public AudioClip chopSound2;

	private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Awake () {

		spriteRenderer = GetComponent<SpriteRenderer> ();		//getting a componenet reference to our spriteRender
	}
	
	public void DamageWall(int loss){
		SoundManager.instance.RandomizeSfx (chopSound1, chopSound2);
		spriteRenderer.sprite = dmgSprite;		//set the sprite of our spriteRender to our damage sprite
		hp -= loss;		//wall takes damage, subtracting from hp.

		if (hp <= 0)			//if hp at 0 then disable game object
			gameObject.SetActive (false);
		
	}
}
