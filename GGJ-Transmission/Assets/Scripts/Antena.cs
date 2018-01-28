using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Antena
{
    public int Id { get; set; }
    public RedCom Red;
    public float X { get; set; }
    public float Y { get; set; }
    public HashSet<Antena> Conexiones { get; set; }

    public bool MensajeTrasnmitido { get; set; }

    public bool Visitada { get; set; }
    public bool AntenaGrande { get; set; }
    public int Grupo = -1;

    private readonly Dictionary<int, Antena> _tablaDeRuteo = new Dictionary<int, Antena>();
    private readonly Dictionary<int, int> _tablaDeDistancias = new Dictionary<int, int>();

    public Antena(int id, RedCom redCom)
    {
        Id = id;
        Red = redCom;
        Conexiones = new HashSet<Antena>();
    }

    public int GetDistancia(Antena destino)
    {
        if (!_tablaDeRuteo.ContainsKey(destino.Id))
        {
            Red.CalcularRutas(destino);
        }
        return _tablaDeDistancias[destino.Id];
    }

    public Antena GetRuta(Antena destino)
    {
        if (!_tablaDeRuteo.ContainsKey(destino.Id))
        {
            Red.CalcularRutas(destino);
        }
        return _tablaDeRuteo[destino.Id];
    }

    public void SetDistancia(Antena destino, int distancia)
    {
        _tablaDeDistancias[destino.Id] = distancia;
    }

    public void SetRuta(Antena destino, Antena ruta)
    {
        _tablaDeRuteo[destino.Id] = ruta;
    }
}