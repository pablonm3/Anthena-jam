using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrearAntenas : MonoBehaviour {
	public GameObject Antena;

	float x;
	float y;

	public int CantidadDeBolas;
	// Use this for initialization
	void Start () {

		for (int i = 0; i < CantidadDeBolas; i++) {
			x = Random.Range (-9, 9);
			y = Random.Range (-3, 3);
			Instantiate (Antena, new Vector3 (x, y, 0), Quaternion.identity);
		}
	}
	

}
