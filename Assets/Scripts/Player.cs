using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

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
				speed += drag * 2 * dt;
				if (grounded) {
					animator.Play ("playerSkid");
				}
				if (speed >= 0) {
					speed = 0;
					if (grounded) {
						animator.Play ("playerIdle");
					}

				}
				//otherwise let's run for it wahoo
			} else {
				if (speed < maxSpeed) {
					speed += accel * dt;
				} else {
					speed = maxSpeed;
				}
				if (grounded) {
					//if we're on the ground, we should also change our animations.
					animator.Play ("playerRun");
					if (transform.localScale.x == 1) {
						Vector3 myScale = transform.localScale;
						myScale.x = -1;
						transform.localScale = myScale;
					}
				}

			}
		}

		if (Input.GetKey (KeyCode.LeftArrow) || Input.GetKey (KeyCode.A)) {
			if (speed > 0) {
				speed -= drag * 2 * dt;
				if (grounded) {
					animator.Play ("playerSkid");
				}
				if (speed <= 0){
					speed = 0;
					if (grounded) {
						animator.Play ("playerIdle");
					}
				}
			} else {
				if (speed > -maxSpeed) {
					speed -= accel * dt;
				} else {
					speed = -maxSpeed;
				}
				if (grounded) {
					animator.Play ("playerRun");
					if (transform.localScale.x == -1) {
						Vector3 myScale = transform.localScale;
						myScale.x = 1;
						transform.localScale = myScale;
					}
				}
			}
		}

		if (grounded && !Input.GetKey (KeyCode.LeftArrow) && !Input.GetKey (KeyCode.RightArrow) && !Input.GetKey (KeyCode.A) && !Input.GetKey (KeyCode.D)) {
			if (speed > 0) {
				speed -= drag * dt;
				animator.Play ("playerSkid");
				if (speed < 0) {
					speed = 0;
					animator.Play ("playerIdle");
				}
			} else if (speed < 0) {
				speed += drag * dt;
				animator.Play ("playerSkid");
				if (speed > 0) {
					speed = 0;
					animator.Play ("playerIdle");
				}
			} else {
					animator.Play ("playerIdle");
			}
		}
			
		// now let's figure out the jump stuff

		//check if object is on ground via an overlap circle checking for the ground layer from around the object's feet.
		if (!justJumped) {
			grounded = Physics2D.OverlapCircle (new Vector2 (transform.position.x, transform.position.y - transform.GetComponent<SpriteRenderer>().bounds.size.y/2), groundCheckCircRad, groundLayer);
			Debug.Log ("gorunded");
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
			animator.Play ("playerFall");
		}

		//apply x velocity changes.
		myBody.velocity = new Vector2 (speed, myBody.velocity.y);



		//change the playerType if the player changes it in the inspector
		if (playerType != lastType) {
			changePlayerType(playerType);
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
