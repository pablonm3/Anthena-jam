using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Touch : MonoBehaviour {
	Antena AntenaScript;
	private int layerAntenas,
				vecesTocado = 0;
	private static int cantidadDeToquesParaDesactivar = 1;
	[SerializeField]private float LargoDelRayo;

	public static void incCantidadDeToquesTemporalmente(){
		cantidadDeToquesParaDesactivar++;
	}

	void Update () {

		if (Input.GetButton ("Fire1")) {
			Ray mousePosition = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit _Hit;

			if(Physics.Raycast (mousePosition,out _Hit,100f)){
				if (_Hit.collider.CompareTag ("Antena") && Input.GetMouseButtonDown(0)) {
					if (vecesTocado >= cantidadDeToquesParaDesactivar) {
						AntenaScript = _Hit.collider.GetComponent <Antena> ();
						AntenaScript.DesactivarAntena();
						Debug.Log("veces tocado: " + vecesTocado);
						Debug.Log("cantidad para desactivar: " + cantidadDeToquesParaDesactivar);
						vecesTocado = 0;
					} else {
						vecesTocado++;
					}
				}
			}

		}
	}
}
