using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : MonoBehaviour {
	
	float smooth = 5.0f;
	float tiltAngle = 60.0f;

	public PlayerController player;
	
	void Update()
	{
		// Smoothly tilts a transform towards a target rotation.
		float tiltAroundZ = player.horizontal * tiltAngle;
		float tiltAroundX = player.vertical * tiltAngle;

		
		Quaternion target = Quaternion.Euler(tiltAroundX, 0, tiltAroundZ);

		// Dampen towards the target rotation
		transform.rotation = Quaternion.Slerp(transform.rotation, target,  Time.deltaTime * smooth);
	}
	
}
