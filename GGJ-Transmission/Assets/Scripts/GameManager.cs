using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    public Random Random;
    public RedCom RedCom;

    public GameObject AntenaFisicaPrefab;
    public float TiempoDeTransmision = 5.0f;
    

    private readonly List<GameObject> _antenasFisicas = new List<GameObject>();
    private readonly List<GameObject> _textoDistancias = new List<GameObject>();
    private Antena _antenaTransmisora;
    

    private float TimerTransmision = 0.0f;

    
    // Use this for initialization
    void Start () {
        Random = new Random(435353451);

        RedCom = new RedCom();
        RedCom.Generar(Random);
        _antenaTransmisora = RedCom.AntenaEmisora;

        foreach (var nodo in RedCom.Antenas) {
            GameObject antenaFisica = Instantiate(AntenaFisicaPrefab, new Vector3(nodo.X, nodo.Y, 0), Quaternion.identity);
            _antenasFisicas.Add(antenaFisica);

            if (nodo == _antenaTransmisora) {
                CambiarMaterialAntena(antenaFisica, true);
            }

            PropiedadesNodo propiedadesNodo = antenaFisica.GetComponent<PropiedadesNodo>();
            propiedadesNodo.IdAntena = nodo.Id;
            propiedadesNodo.DistanciaAlReceptor = nodo.DistanciaAlReceptor;
        }

        RefreshTextosDistancias();
    }

	// Update is called once per frame
	void Update () {
        foreach (var nodo in RedCom.Antenas)
        {
            foreach (var conexion in nodo.Conexiones)
            {
                Debug.DrawRay(new Vector3(nodo.X, nodo.Y, 0), new Vector3(conexion.X - nodo.X, conexion.Y - nodo.Y, 0));
            }
        }

        TimerTransmision += Time.deltaTime;
	    if (TimerTransmision > TiempoDeTransmision) {
	        TransmitirMensaje();
            TimerTransmision = 0;
        }

	    ManageInput();
	}

    private GameObject GetAntenaFisica(Antena antena)
    {
        return _antenasFisicas.FirstOrDefault(a => a.GetComponent<PropiedadesNodo>().IdAntena == antena.Id);
    }

    private void ManageInput()
    {
        if (Input.GetMouseButtonDown(0))
        { // if left button pressed...
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                int idAntena = hit.collider.GetComponent<PropiedadesNodo>().IdAntena;
                DestruirAntena(RedCom.Antenas.FirstOrDefault(a => a.Id == idAntena));
            }
        }
    }

    private void DestruirAntena(Antena antenaADestruir)
    {
        RedCom.DestruirAntena(antenaADestruir);
        GameObject antenaFisica = GetAntenaFisica(antenaADestruir);
        _antenasFisicas.Remove(antenaFisica);
        Destroy(antenaFisica);

        RefreshTextosDistancias();
    }

    private void RefreshTextosDistancias()
    {
        foreach (var texto in _textoDistancias) {
            Destroy(texto);
        }
        _textoDistancias.Clear();

        foreach (var nodo in RedCom.Antenas) {
            GameObject antenaFisica = GetAntenaFisica(nodo);

            GameObject textDistanceObject = new GameObject();
            _textoDistancias.Add(textDistanceObject);
            textDistanceObject.transform.localScale = new Vector3(0.075f, 0.075f, 0.075f);
            textDistanceObject.transform.position = new Vector3(antenaFisica.transform.position.x, antenaFisica.transform.position.y, -1.0f);

            TextMesh distanceText = textDistanceObject.AddComponent<TextMesh>();

            distanceText.text = nodo.DistanciaAlReceptor != int.MaxValue ? nodo.DistanciaAlReceptor.ToString() : "X";
            distanceText.anchor = TextAnchor.MiddleCenter;
            distanceText.color = Color.black;
            distanceText.fontSize = 64;
            distanceText.offsetZ = -1;
        }
    }

    void CambiarMaterialAntena(GameObject antenaFisica, bool transmitiendo)
    {
        Material newMaterial;
        if (transmitiendo)
        {
            newMaterial = (Material)Resources.Load("Materials/Red.mat", typeof(Material));
        }
        else
        {
            newMaterial = (Material)Resources.Load("Materials/White.mat", typeof(Material));
        }
        antenaFisica.GetComponent<Renderer>().material = newMaterial;
    }

    private void TransmitirMensaje()
    {
        GameObject antenaFisicaTransmisora = _antenasFisicas.FirstOrDefault(aF => aF.GetComponent<PropiedadesNodo>().IdAntena == _antenaTransmisora.Id);
        CambiarMaterialAntena(antenaFisicaTransmisora, false);

        _antenaTransmisora = _antenaTransmisora.CaminoHaciaElReceptor;

        antenaFisicaTransmisora = _antenasFisicas.FirstOrDefault(aF => aF.GetComponent<PropiedadesNodo>().IdAntena == _antenaTransmisora.Id);
        CambiarMaterialAntena(antenaFisicaTransmisora, true);
    }
}
