using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour {							//abstract so we can implement in derived class

	public float moveTime = 0.1f; //time it takes to move object in seconds.
	public LayerMask blockingLayer;		//where we will check for collisions. on this layer. Walls, player, enemies are on this layer.

	private BoxCollider2D boxCollider;	//
	private Rigidbody2D rb2D;			//storing a component reference to the rigidbody 2D component of the unit we are moving.
	private float inverseMoveTime;		//make movement calculatiions more efficient.

	// Use this for initialization
	protected virtual void Start () {			//protected virtual so it can be overwriten by the inheriting classes.

		boxCollider = GetComponent<BoxCollider2D> (); //getting reference to objects box collider 2D.
		rb2D = GetComponent<Rigidbody2D>();		//getting reference to objects rigidbody 2D.
		inverseMoveTime = 1f/ moveTime;			// inverse of move time so we can use multiplication later on instead of divison. This is more efficient computationally.
	}

	protected bool Move(int xDir, int yDir, out RaycastHit2D hit){		//out causes arguments to be pass by reference.

		Vector2 start = transform.position;			//casting it to Vector2, getting rid of the z position.
		Vector2 end = start + new Vector2 (xDir, yDir);		//calculating end position based on direction params.

		boxCollider.enabled = false;		//disabeling box collider to make sure when we cast our ray, we wont hit our own collider.
		hit = Physics2D.Linecast (start, end, blockingLayer);		//casting a line from our start point to our end point checking collision on blocking layer.
		boxCollider.enabled = true; 		//reenable the box collider

		if(hit.transform == null){		//checking if we hit anything. null means the space is open and available to move into...
			StartCoroutine(SmoothMovement(end));		
			return true; //we were able to move
		}
		return false; //if hit.transform was not null aka there was a collision. unable to move.
	}

	protected IEnumerator SmoothMovement (Vector3 end){		//using this to move units from one space to the next. Takes end to specify where to move to.

		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;		//calculating the remaining distance to move. Based on the square magnitude of the dfifference between the current pos and end.
		while(sqrRemainingDistance > float.Epsilon){		//checks if the distance is greater than float.Epsilon. just a really small value almost 0.

			Vector3 newPosition = Vector3.MoveTowards (rb2D.position, end, inverseMoveTime * Time.deltaTime);	//finding a new position proportionally closer to the end based on the move time.
			rb2D.MovePosition(newPosition);		//moving the rigidbody
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;	//rrecalculating the distance.
			yield return null;		//yield return means we will wait for a frame before reevaluating the condition of the loop
		}
	}

	protected virtual void AttemptMove <T> (int xDir, int yDir)		//Generic param T is being used to specify the type of componenet we expect our unit to interact with if it is blocked. player if enemy. walls if player.
		where T: Component			//where specifies that T is going to be a component.
	{
		RaycastHit2D hit;
		bool canMove = Move (xDir, yDir, out hit);		//calling move function. will be true if we can move or false otherwise. hit is out means we will be able to check if the transform we hit in move is null. 

		if (hit.transform == null)		//(nothing was hit, we will return and not execute the code.)
			return;

		T hitComponent = hit.transform.GetComponent<T> ();		//if hit, get a component reference to the component of type T attached to the object that we hit.

		if (!canMove && hitComponent != null)		//if we cant move and the hitcomponent is blocked and has hit something it can interact with
			OnCantMove (hitComponent);		//call the oncantMove which will be manipulated depending on the object and we will pass it hitcomponent aka which component.
			//we are using the generic T because both the player and the Enemy are inheriting from here. They interact with different components so we dont know what they will be interacting with. This is generic so
			//player with walls enemy with player. we can get a reference and pass it in to oncant move which will change in the inheriting classes.
	}

	protected abstract void OnCantMove <T> (T component)		//protected abstract takes a generic param T as well as T component. Abstract means it has a missing or incomplete implementation. going to be overwriten by other classes
		where T: Component;
}
