using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Antena : MonoBehaviour {
//	public Material[] Colores;
//	Renderer material;
	//public Text Score;
	//public int ScoreInt;
	public bool Encendida;

	Animator Anim;

	void Start () {
		Anim = GetComponent <Animator>();
		//Score = GameObject.Find ("Puntos").GetComponent<Text> ();
		//ScoreInt = 0;
		//Score.text = "Puntos: " + ScoreInt.ToString ();
		Encendida = false;
	//	material = GetComponent<Renderer> ();
	}
	
	public void ActivarAntena(){
		Anim.SetBool ("Activada",true);
		Encendida = true;
	//	material.material = Colores [1];
	}

	public void DesactivarAntena(){
		Anim.SetBool ("Activada",false);
		//ScoreInt = ScoreInt + 1;
		//Score.text = "Puntos: " + ScoreInt.ToString ();
		Encendida = false;
		//material.material = Colores [0];
	}

	void OnTriggerEnter(Collider other){
		if (other.CompareTag ("Antena")) {
			float x = other.transform.position.x - transform.position.x;
			float y = other.transform.position.y - transform.position.y;
			Mathf.Round (x);
			Mathf.Round (y);
			float Xclamp = Mathf.Clamp (x, -11, 11);
			float Yclamp = Mathf.Clamp (y, -4, 7);

			transform.position = transform.position - new Vector3 (Xclamp, Yclamp, 0);
			Debug.Log ("Colisionando");
		}


	}

}
