using System;
using UnityEngine;

// Include the namespace required to use Unity UI
using UnityEngine.UI;

using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{

	private const float ANGLE_MOVEMENT_THRESHOLD = 15.0f;
	// Create public variables for player speed, and for the Text UI game objects
	public float speed;
	public Text countText;
	public Text winText;

	public float angleX;
	public float angleZ;

	// Create private references to the rigidbody component on the player, and the count of pick up objects picked up so far
	private Rigidbody rb;
	private int count;
	public float vertical;
	public float horizontal;
	private MeshRenderer renderer;

	// At the start of the game..
	void Start ()
	{
		// Assign the Rigidbody component to our private rb variable
		rb = GetComponent<Rigidbody>();

		// Set the count to zero 
		count = 0;

		// Run the SetCountText function to update the UI (see below)
		SetCountText ();

		// Set the text property of our Win Text UI to an empty string, making the 'You Win' (game over message) blank
		winText.text = "";

		renderer = GetComponent<MeshRenderer>();

	}

	// Each physics step..
	void FixedUpdate ()
	{

		if (Mathf.Abs(angleX) > ANGLE_MOVEMENT_THRESHOLD)
			vertical = map(angleX,-90,90,1,-1);		
		else
			vertical = 0;

		if (Mathf.Abs(angleZ) > ANGLE_MOVEMENT_THRESHOLD)
			horizontal = map(angleZ,90,-90,-1,1);		
		else
			horizontal = 0;

		// Create a Vector3 variable, and assign X and Z to feature our horizontal and vertical float variables above
		Vector3 movement = new Vector3 (horizontal, 0.0f, vertical);

		// Add a physical force to our Player rigidbody using our 'movement' Vector3 above, 
		// multiplying it by 'speed' - our public player speed that appears in the inspector
		//rb.AddForce (movement * speed,ForceMode.VelocityChange);

		renderer.sharedMaterial.color = new Color(rb.velocity.x,rb.velocity.y,rb.velocity.z);
	}

	// When this game object intersects a collider with 'is trigger' checked, 
	// store a reference to that collider in a variable named 'other'..
	void OnTriggerEnter(Collider other) 
	{
		// ..and if the game object we intersect has the tag 'Pick Up' assigned to it..
		if (other.gameObject.CompareTag ("Pick Up"))
		{
			// Make the other game object (the pick up) inactive, to make it disappear
			other.gameObject.SetActive (false);

			// Add one to the score variable 'count'
			count = count + 1;

			// Run the 'SetCountText()' function (see below)
			SetCountText ();
		}
	}

	// Create a standalone function that can update the 'countText' UI and check if the required amount to win has been achieved
	void SetCountText()
	{
		// Update the text field of our 'countText' variable
		countText.text = "Count: " + count.ToString ();

		// Check if our 'count' is equal to or exceeded 12
		if (count >= 12) 
		{
			// Set the text value of our 'winText'
			winText.text = "You Win!";
		}
	}
	
	float map(float s, float a1, float a2, float b1, float b2)
	{
		return b1 + (s-a1)*(b2-b1)/(a2-a1);
	}
	
}