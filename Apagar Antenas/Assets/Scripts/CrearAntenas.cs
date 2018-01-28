using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrearAntenas : MonoBehaviour {
	public GameObject Antena;
	public GameObject[] AntenasInstanciadas;
	int x;
	int y;
	public float DistanciaMinima;
	public int CantidadDeAntenas;
	// Use this for initialization
	void Start () {
		AntenasInstanciadas = new GameObject[CantidadDeAntenas];
		for (int i = 0; i < CantidadDeAntenas; i++) {
			x = Random.Range (-11, 11);
			y = Random.Range (-4, 7);
			AntenasInstanciadas[i] = Instantiate (Antena, new Vector3 (x, y, 0), Quaternion.identity) as GameObject;
			Invoke ("reacomodar", .15f);

		}

	}
		


	void Reacomodar(){
		
		for (int a = 1; a < CantidadDeAntenas; a++) {
			if (AntenasInstanciadas [a] != null) {	
				return;
			}
			if (Vector3.Distance (AntenasInstanciadas [a - 1].transform.position, AntenasInstanciadas [a].transform.position) < DistanciaMinima) {
					x = Random.Range (-11, 11);
					y = Random.Range (-4, 7);
					AntenasInstanciadas [a].transform.position = new Vector3 (x, y, 0);
				}

			}
		}


}



	




