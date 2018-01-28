using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Estacion
{
    public int Id { get; set; }
    public float X { get; set; }
    public float Y { get; set; }

    public Antena AntenaPing;

    public Estacion(int id, Antena antenaPing)
    {
        Id = id;
        AntenaPing = antenaPing;
    }
}
