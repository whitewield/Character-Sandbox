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
		speed = myBody.velocity.x;


		//key input for left and right, we modify speed by acceleration if necessary
		if (Input.GetKey (KeyCode.RightArrow) || Input.GetKey (KeyCode.D)) {
			if (speed < 0) {
				speed += drag * 2 * dt;
				animator.Play ("playerSkid");
				if (speed >= 0) {
					speed = 0;
					if (grounded) {
						animator.Play ("playerIdle");
					}

				}
			} else {
				if (speed < maxSpeed) {
					speed += accel * dt;
				} else {
					speed = maxSpeed;
				}
				if (grounded) {
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
				animator.Play ("playerSkid");
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

		//check if object is on gorund
		if (!justJumped) {
			grounded = Physics2D.OverlapCircle (new Vector2 (transform.position.x, transform.position.y - .5f), groundCheckCircRad, groundLayer);
			Debug.Log ("gorunded");
		} else {
			grounded = false;
		}

		if((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && grounded && !justJumped) {
			
			justJumped = true;
			myBody.velocity = new Vector2 (speed, jumpSpeed);
			grounded = false;
			animator.Play ("playerJump");
		}
			
		if (!grounded && myBody.velocity.y <= 0) {
			justJumped = false;
			animator.Play ("playerFall");
		}

		myBody.velocity = new Vector2 (speed, myBody.velocity.y);



		//change the playerType if the player changes it in the inspector
		if (playerType != lastType) {
			changePlayerType(playerType);
		}
	}

	void changePlayerType(PlayerType _type){
			
		switch(playerType) {
		case PlayerType.quick:
			maxSpeed = 10;
			accel = 20;
			jumpSpeed = 20;
			drag = 25;
			myBody.gravityScale = 2;
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
			accel = 8;
			jumpSpeed = 19;
			drag = 20;
			myBody.gravityScale = 5;
			break;
		}
		lastType = playerType;

	}
}
