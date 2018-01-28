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
    private const float GenAleatoriaDistanciaDeConexion = 2.5f;
    private const float GenAleatoriaMinDistanciaEntreNodos = 1.25f;

    public ICollection<Antena> Antenas { get; set; }
    public Antena AntenaEmisora;
    public Antena AntenaReceptora;
    public Antena AntenaEstacion;

    public RedCom()
    {
        Antenas = new List<Antena>();
    }

    public void Generar(Random random)
    {
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

        GenerarGrafoAleatorio(random, 90, -width/2 + 1, width/2 - 1, -height/2 + 2, height/2 - 1);
        // GenerarGrafoRectangular(random, 8, 8, width - 1, height - 1);

        CalcularTodasLasRutas();

        AntenaReceptora = Antenas.ElementAt(random.Next(Antenas.Count));
        AntenaEmisora = ElegirAntenaEmisora(random, 6, 10);
        AntenaEstacion = ElegirAntenaEstacion(random, 3, 3);
    }

    public void DestruirAntena(Antena antenaADestruir)
    {
        Antenas.Remove(antenaADestruir);
        foreach (var a in Antenas) {
            a.Conexiones.Remove(antenaADestruir);
        }

        CalcularTodasLasRutas();
    }

    public void CalcularTodasLasRutas()
    {
        foreach (var antena in Antenas)
        {
            CalcularRutas(antena);
        }
    }

    public void CalcularRutas(Antena antenaDestino)
    {
        foreach (var antena in Antenas)
        {
            antena.Visitada = false;
            antena.SetRuta(antenaDestino, null);
            antena.SetDistancia(antenaDestino, DistanciaSinConexion);
        }

        List<Antena> antenasPorVisitar = new List<Antena>();
        antenaDestino.SetDistancia(antenaDestino, 0);
        antenaDestino.Visitada = true;

        antenasPorVisitar.Add(antenaDestino);
        while (antenasPorVisitar.Any())
        {
            Antena antenaActual = antenasPorVisitar.First();
            antenasPorVisitar.RemoveAt(0);
            antenaActual.Visitada = true;

            foreach (var conexion in antenaActual.Conexiones)
            {
                if (conexion.Visitada)
                {
                    continue;
                }

                int distanciaConexion = antenaActual.GetDistancia(antenaDestino) + 1;
                if (distanciaConexion < conexion.GetDistancia(antenaDestino))
                {
                    conexion.SetDistancia(antenaDestino, distanciaConexion);
                    conexion.SetRuta(antenaDestino, antenaActual);
                }

                antenasPorVisitar.Add(conexion);
            }
        }
    }

    private void GenerarGrafoAleatorio(Random random, int numeroNodos, float x0, float x1, float y0, float y1)
    {
        int idNodo = 0;
        float anchoMapa = Math.Abs(x1 - x0);
        float altoMapa  = Math.Abs(y1 - y0);
        for (int i = 0; i < numeroNodos; i++)
        {
            Antena nuevaAntena = new Antena(idNodo++, this);
            bool posicionExitosa = false;
            while (!posicionExitosa && i < GenAleatoriaMaxIntentosPonerNodo)
            {
                nuevaAntena.X = anchoMapa * (float) random.NextDouble() + x0;
                nuevaAntena.Y = altoMapa  * (float) random.NextDouble() + y0;

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

        UnirGrupos();
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
                Antena nuevoAntena = new Antena(idNodo++, this)
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
        ICollection<Antena> antenasCandidatas = Antenas.Where(a => a.GetDistancia(AntenaReceptora) >= distanciaMinima && a.GetDistancia(AntenaReceptora) <= distanciaMaxima).ToList();
        if (antenasCandidatas.Count == 0)
        {
            antenasCandidatas = Antenas.Where(a => a.GetDistancia(AntenaReceptora) > distanciaMinima).ToList();
            if (antenasCandidatas.Count == 0)
            {
                antenasCandidatas = Antenas;
            }
        }

        return antenasCandidatas.ElementAt(random.Next(antenasCandidatas.Count));
    }

    private Antena ElegirAntenaEstacion(Random random, int distanciaMinimaReceptora, int distanciaMinimaEmisora)
    {
        ICollection<Antena> antenasCandidatas = Antenas.Where(a => a.GetDistancia(AntenaReceptora) >= distanciaMinimaReceptora && a.GetDistancia(AntenaEmisora) >= distanciaMinimaEmisora).ToList();
        if (antenasCandidatas.Count == 0)
        {
            antenasCandidatas = Antenas;
        }

        return antenasCandidatas.ElementAt(random.Next(antenasCandidatas.Count));
    }

    private void UnirGrupos()
    {
        int cantidadGrupos = int.MaxValue;
        while (cantidadGrupos > 1) {
            foreach (var antena in Antenas) {
                antena.Grupo = -1;
            }
            int numeroDeGrupo = 0;
            foreach (var antena in Antenas)
            {
                if (antena.Grupo != -1)
                {
                    continue;
                }

                List<Antena> grupoDeAntenas = EncontrarGrupo(antena, numeroDeGrupo);
                List<Antena> antenasFueraDelGrupo = Antenas.Where(a => !grupoDeAntenas.Contains(a)).ToList();

                float distanciaMinima = float.MaxValue;
                Antena conexionInterna = null;
                Antena conexionExterna = null;
                foreach (var antenaEnGrupo in grupoDeAntenas)
                {
                    foreach (var antenaOtroGrupo in antenasFueraDelGrupo)
                    {
                        float dX = antenaOtroGrupo.X - antenaEnGrupo.X;
                        float dY = antenaOtroGrupo.Y - antenaEnGrupo.Y;
                        float d2 = dX * dX + dY * dY;

                        if (d2 < distanciaMinima)
                        {
                            distanciaMinima = d2;
                            conexionInterna = antenaEnGrupo;
                            conexionExterna = antenaOtroGrupo;
                        }
                    }
                }

                if (conexionInterna != null) {
                    conexionInterna.Conexiones.Add(conexionExterna);
                    conexionExterna.Conexiones.Add(conexionInterna);
                    conexionInterna.AntenaGrande = true;
                    conexionExterna.AntenaGrande = true;

                    // Agrega la sección conectada al grupo actual
                    EncontrarGrupo(conexionExterna, numeroDeGrupo);
                }

                
                numeroDeGrupo++;
            }
            cantidadGrupos = numeroDeGrupo;
        }
    }

    private List<Antena> EncontrarGrupo(Antena antenaInicial, int numeroDeGrupo)
    {
        foreach (var antena in Antenas)
        {
            antena.Visitada = false;
        }
        List<Antena> antenasPorVisitar = new List<Antena>();
        antenasPorVisitar.Add(antenaInicial);
        List<Antena> antenasEnGrupo = new List<Antena>();

        while(antenasPorVisitar.Any()) {
            Antena antenaActual = antenasPorVisitar.First();
            antenasPorVisitar.RemoveAt(0);
            antenaActual.Visitada = true;
            antenaActual.Grupo = numeroDeGrupo;
            antenasEnGrupo.Add(antenaActual);

            foreach (var conexion in antenaActual.Conexiones) {
                if (conexion.Visitada) {
                    continue;
                }

                antenasPorVisitar.Add(conexion);
            }
        }

        return antenasEnGrupo;
    }
};