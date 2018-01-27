using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Antena : MonoBehaviour {
	public Material[] Colores;
	Renderer material;

	public bool Encendida;
	// Use this for initialization
	void Start () {
		Encendida = false;
		material = GetComponent<Renderer> ();
	}
	
	public void ActivarAntena(){
		Encendida = true;
		material.material = Colores [1];
	}

	public void DesactivarAntena(){
		Encendida = false;
		material.material = Colores [0];
	}
}
