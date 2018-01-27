using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Antena
{
    public int Id { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public ICollection<Antena> Conexiones { get; set; }

    public bool MensajeTrasnmitido { get; set; }

    public bool Visitada { get; set; }

    public Antena CaminoHaciaElReceptor;
    public int DistanciaAlReceptor;

    public Antena(int id)
    {
        Id = id;
        Conexiones = new List<Antena>();
    }
}
