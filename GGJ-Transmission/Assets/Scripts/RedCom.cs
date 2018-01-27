using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Graphs;
using UnityEngine;
using Random = System.Random;

public class RedCom
{
    private const int DistanciaSinConexion = int.MaxValue;
    private const int GenAleatoriaMaxIntentosPonerNodo = 1000;
    private const float GenAleatoriaDistanciaDeConexion = 3;
    private const float GenAleatoriaMinDistanciaEntreNodos = 0.5f;

    public ICollection<Antena> Antenas { get; set; }
    public Antena AntenaEmisora;
    public Antena AntenaReceptora;

    public RedCom()
    {
        Antenas = new List<Antena>();
    }

    public void Generar(Random random)
    {
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

        // GenerarGrafoAleatorio(Random, 15, width - 1, height - 1);
        GenerarGrafoRectangular(random, 10, 10, width - 1, height - 1);

        AntenaReceptora = Antenas.ElementAt(random.Next(Antenas.Count));
        CalcularCaminosMinimos(AntenaReceptora);

        AntenaEmisora = ElegirAntenaEmisora(random, 7, 10);
    }

    public void RecalcularRuta()
    {
       CalcularCaminosMinimos(AntenaReceptora); 
    }

    public void DestruirAntena(Antena antenaADestruir)
    {
        Antenas.Remove(antenaADestruir);
        foreach (var a in Antenas) {
            a.Conexiones.Remove(antenaADestruir);
        }

        RecalcularRuta();
    }

    private void GenerarGrafoAleatorio(Random random, int numeroNodos, float anchoMapa, float altoMapa)
    {
        int idNodo = 0;
        for (int i = 0; i < numeroNodos; i++)
        {
            Antena nuevaAntena = new Antena(idNodo++);
            bool posicionExitosa = false;
            while (!posicionExitosa && i < GenAleatoriaMaxIntentosPonerNodo)
            {
                nuevaAntena.X = anchoMapa * ((float)random.NextDouble() - 0.5f);
                nuevaAntena.Y = altoMapa * ((float)random.NextDouble() - 0.5f);

                posicionExitosa = true;
                foreach (Antena nodo in Antenas)
                {
                    double dX = nuevaAntena.X - nodo.X;
                    double dY = nuevaAntena.Y - nodo.Y;
                    float distancia = Mathf.Sqrt((float)(dX * dX + dY * dY));
                    if (distancia < GenAleatoriaMinDistanciaEntreNodos)
                    {
                        posicionExitosa = false;
                        break;
                    }
                }

                if (posicionExitosa)
                {
                    break;
                }
                i++;
            }

            if (posicionExitosa)
            {
                Antenas.Add(nuevaAntena);
            }
        }

        foreach (Antena n in Antenas)
        {
            foreach (Antena m in Antenas)
            {
                double dX = n.X - m.X;
                double dY = n.Y - m.Y;
                float distancia = Mathf.Sqrt((float)(dX * dX + dY * dY));
                if (distancia < GenAleatoriaDistanciaDeConexion)
                {
                    n.Conexiones.Add(m);
                    m.Conexiones.Add(n);
                }
            }
        }
    }

    private void GenerarGrafoRectangular(Random random, int anchoGrafo, int altoGrafo, float anchoMapa, float altoMapa)
    {
        float distanciaHorizontal = anchoMapa / (anchoGrafo - 1);
        float distanciaVertical = altoMapa / (altoGrafo - 1);

        int idNodo = 0;
        for (int y = 0; y < altoGrafo; y++)
        {
            for (int x = 0; x < anchoGrafo; x++)
            {
                Antena nuevoAntena = new Antena(idNodo++)
                {
                    X = x * distanciaHorizontal - anchoMapa / 2,
                    Y = -y * distanciaVertical + altoMapa / 2
                };
                Antenas.Add(nuevoAntena);
            }
        }

        for (int y = 0; y < altoGrafo; y++)
        {
            for (int x = 0; x < anchoGrafo; x++)
            {
                int index = y * anchoGrafo + x;
                int indexDerecha = y * anchoGrafo + x + 1;
                int indexAbajo = (y + 1) * anchoGrafo + x;

                Antena antenaActual = Antenas.ElementAt(index);
                ICollection<Antena> conexiones = antenaActual.Conexiones;

                if (random.Next(4) == 0)
                {
                    continue;
                }

                if (x + 1 < anchoGrafo)
                {
                    conexiones.Add(Antenas.ElementAt(indexDerecha));
                    Antenas.ElementAt(indexDerecha).Conexiones.Add(antenaActual);
                }


                if (y + 1 < altoGrafo)
                {
                    conexiones.Add(Antenas.ElementAt(indexAbajo));
                    Antenas.ElementAt(indexAbajo).Conexiones.Add(antenaActual);
                }
            }
        }
    }

    private Antena ElegirAntenaEmisora(Random random, int distanciaMinima, int distanciaMaxima)
    {
        ICollection<Antena> antenasCandidatas = Antenas.Where(a => a.DistanciaAlReceptor >= distanciaMinima && a.DistanciaAlReceptor <= distanciaMaxima).ToList();
        if (antenasCandidatas.Count == 0)
        {
            antenasCandidatas = Antenas.Where(a => a.DistanciaAlReceptor > distanciaMinima).ToList();
            if (antenasCandidatas.Count == 0)
            {
                antenasCandidatas = Antenas;
            }
        }

        return antenasCandidatas.ElementAt(random.Next(antenasCandidatas.Count));
    }

    private void CalcularCaminosMinimos(Antena antenaReceptora)
    {
        foreach (var antena in Antenas) {
            antena.Visitada = false;
            antena.CaminoHaciaElReceptor = null;
            antena.DistanciaAlReceptor = DistanciaSinConexion;
        }

        List<Antena> antenasPorVisitar = new List<Antena>();
        antenaReceptora.DistanciaAlReceptor = 0;
        antenaReceptora.Visitada = true;

        antenasPorVisitar.Add(antenaReceptora);
        while (antenasPorVisitar.Any()) {
            Antena antenaActual = antenasPorVisitar.First();
            antenasPorVisitar.RemoveAt(0);
            antenaActual.Visitada = true;

            foreach (var conexion in antenaActual.Conexiones) {
                if (conexion.Visitada) {
                    continue;
                }

                int distanciaConexion = antenaActual.DistanciaAlReceptor + 1;
                if (distanciaConexion < conexion.DistanciaAlReceptor) {
                    conexion.DistanciaAlReceptor = distanciaConexion;
                    conexion.CaminoHaciaElReceptor = antenaActual;
                }

                if (!conexion.Visitada) {
                    antenasPorVisitar.Add(conexion);
                }
            }
        }
    }
};