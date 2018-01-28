using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {
	public GameObject MenuPrincipal;
	public GameObject creditosMenu;

	public void Empezar(){
		SceneManager.LoadScene (1);
	}

	public void Salir(){
		Debug.Log ("Apretaste Salir");
		Application.Quit ();
	}

	public void Creditos(){
		MenuPrincipal.SetActive (false);
		creditosMenu.SetActive (true);
	}

	public void Volver(){
		MenuPrincipal.SetActive (true);
		creditosMenu.SetActive (false);
	}

}
