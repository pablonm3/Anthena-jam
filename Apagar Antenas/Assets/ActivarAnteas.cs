using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActivarAnteas : MonoBehaviour {
	public Antena antenaScript;
	public GameObject[] Antenas;

	public int CantidadDeAntenas;
	public float Cadencia = 3000;
	void Start () {
		Debug.Log ("starting the game");
		Antenas = new GameObject[CantidadDeAntenas];
		Invoke ("InicializarAntenas",1f);
	}

	public void InicializarAntenas(){
		Antenas = GameObject.FindGameObjectsWithTag ("Antena");
		InvokeRepeating ("activarAntena", 0, Cadencia);
		InvokeRepeating ("reducirCadencia", 0, 5);
	}

	public void reducirCadencia(){
		Debug.Log ("cadencia: "+ Cadencia);
		if (Cadencia > 0.3f)
			Cadencia -= 0.3f;
		else
			Cadencia = 0.2f;
		CancelInvoke("activarAntena");
		InvokeRepeating ("activarAntena",0,Cadencia);
	}

	public void activarAntena(){
		Debug.Log ("desactivate next anthena");
		antenaScript = GetProximaAntenaScript();
		if(antenaScript != null)
			antenaScript.ActivarAntena();
	}

	public bool todasLasAntenasEncendidas(){
		for(int i = 0 ; i < CantidadDeAntenas ; i++){
			Antena antenaScript =  Antenas[i].GetComponent<Antena>();
			if (!antenaScript.Encendida)
				return false;
		}
		return true;
	}

	public Antena GetProximaAntenaScript(){
		int posNextAntena =  Random.Range (0, CantidadDeAntenas);
		antenaScript =  Antenas [posNextAntena].GetComponent<Antena>();
		if (todasLasAntenasEncendidas()) {
			Debug.Log("JUEGO TERMINADO, CARGANDO NUEVA ESCENA");
			SceneManager.LoadScene(0);
			CancelInvoke("activarAntena");
			return null;
		}
		while(antenaScript.Encendida) {
			posNextAntena =  Random.Range (0, CantidadDeAntenas);
			antenaScript =  Antenas [posNextAntena].GetComponent<Antena> ();
		}
		return antenaScript;
	}
}