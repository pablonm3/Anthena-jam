using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActivarAnteas : MonoBehaviour {
	public Antena antenaScript;
	public GameObject[] Antenas;

	public int CantidadDeAntenas;
	public float Cadencia;
	void Start () {
		Antenas = new GameObject[CantidadDeAntenas];
		Invoke ("InicializarAntenas",1f);
	}

	public void InicializarAntenas(){
		for(int i = 0 ; i < CantidadDeAntenas ; i++){
			Antenas = GameObject.FindGameObjectsWithTag ("Antena");
		}
		InvokeRepeating ("activarAntena",0,Cadencia);
	}

	public void activarAntena(){
		antenaScript = GetProximaAntenaScript();
		antenaScript.ActivarAntena ();
		
	}


	public Antena GetProximaAntenaScript(){
		int antena =  Random.Range (0, CantidadDeAntenas);
		antenaScript =  Antenas [antena].GetComponent<Antena> ();
		int cantAntenas = Antenas.Length,
			antenasProbadas = 0;
		while(antenaScript.Encendida) {
			antena =  Random.Range (0, CantidadDeAntenas);
			antenaScript =  Antenas [antena].GetComponent<Antena> ();
			Debug.Log ("AntenasProbadas: "+antenasProbadas.ToString ());
			Debug.Log ("CantAntenas: "+cantAntenas.ToString ());

			if(antenasProbadas == cantAntenas){
				Debug.Log ("Holis");
				SceneManager.LoadScene (0);
			}
			antenasProbadas++;
			return antenaScript;
		}
		return antenaScript;
	}
}