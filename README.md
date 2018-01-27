# Anthena-jam
Juego Unity para la Hackathon GameJam 2018

## Prospecto del juego:
Juego de simulación de control de redes en perspectiva top-down 2D, inspirado en el software Packet Tracer. El jugador intentara evitar que un mensaje encriptado llegue a destino, adivinando cual es este.

## Objetivos:
#### Objetivo corto plazo:
El jugador debe interceptar un mensaje encriptado antes de que llegue al receptor, desactivando las antenas de su camino hasta, eventualmente, desactivar la final o impedir que hayan caminos hasta esta. 

#### Objetivo mediano plazo:
Lograr la mayor puntuación posible y compartirla en redes sociales (Ñaca ñaca).

#### Objetivo largo plazo:
Revelar historia por medio de los mensajes desencriptados (Opcional en desarrollo).

## Al dibujarse el mapa tener en cuenta:
- Las _antenas_ son colocadas con _uniones_ aleatorias entre si.
- Algunas de estas _uniones_ estan rotas y no pueden enviarse mensajes a traves de ellas. Esto es invisible al jugador.
- Internamente el programa plantea una ruta invisible entre la antena inicial (Emisora) y la final (Receptora). En la esquina superior derecha se mostrara la cantidad de antenas por las que debe pasar para completar la ruta.
- Todas las _antenas_, menos la emisora, inician desactivadas. La emisora comenzara a parpadear intermitentemente por la cantidad de        distancia que esta el mensaje de ella (inicialmente 1, debido a que estara en camino a la primera antena). Alternativamente, podra      aparecer el numero debajo de la antena.
- El mensaje viajara a una velocidad fija (10 segundos por antena). Ej: En una ruta que deba pasar por cuatro antenas, el mensaje tardara 40 segundos (AE -> A1 -> A2 -> A3 -> AR).

      Ejemplo de juego (Lease junto con la imagen "Anthena - Ejemplo"):
      - Las antenas y uniones se generan aleatoriamente, dejando el mapa de ejemplo. Las uniones AE - AD, AG - AJ, AM - AP & AN - AO son las rotas.
      - Se designa a AO como emisor y a AA como receptor. 
      - Se genera la mejor ruta invisible (AO -> AL -> AI -> AF -> AC -> AA). Se muestra el numero 5 en la esquina superior derecha.
      - Se enciende la antena AO y se muestra con un 1.
      - El viaje sin interrupciones tardara 50 segundos.

## Gameplay
- El jugador puede clickear sobre cualquier antena desactivada (o aparentemente desactivada). 
- Al clickearla, _pingeara_ hacia ella. Se perdera un segundo (1) por cada trayecto a recorrer desde la antena emisora (Si esta a una antena de distancia, perdera dos segundos, uno de ida y uno de vuelta). Esto hace que pingear a antenas mas lejanas tarde mas, aumentando el riesgo que conlleva un mal pingado.
### Al pingear puede pasar una de las siguientes cosas:   
 - Si la ruta no pasa por ahi, avisa que el camino esta libre. Puede pasar por alli a futuro, pero no ahora.
 - Si el camino entre la ultima antena comprobada como parte de la ruta y la antena pingeada esta rota, avisa que el camino esta roto.
 - Si la ruta de comunicación (Espacio entre la ultima antena y la proxima) actual pasa por esta antena, avisa que esta ocupada.
   - En este ultimo caso avisara si ya ha pasado por alli (Aparece un 1, si esta siendo transmitido desde alli) o si aun no ha pasado        (Aparece un 0). De ambas maneras la antena aparece activa y el jugador gana 1 punto.
   - En el caso de que ya haya pasado los siguientes pingeos pasaran automaticamente por alli.
   - En el caso de que aun no haya pasado se podra anular la antena, si se disponen de tokens de anulación. Si el anular esa antena provoca que el camino deba ser replanteado, el numero de la esquina superior derecha se actualiza 
   
         Ejemplo de juego (Lease junto con la imagen "Anthena - Ejemplo"):
         - Arranco el juego y pasaron 20 segundos. El mensaje se encuentra siendo transmitido entre AI & AF.
         - El jugador pingeo a AN (Recibio que no se puede entregar), y luego pingeo con AR y AP (Tardando 2 segundos en cada uno). Luego pingeo a la AL, pero el mensaje ya habia pasado por alli, por lo que muestra un 1. El numero de AO ya habria pasado a ser 2.
         - Rapidamente ambos numeros suben uno mas, porque el mensaje esta viajando de AI a AF. El jugador pingea a AF y a AC, y luego anula AD (en una confusion) y AF, ya que el mensaje aun no llego alli.
         - Al momento de anular AF, la ruta se debe recalcular. La nueva ruta seria (AO -> AL -> AI -> AJ -> AM -> AA). Esta nueva ruta sigue teniendo 5 antenas, pero tardara 10 segundos mas en terminarse en lo que el mensaje vuelve de AF a AI y retoma la ruta.

### Final de fase
La fase termina una vez que:
- El mensaje llega a la terminal. Esto dara un strike al jugador. Si se tiene 3 strikes, el juego termina.
- El mensaje no puede llegar a la terminal (Todos los caminos estan bloqueados). El jugador recibe 3 puntos.
- El mensaje no puede llegar a la terminal (La terminal ha sido bloquedada). El jugador recibe 15 puntos.

Una vez terminada la fase, se elegira a otro emisor y receptor. Luego de 5 fases se ofrecera la opcion de terminar el juego o seguir jugando. Si se sigue, se redibujaran las uniones.

### Fin del juego
Si el jugador recibe tres strikes o elige terminar el juego luego de 5 fases, el juego termina. Se lo lleva a una pantalla de resumen de puntaje. Alli deberan aparecer los siguientes valores:
 - Puntaje.
 - Cantidad de strikes.
 - Cantidad de receptoras destruidas.
 - Cantidad de mensajes bloqueados (Que no pudieron llegar a la terminal por caminos bloqueados).
 
 Se debe ofrecer la opcion de compartir el puntaje (Y el juego) en las redes sociales.
