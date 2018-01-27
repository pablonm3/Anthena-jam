using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nodo
{
    public int Id { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public ICollection<Nodo> Conexiones { get; set; }

    public Nodo(int id)
    {
        Id = id;
        Conexiones = new List<Nodo>();
    }
}
