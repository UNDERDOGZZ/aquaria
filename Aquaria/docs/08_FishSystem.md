# Sistema del primer pez

## Objetivo

Sprint 3 incorporó el primer pez. Sprint 4 conserva ese movimiento y lo amplía a
múltiples especies e instancias, sin añadir necesidades ni comportamiento social.

## Arquitectura

- `FishSpecies` contiene nombre, velocidades, intervalo de cambio de dirección,
  radio de giro, tamaño y color provisional.
- `FishMovementSettings` copia esos valores a una estructura inmutable para simulación.
- `Fish` contiene exclusivamente estado dinámico.
- `FishMovementLogic` calcula destinos, dirección, velocidad y posición.
- `FishMovement` conecta la lógica con el transform y dibuja gizmos opcionales.
- `FishSpawner` materializa grupos de especies y los conecta al volumen y registro.
- `FishRegistry` permite separación local sin búsquedas globales.
- `FishOrientationLogic` limita pitch y elimina roll lógico.

## Flujo

1. `FishSpawner` recibe especie, prefab y `AquariumVolume`.
2. Crea planes deterministas para cada grupo al comenzar la escena.
3. `FishMovement` crea el estado inicial en el centro del volumen seguro.
4. La lógica selecciona un destino cercano, con pequeñas variaciones respecto a la
   dirección actual.
5. Cerca de un borde, el nuevo rumbo se inclina hacia el centro.
6. El giro se limita usando velocidad y radio de giro.
7. La posición final siempre se restringe a un volumen reducido por el tamaño corporal.
8. La separación suave evita superposición visual sin convertirse en cardumen.

## Placeholder visual

El prefab usa una esfera alargada, una cola y dos ojos. El eje local `+Z` representa
el frente, señalado por los ojos. No contiene colliders, animaciones, partículas ni audio.

## Configuración

Cada asset de especie permite ajustar identidad, prefab, escala, aceleración,
profundidad, límites verticales, separación y parámetros visuales además de:

- nombre visible;
- velocidad mínima y máxima;
- tiempo mínimo y máximo entre cambios;
- radio de giro;
- tamaño;
- color provisional.

Modificar el asset no altera el estado actual de una partida: el estado vive en `Fish`.

## Debug

Activa `Show Debug Gizmos` en `FishMovement` para mostrar dirección, destino y radio
de detección. `AquariumVolume` ya muestra los límites al seleccionarlo.

## Futuras extensiones

La separación permite añadir más especies y una política de múltiples instancias sin
cambiar la lógica básica. Hambre, salud, reproducción, cardúmenes, animación y
simulación del agua quedan explícitamente fuera de este sprint.

## Prueba manual

1. Abre `Aquarium.unity` y entra en Play Mode.
2. Confirma que `FishSpawner` crea 14 peces de tres especies.
3. Obsérvalo varios minutos y verifica que gira gradualmente.
4. Comprueba todos los laterales, techo y suelo del tanque.
5. Activa gizmos y observa dirección, destino y anticipación de bordes.
6. Cambia velocidades o radio de giro en `PrototypeFish.asset`, reinicia Play Mode
   y compara el resultado.
7. Ejecuta la suite EditMode desde Test Runner.
