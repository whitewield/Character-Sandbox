using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public enum PlayerType { quick, normal, slow }
	public PlayerType playerType;
	private float speed;
	private float maxSpeed;
	private float accel;
	private float drag;
	private float jumpSpeed;
	private float currentJumpStrength;
	private Rigidbody2D myBody;
	private float dt;
	private bool grounded = false;
	private float groundCheckCircRad;
	private bool justJumped = false;
	private PlayerType lastType;

	private Animator animator;

	public LayerMask groundLayer;

	// Use this for initialization
	void Start () {
		groundCheckCircRad = .5f;
		myBody = transform.GetComponent<Rigidbody2D>();
		animator = transform.GetComponent<Animator> ();

		changePlayerType(playerType);

	}

	// Update is called once per frame
	void Update () {
		//just a shortcut to make my life easier
		dt = Time.deltaTime;

		//making sure speed is set to what it should be at the start frame after physics calcs have taken place between frames.
		speed = myBody.velocity.x;


		//key input for left and right, we modify speed by acceleration if necessary
		if (Input.GetKey (KeyCode.RightArrow) || Input.GetKey (KeyCode.D)) {
			//if we're moving opposite, let's apply drag until it's time to turn
			if (speed < 0) {
				animator.SetBool ("slowing", true);
				speed += drag * 2 * dt;
				if (speed >= 0) {
					speed = 0;
					animator.SetBool ("slowing", false);
				}
				//otherwise let's run for it wahoo
			} else  if (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.A)) {
				animator.SetBool ("slowing", false);
				if (speed < maxSpeed) {
					speed += accel * dt;
				} else {
					speed = maxSpeed;
				}
			}
		}

		if (Input.GetKey (KeyCode.LeftArrow) || Input.GetKey (KeyCode.A)) {
			if (speed > 0) {
				animator.SetBool ("slowing", true);
				speed -= drag * 2 * dt;
				if (speed <= 0){
					speed = 0;
					animator.SetBool ("slowing", false);
				}
			} else if (!Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.D)){
				animator.SetBool ("slowing", false);
				if (speed > -maxSpeed) {
					speed -= accel * dt;
				} else {
					speed = -maxSpeed;
				}
			}
		}

		if (!Input.GetKey (KeyCode.LeftArrow) && !Input.GetKey (KeyCode.RightArrow) && !Input.GetKey (KeyCode.A) && !Input.GetKey (KeyCode.D)) {
			if (speed > .2 || speed < -.2) {
				animator.SetBool ("slowing", true);
			}
			if (grounded){
				if (speed > 0) {
					speed -= drag * dt;
					if (speed < 0) {
						speed = 0;
						animator.SetBool ("slowing", false);
					}
				} else if (speed < 0) {

					speed += drag * dt;
					if (speed > 0) {
						speed = 0;
						animator.SetBool ("slowing", false);
					}
				}
			}
		}

		// now let's figure out the jump stuff

		//check if object is on ground via an overlap circle checking for the ground layer from around the object's feet.
		if (!justJumped) {
			grounded = Physics2D.OverlapCircle (new Vector2 (transform.position.x, transform.position.y - transform.GetComponent<SpriteRenderer>().bounds.size.y/2), groundCheckCircRad, groundLayer);
		} else {
			grounded = false;
		}

		if((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && grounded && !justJumped) {
			//if we're on the ground and haven't just jumped, we'll jump. we'll use 'justjumped' to tell our anims not to switch unless it's time.
			justJumped = true;
			myBody.velocity = new Vector2 (speed, jumpSpeed);
			grounded = false;
			//switch to jump anim.
			animator.Play ("playerJump");
		}

		if (!grounded && myBody.velocity.y <= 0) {
			//make us play a 'falling' animation if we are on the way down.
			justJumped = false;
		}

		//apply x velocity changes.
		myBody.velocity = new Vector2 (speed, myBody.velocity.y);



		//change the playerType if the player changes it in the inspector
		if (playerType != lastType) {
			changePlayerType(playerType);
		}
			
		//animator state control
		//reminder: slowing is controlled by movement control, so let's not worry about it here.
		//grounded?
		animator.SetBool("grounded",grounded);

		//moving?
		if (myBody.velocity.x > .1 || myBody.velocity.x < -.1) {
			animator.SetBool ("moving", true);
		} else {
			animator.SetBool ("moving", false);
			animator.SetBool ("slowing", false);
		}



		//falling?
		if (myBody.velocity.y <= 0) {
			animator.SetBool ("falling", true);
		} else {
			animator.SetBool ("falling", false);
		}
	
	}

	//this happens after update, and after the animator updates.
	void LateUpdate(){
		if (grounded) {
			if (myBody.velocity.x > 0 && !animator.GetCurrentAnimatorStateInfo(0).IsName("playerSkid") && animator.GetBool("slowing") == false) {
				transform.GetComponent<SpriteRenderer> ().flipX = true;
			} else if (myBody.velocity.x < 0 && !animator.GetCurrentAnimatorStateInfo(0).IsName("playerSkid") && animator.GetBool("slowing") == false) {
				transform.GetComponent<SpriteRenderer> ().flipX = false;
			}
		}
	}


	void changePlayerType(PlayerType _type){

		switch(playerType) {
		case PlayerType.quick:
			maxSpeed = 12;
			accel = 40;
			jumpSpeed = 20;
			drag = 40;
			myBody.gravityScale = 5;
			break;
		case PlayerType.normal:
			maxSpeed = 8;
			accel = 8;
			jumpSpeed = 15;
			drag = 12;
			myBody.gravityScale = 3;
			break;
		case PlayerType.slow:
			maxSpeed = 4;
			accel = 4;
			jumpSpeed = 19;
			drag = 20;
			myBody.gravityScale = 5;
			break;
		}
		lastType = playerType;

	}
}

