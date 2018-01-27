# Anthena-jam
Juego Unity para la Hackathon GameJam 2018

## Prospecto del juego:
Juego de simulación de control de redes en perspectiva top-down 2D, inspirado en el software Packet Tracer. El jugador intentara evitar que un mensaje encriptado llegue a destino, adivinando cual es este.

## Objetivos:
#### Objetivo corto plazo:
El jugador debe interceptar un mensaje encriptado antes de que llegue al receptor, desactivando las antenas de su camino. 

#### Objetivo mediano plazo:
Lograr la mayor puntuación posible.

#### Objetivo largo plazo:
Revear historia por medio de los mensajes desencriptados (Opcional en desarrollo).

## Al dibujarse el mapa:
- Las _antenas_ son colocadas con _uniones_ aleatorias entre si.
- Algunas de estas _uniones_ estan rotas y no pueden enviarse mensajes a traves de ellas, pero esto es invisible al jugador.
- Internamente el programa plantea una ruta invisible entre la antena inicial (Emisora) y la final (Receptora).
- Todas las _antenas_, menos la emisora, inician desactivadas. La emisora comenzara a parpadear intermitentemente por la cantidad de        distancia que esta el mensaje de ella (inicialmente 1, debido a que estara en camino a la primera antena).

## Gameplay
- El jugador puede clickear sobre cualquier antena desactivada. 
- Al clickearla, _pingeara_ hacia ella. Se perdera un segundo (1) por cada trayecto a recorrer (Si esta a una antena de distancia, perdera dos segundos, uno de ida y uno de vuelta). Esto hace que pingear a antenas mas lejanas tarde mas, aumentando el riesgo que conlleva un mal pingado.
### Al pingear puede pasar una de las siguientes cosas:   
 - Si la ruta , avisa que el camino esta libre.
 - Si la ruta entre la ultima antena comprobada como parte del camino y la antena pingeada esta rota, avisa que el camino esta roto.
 - Si la ruta de comunicación actual pasa por esta antena, avisa que esta ocupada.
   - En este ultimo caso avisara si ya ha pasado por alli (Aparece un 1, si esta siendo transmitido desde alli) o si aun no ha pasado        (Aparece un 0).
   - En este ultimo caso se puede anular la antena, si se disponen de tokens de anulación.
   
### Final de fase
La fase termina una vez:
- El mensaje llega a a terminal. Ocurrira un strike.
- El mensaje no puede llegar a la terminal (Todos los caminos estan bloqueados). El jugador recibe 2 puntos.
- El mensaje no puede llegar a la terminal (La terminal ha sido bloquedada). El jugador recibe 10 puntos.
