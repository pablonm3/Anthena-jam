using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.UIElements;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    public Random Random;
    public RedCom RedCom;

    public GameObject AntenaFisicaPrefab;
    public GameObject PingPrefab;
    public float TiempoDeTransmision = 5.0f;
    public float TiempoDePing = 0.25f;

    public bool Pinging;
    public bool PingingLlegoADestino;

    public Material MaterialInactiva;
    public Material MaterialTransmitiendo;
    public Material MaterialEstacion;
    public Estacion EstacionRed;

    private float _timerTransmision;
    private float _timerPing;

    private readonly List<GameObject> _antenasFisicas = new List<GameObject>();
    private readonly List<GameObject> _textoDistancias = new List<GameObject>();
    private Antena _antenaTransmisoraMensaje;
    private Antena _antenaTransmisoraPing;
    private Antena _antenaOrigenPing;
    private Antena _antenaDestinoPing;
    private GameObject _pingFisico;


    void Start () {
        Random = new Random(435353451);

        RedCom = new RedCom();
        RedCom.Generar(Random);
        _antenaTransmisoraMensaje = RedCom.AntenaEmisora;
        _antenaTransmisoraMensaje.MensajeTrasnmitido = true;

        foreach (var nodo in RedCom.Antenas) {
            GameObject antenaFisica = Instantiate(AntenaFisicaPrefab, new Vector3(nodo.X, nodo.Y, 0), Quaternion.identity);
            _antenasFisicas.Add(antenaFisica);

            if (nodo == RedCom.AntenaEstacion)
            {
                CambiarMaterialAntena(antenaFisica, MaterialEstacion);
                antenaFisica.transform.localScale *= 2.0f;
            }

            if (nodo == _antenaTransmisoraMensaje) {
                CambiarMaterialAntena(antenaFisica, MaterialTransmitiendo);
            }

            PropiedadesAntena propiedadesAntena = antenaFisica.GetComponent<PropiedadesAntena>();
            propiedadesAntena.IdAntena = nodo.Id;
            propiedadesAntena.Grupo = nodo.Grupo;

            if (nodo.AntenaGrande) {
                antenaFisica.transform.localScale *= 1.5f;
            }
        }

        RefreshTextosDistancias();
    }

	void Update () {
        foreach (var nodo in RedCom.Antenas)
        {
            foreach (var conexion in nodo.Conexiones)
            {
                Debug.DrawRay(new Vector3(nodo.X, nodo.Y, 0), new Vector3(conexion.X - nodo.X, conexion.Y - nodo.Y, 0));
            }
        }

        // Input
        ManageInput();

        // Mensaje
        _timerTransmision += Time.deltaTime;
	    if (_timerTransmision > TiempoDeTransmision) {
	        TransmitirMensaje();
            _timerTransmision = 0;
        }

        // Ping
	    if (Pinging) {
            _timerPing += Time.deltaTime;
	        float alphaPing = _timerPing / TiempoDePing;
	        float pingX = (1 - alphaPing) * _antenaTransmisoraPing.X +
	                      alphaPing * _antenaTransmisoraPing.GetRuta(_antenaDestinoPing).X;
            float pingY = (1 - alphaPing) * _antenaTransmisoraPing.Y +
                          alphaPing * _antenaTransmisoraPing.GetRuta(_antenaDestinoPing).Y;
	        _pingFisico.transform.position = new Vector3(pingX, pingY, -1);
            if (_timerPing > TiempoDePing) {
                TransmitirPing();
                _timerPing = 0;
            }
	    }
	}

    private GameObject GetAntenaFisica(Antena antena)
    {
        return _antenasFisicas.FirstOrDefault(a => a.GetComponent<PropiedadesAntena>().IdAntena == antena.Id);
    }

    private void ManageInput()
    {
        if (Input.GetMouseButtonDown((int) MouseButton.LeftMouse))
        {
            // Ping
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                int idAntena = hit.collider.GetComponent<PropiedadesAntena>().IdAntena;
                PingAntena(RedCom.Antenas.FirstOrDefault(a => a.Id == idAntena));
            }
        } else if (Input.GetMouseButtonDown((int) MouseButton.RightMouse)) {
            // Destroy
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                int idAntena = hit.collider.GetComponent<PropiedadesAntena>().IdAntena;
                DestruirAntena(RedCom.Antenas.FirstOrDefault(a => a.Id == idAntena));
            }
        }
    }

    private void PingAntena(Antena antenaAPinguear)
    {
        if (Pinging || antenaAPinguear == RedCom.AntenaEstacion)
        {
            return;
        }

        _antenaOrigenPing = RedCom.AntenaEstacion;
        _antenaTransmisoraPing = _antenaOrigenPing;
        _antenaDestinoPing = antenaAPinguear;
        _timerPing = 0;
        Pinging = true;

        _pingFisico = Instantiate(PingPrefab, new Vector3(_antenaOrigenPing.X, _antenaOrigenPing.Y, 0), Quaternion.identity);
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

            distanceText.text = nodo.GetDistancia(RedCom.AntenaReceptora) != int.MaxValue ? nodo.GetDistancia(RedCom.AntenaReceptora).ToString() : "X";
            distanceText.anchor = TextAnchor.MiddleCenter;
            distanceText.color = Color.black;
            distanceText.fontSize = 64;
            distanceText.offsetZ = -1;
        }
    }

    void CambiarMaterialAntena(GameObject antenaFisica, Material material)
    {
        antenaFisica.GetComponent<Renderer>().material = material;
    }

    private void TransmitirMensaje()
    {
        GameObject antenaFisicaTransmisora = GetAntenaFisica(_antenaTransmisoraMensaje);
        if (_antenaTransmisoraMensaje == RedCom.AntenaEstacion) {
            CambiarMaterialAntena(antenaFisicaTransmisora, MaterialEstacion);
        } else {
            CambiarMaterialAntena(antenaFisicaTransmisora, MaterialInactiva);
        }

        _antenaTransmisoraMensaje = _antenaTransmisoraMensaje.GetRuta(RedCom.AntenaReceptora);
        _antenaTransmisoraMensaje.MensajeTrasnmitido = true;
        if (_antenaTransmisoraMensaje == RedCom.AntenaReceptora) {
            MensajeEnDestino();
        }

        antenaFisicaTransmisora = GetAntenaFisica(_antenaTransmisoraMensaje);
        CambiarMaterialAntena(antenaFisicaTransmisora, MaterialTransmitiendo);
    }

    private void TransmitirPing()
    {
        GameObject antenaFisicaTransmisora = GetAntenaFisica(_antenaTransmisoraMensaje);
        if (_antenaTransmisoraMensaje == RedCom.AntenaEstacion)
        {
            CambiarMaterialAntena(antenaFisicaTransmisora, MaterialEstacion);
        }
        else
        {
            CambiarMaterialAntena(antenaFisicaTransmisora, MaterialInactiva);
        }

        _antenaTransmisoraPing = _antenaTransmisoraPing.GetRuta(_antenaDestinoPing);
        if (_antenaTransmisoraPing == _antenaDestinoPing) {
            PingingLlegoADestino = true;
            _antenaDestinoPing = _antenaOrigenPing;
        }

        if (PingingLlegoADestino && _antenaTransmisoraPing == _antenaOrigenPing) {
            ResultadoPing();
            Destroy(_pingFisico);
            Pinging = false;
        }

        antenaFisicaTransmisora = GetAntenaFisica(_antenaTransmisoraMensaje);
        CambiarMaterialAntena(antenaFisicaTransmisora, MaterialTransmitiendo);
    }

    private void MensajeEnDestino()
    {
        
    }

    private void ResultadoPing()
    {
        
    }
}
