using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActivarAnteas : MonoBehaviour {
	public GameObject[] Antenas;

	public int CantidadDeAntenas,
			   antenasGeneradasEnParalelo = 1;
	public float Cadencia = 3000;
	void Start () {
		Debug.Log ("starting the game");
		Antenas = new GameObject[CantidadDeAntenas];
		Invoke ("InicializarAntenas", 1f);
	}

	public void InicializarAntenas(){
		Antenas = GameObject.FindGameObjectsWithTag ("Antena");
		InvokeRepeating ("activarAntenas", 0, Cadencia);
		InvokeRepeating ("incrementarDificultad", 0, 5);
	}

	public void incrementarDificultad(){
		int opcion = Random.Range (0, 3);
		switch (opcion) {
		case 0: {	reducirCadencia();
					break;
			}
		case 1: {
				aumentarCantAntenasGeneraParalelo();
				break;
			}
		case 2: {
				aumentarToquesNecesariosParaDesactivar();
				break;
		}
		default: Debug.Log ("ERROR, OPCION INVALIDA EN incrementarDificultad");
				 break;
		}
	}

	public void aumentarCantAntenasGeneraParalelo(){
		antenasGeneradasEnParalelo++;
	}

	public void aumentarToquesNecesariosParaDesactivar(){
		Debug.Log("aumentar toques necesarios");
	}		

	public void reducirCadencia(){
		Debug.Log ("cadencia: "+ Cadencia);
		if (Cadencia > 0.3f)
			Cadencia -= 0.3f;
		else
			Cadencia = 0.2f;
		CancelInvoke("incrementarDificultad");
		InvokeRepeating ("incrementarDificultad",0,Cadencia);
	}

	public void activarAntenas(){
		Antena[] antenasScript = GetProximasAntenasScripts();
		for(int i = 0; i < antenasScript.Length; i++) {
			antenasScript[i].ActivarAntena();
		}
	}

	public bool todasLasAntenasEncendidas(){
		for(int i = 0 ; i < CantidadDeAntenas ; i++){
			Antena antenaScript =  Antenas[i].GetComponent<Antena>();
			if (!antenaScript.Encendida)
				return false;
		}
		return true;
	}

	public Antena[] GetProximasAntenasScripts(){
		Antena[] antenasScript = new Antena[antenasGeneradasEnParalelo];
		for (int i = 0; i < antenasScript.Length; i++) {
			if (todasLasAntenasEncendidas()) {
				Debug.Log("JUEGO TERMINADO, CARGANDO NUEVA ESCENA");
				SceneManager.LoadScene(0);
				CancelInvoke("activarAntenas");
				return new Antena[0];
			}
			int posNextAntena =  Random.Range (0, CantidadDeAntenas);
			Antena antena =  Antenas [posNextAntena].GetComponent<Antena>();
			while(antena.Encendida) {
				posNextAntena =  Random.Range (0, CantidadDeAntenas);
				antena =  Antenas [posNextAntena].GetComponent<Antena> ();
			}
			antenasScript[i] = antena;
		}
		return antenasScript;
	}
}