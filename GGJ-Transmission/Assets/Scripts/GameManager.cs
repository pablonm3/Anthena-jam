using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    public System.Random Random;
    public Grafo Red;

    public GameObject Antena;
    private List<LineRenderer> _lineRenderers;

    // Use this for initialization
    void Start () {
        Random = new Random(435453452);
        //_lineRenderers = GetComponent<LineRenderer>();

        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

        Red = Grafo.GenerarGrafo(Random, 15, width - 1, height - 1);

        foreach (var nodo in Red.Nodos) {
            Instantiate(Antena, new Vector3(nodo.X, nodo.Y, 0), Quaternion.identity);

            //foreach (var conexion in nodo.Conexiones) {
            //    LineRenderer newLineRenderer = new LineRenderer();
            //    newLineRenderer.SetPosition(0, new Vector3(nodo.X, nodo.Y, 0));
            //    newLineRenderer.SetPosition(1, new Vector3(conexion.X, conexion.Y, 0));
            //    _lineRenderers.Add(newLineRenderer);
            //}
        }
    }
	
	// Update is called once per frame
	void Update () {
        foreach (var nodo in Red.Nodos)
        {
            foreach (var conexion in nodo.Conexiones)
            {
                Debug.DrawRay(new Vector3(nodo.X, nodo.Y, 0), new Vector3(conexion.X - nodo.X, conexion.Y - nodo.Y, 0));
            }
        }
    }
}
