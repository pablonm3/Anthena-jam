using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Touch : MonoBehaviour {
	Antena AntenaScript;
	private int layerAntenas;
	[SerializeField]private float LargoDelRayo;

	void Update () {
		if (Input.GetButton ("Fire1")) {
			Ray mousePosition = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit _Hit;

			if(Physics.Raycast (mousePosition,out _Hit,100f)){
				if (_Hit.collider.CompareTag ("Antena")) {
					AntenaScript = _Hit.collider.GetComponent <Antena>();
					AntenaScript.DesactivarAntena ();
				}
			}

		}
	}
}
