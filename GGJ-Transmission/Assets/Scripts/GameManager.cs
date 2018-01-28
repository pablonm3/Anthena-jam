using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.UIElements;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    public Random Random;
    public RedCom RedCom;

    public int GenerationSeed = 435353451;

    public Text MensajePing;
    public bool DebugMode = false;

    public GameObject AntenaFisicaPrefab;
    public GameObject PingPrefab;
    public float TiempoDeTransmision = 5.0f;
    public float TiempoDePing = 0.25f;

    public Material MaterialInactiva;
    public Material MaterialTransmitiendo;
    public Material MaterialEstacion;

    private bool _pinging;
    private bool _pingingLlegoADestino;

    private float _timerTransmision;
    private float _timerPing;

    private readonly List<GameObject> _antenasFisicas = new List<GameObject>();
    private readonly List<GameObject> _textoDistancias = new List<GameObject>();
    private Antena _antenaTransmisoraMensaje;
    private Antena _antenaTransmisoraPing;
    private Antena _antenaOrigenPing;
    private Antena _antenaDestinoPing;
    private GameObject _pingFisico;

    private bool _pingExitoso;
    private string _pingMensaje;

    void Start ()
    {
        MensajePing.text = "";

        Random = new Random(GenerationSeed);

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

            foreach (var conexion in nodo.Conexiones) {
                GameObject objetoLinea = new GameObject();
                var lineRenderer = objetoLinea.AddComponent< LineRenderer>();
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, new Vector3(nodo.X, nodo.Y, 5));
                lineRenderer.SetPosition(1, new Vector3(conexion.X, conexion.Y, 5));

                lineRenderer.startWidth = 0.05f;
                lineRenderer.endWidth = 0.02f;

                lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
                lineRenderer.startColor = Color.red;
                lineRenderer.endColor = Color.black;
            }
        }

        if (DebugMode) {
            RefreshTextosDistancias();
        }
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
	    if (_pinging) {
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
        if (_pinging || antenaAPinguear == RedCom.AntenaEstacion)
        {
            return;
        }

        _antenaOrigenPing = RedCom.AntenaEstacion;
        _antenaTransmisoraPing = _antenaOrigenPing;
        _antenaDestinoPing = antenaAPinguear;
        _timerPing = 0;
        _pinging = true;

        _pingFisico = Instantiate(PingPrefab, new Vector3(_antenaOrigenPing.X, _antenaOrigenPing.Y, 0), Quaternion.identity);
    }

    private void DestruirAntena(Antena antenaADestruir)
    {
        RedCom.DestruirAntena(antenaADestruir);
        GameObject antenaFisica = GetAntenaFisica(antenaADestruir);
        _antenasFisicas.Remove(antenaFisica);
        Destroy(antenaFisica);

        if (DebugMode) {
            RefreshTextosDistancias();
        }
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
        antenaFisicaTransmisora.GetComponent<Animator>().SetBool("Emitiendo", false);

        _antenaTransmisoraMensaje = _antenaTransmisoraMensaje.GetRuta(RedCom.AntenaReceptora);
        _antenaTransmisoraMensaje.MensajeTrasnmitido = true;
        if (_antenaTransmisoraMensaje == RedCom.AntenaReceptora) {
            MensajeEnDestino();
        }

        antenaFisicaTransmisora = GetAntenaFisica(_antenaTransmisoraMensaje);
        antenaFisicaTransmisora.GetComponent<Animator>().SetBool("Emitiendo", true);
    }

    private void TransmitirPing()
    {
        _antenaTransmisoraPing = _antenaTransmisoraPing.GetRuta(_antenaDestinoPing);

        if (_pingingLlegoADestino && _antenaTransmisoraPing == _antenaOrigenPing) {
            ResultadoPing();
            Destroy(_pingFisico);
            _pinging = false;
        } else {
            if (_antenaTransmisoraPing == _antenaDestinoPing)
            {
                _pingingLlegoADestino = true;
                _pingExitoso = _antenaDestinoPing.MensajeTrasnmitido || _antenaTransmisoraMensaje.GetRuta(RedCom.AntenaReceptora) == _antenaDestinoPing;

                _pingMensaje = "---INFORMACIÓN EN DESTINO / ANTENA #" + _antenaDestinoPing.Id + " ---\n";
                int anchoMensaje = _pingMensaje.Count();
                if (_pingExitoso)
                {
                    RedCom.CalcularRutas(_antenaTransmisoraMensaje);
                    int distancia = _antenaDestinoPing.GetDistancia(_antenaTransmisoraMensaje);
                    if (_antenaDestinoPing.MensajeTrasnmitido) {
                        if (distancia == 0) {
                            _pingMensaje += " ! EMITIENDO MENSAJE !\n";
                        } else {
                            _pingMensaje += "MENSAJE REEMITIDO - Distancia: " + distancia;
                            if (distancia == 1) { _pingMensaje += " salto.\n"; }
                            else { _pingMensaje += " saltos.\n"; }
                        }
                    } else {
                        _pingMensaje += "! RECIBIENDO MENSAJE !\n";
                    }  
                }
                else
                {
                    _pingMensaje += "El mensaje no paso por aquí.\n";
                }

                for (int i = 0; i < anchoMensaje - 1; i++) {
                    _pingMensaje += "-";
                }
                
                _antenaDestinoPing = _antenaOrigenPing;
            }
        }
    }

    private void MensajeEnDestino()
    {
        
    }

    private void ResultadoPing()
    {
        MensajePing.text = _pingMensaje;
    }
}
