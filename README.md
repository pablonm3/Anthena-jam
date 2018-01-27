# Anthena-jam
Juego Unity para la Hackathon GameJam 2018

## Prospecto del juego:
Juego de simulación de control de redes, inspirado en el software Packet Tracer. El jugador intentara capturar 

## Objetivos:
#### Objetivo corto plazo:
El jugador debe interceptar un mensaje encriptado antes de que llegue al receptor, desactivando las antenas de su camino. 

#### Objetivo mediano plazo:
Lograr la mayor puntuación posible.

#### Objetivo largo plazo:
Historia revelada por los mensajes desencriptados.

### Al dibujarse el mapa:
- Las _antenas_ son colocadas con _uniones_ aleatorias entre si. 
   - Internamente el programa plantea una ruta invisible entre la antena inicial (Emisora) y la fina (Receptora).
   - Todas las _antenas_, menos la inicial, inician desactivadas. La inicial comenzara a parpadear intermitentemente con la cantidad de        distancia que esta el mensaje de ella (inicialmente 1, debido a que estara en camino a la primera antena).
    - Algunas de estas _uniones_ estan rotas y no pueden enviarse mensajes.


El jugador puede clickear sobre cualquier antena desactivada. 
Al clickearla, pingeara hacia ella, perdiendo la misma cantidad de segundos de la distancia (Si esta a 1 de distancia, perdera 2).
Si pingea y no pasa nada, es que esta libre (El mensajeno pasa por alli).
Si pingea y no se llego a destino (El camino esta roto)
Si pingea y esta ocupada (El mensaje pasara o paso por ahi).
En este ultimo caso se puede anular 


Si el jugador logra anular al receptor, gana 10 puntos. Si el mensaje tiene imposibilitado el arribo al receptor, gana 1 punto.
