using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Persistence;
using Random = System.Random;

public class Grafo
{
    private const int MaxIntentosPonerNodo = 1000;

    private const double DistanciaDeConexion = 3;
    private const double MinDistanciaEntreNodos = 0.5;

    public static Grafo GenerarGrafo(Random random, int numeroNodos, float anchoMapa, float altoMapa)
    {
        Grafo grafoGenerado = new Grafo();

        for (int i = 0; i < numeroNodos; i++)
        {
            int idNodo = 0;
            Nodo nuevoNodo = new Nodo(idNodo++);
            bool posicionExitosa = false;
            while (!posicionExitosa && i < MaxIntentosPonerNodo)
            {
                nuevoNodo.X = anchoMapa * ((float) random.NextDouble() - 0.5f);
                nuevoNodo.Y = altoMapa  * ((float) random.NextDouble() - 0.5f);

                posicionExitosa = true;
                foreach (Nodo nodo in grafoGenerado.Nodos)
                {
                    double dX = nuevoNodo.X - nodo.X;
                    double dY = nuevoNodo.Y - nodo.Y;
                    float distancia = Mathf.Sqrt((float)(dX * dX + dY * dY));
                    if (distancia < MinDistanciaEntreNodos)
                    {
                        posicionExitosa = false;
                        break;
                    }
                }

                if (posicionExitosa) {
                    break;
                }
                i++;
            }

            if (posicionExitosa)
            {
                grafoGenerado.Nodos.Add(nuevoNodo);
            }
        }

        foreach (Nodo n in grafoGenerado.Nodos)
        {
            foreach (Nodo m in grafoGenerado.Nodos)
            {
                double dX = n.X - m.X;
                double dY = n.Y - m.Y;
                float distancia = Mathf.Sqrt((float)(dX * dX + dY * dY));
                if (distancia < DistanciaDeConexion)
                {
                    n.Conexiones.Add(m);
                    m.Conexiones.Add(n);
                }
            }
        }

        return grafoGenerado;
    }

    public ICollection<Nodo> Nodos { get; set; }

    public Grafo()
    {
        Nodos = new List<Nodo>();
    }
};