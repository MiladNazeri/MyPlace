﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MyPlace
{
	public class PaintballManager : ActivityManager
	{
		//readonly

		//Serialized
		[SerializeField]
		protected GameObject paintballPrefab;
		[SerializeField]
		protected LayerMask layerMask;
		
		/////Protected/////
		//References
		//Primitives

		public override ActivityObject.ActivityObjectType ActivityType {
			get {
				return ActivityObject.ActivityObjectType.Paintball;
			}
		}

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//
		
		protected void Awake()
		{

		}
		
		protected void Start ()
		{
			
		}
		
		protected void Update ()
		{
			
		}
		
		///////////////////////////////////////////////////////////////////////////
		//
		// PaintballManager Functions
		//

		protected void CreatePaintball() {
			

			Vector3 spawnPosition = Input.Instance.GetCameraPosition() + Input.Instance.GetCameraTransform().right*.15f;
			Vector3 spawnDirection = Input.Instance.GetCameraForward();

			Ray ray = new Ray(spawnPosition,spawnDirection);
			RaycastHit raycastHit;
			if(Physics.Raycast(ray,out raycastHit,10f,layerMask)) {


			}else {
				Debug.Log("Could not find collider for paintball.");
				return;
			}


			Color paintballColor = Color.HSVToRGB(UnityEngine.Random.value,.8f,1f);

			GameObject paintballGameObject = Instantiate (paintballPrefab,spawnPosition,Quaternion.identity) as GameObject;
//			Destroy(paintballGameObject,10f);


			Paintball paintball = paintballGameObject.GetComponent<Paintball>();
			paintball.Launch(raycastHit.point,paintballColor);

			Audio.Instance.PlaySoundEffect(Audio.SoundEffect.WhooshMedium);
		}

		////////////////////////////////////////
		//
		// Function Functions

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from ActivityManager
		//
	

		public override void LaunchActivity () {

			CreatePaintball();

		}

		public override void StartActivity () {


		}

		public override void StopActivity () {


		}


	}
}