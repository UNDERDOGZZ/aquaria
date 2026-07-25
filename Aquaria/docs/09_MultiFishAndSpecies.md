# Múltiples peces y especies

## Arquitectura

`FishSpecies` contiene datos estáticos y un prefab visual. Cada `Fish` conserva
identidad, especie, semilla, escala, velocidades, rumbo, destino, pitch y proximidad
a paredes. `FishSpawnPlanner` crea planes reproducibles; `FishSpawner` instancia una
vez; `FishRegistry` expone vecinos activos sin búsquedas globales.

## Spawner e identidad

La escena configura tres grupos: 8 Neon Tetra, 4 Guppy y 2 Angelfish. Cada grupo
tiene semilla y separación inicial. La identidad combina especie, grupo, índice y
semilla; no depende del nombre del GameObject. Los peces se agrupan en `RuntimeFish`.

## Destinos y profundidad

Los destinos favorecen avance horizontal, aplican yaw moderado, limitan la diferencia
vertical y restringen Y al rango de profundidad normalizado de la especie. Cerca de
paredes el rumbo se sesga al centro sin reflejar velocidad ni recalcular cada frame.

## Orientación

`FishRoot` mueve y orienta con forward `+Z` y up mundial `+Y`. La dirección vertical
se limita antes de crear un quaternion mediante `LookRotation(direction, Vector3.up)`.
El hijo `Visual` aplica oscilación barata y un bank visual pequeño que nunca modifica
la trayectoria. Para corregir otro modelo, rota su hijo `Visual`; no cambies el root.

## Separación y rendimiento

Cada pez recorre la lista preasignada del registro y suma contribuciones solo dentro
de su radio. La fuerza se limita. Para 14–20 peces, O(n²) es simple y suficiente;
no hay colliders, Rigidbody, OverlapSphere, LINQ, `FindObjectOfType`, materiales por
instancia ni allocations deliberadas en `Update`. `MaterialPropertyBlock` aplica color.

## Crear una especie

1. Duplica un asset en `Data/Fish`.
2. Asigna un `speciesId` único y nombre.
3. Asigna un prefab con `FishMovement`.
4. Ajusta escalas, velocidades, aceleración, giro y profundidad.
5. Limita variación vertical y ángulos de ascenso/descenso.
6. Añade un grupo en `FishSpawner` con cantidad y semilla.

## Parámetros verticales

`Preferred Depth Minimum/Maximum` son fracciones 0–1 de la altura segura del tanque.
`Maximum Vertical Variation` limita cada cambio de altura. Los ángulos máximos evitan
trayectorias casi verticales e inversión.

## Limitaciones y futuro

La separación no es cardumen y no implementa compatibilidad, hambre, salud,
reproducción ni guardado. La política O(n²) deberá reevaluarse solo si la población
crece de forma significativa.

## Prueba manual

1. Entra en Play Mode y confirma 14 hijos bajo `RuntimeFish`.
2. Observa dos minutos desde frente y lateral.
3. Verifica tres colores/tamaños y ritmos distintos.
4. Comprueba techo, suelo, esquinas, pitch y ausencia de roll permanente.
5. Activa gizmos en un pez y revisa destino, rumbo y radio de separación.
6. Cambia temporalmente la suma de grupos a 20 y repite sin errores rojos.

## Población con modelos reales

La escena actual sustituye la población inicial anterior por 6 Guppy, 4 Clownfish y
2 Neon Tetra placeholder. Guppy y Clownfish usan adaptadores animados bajo
`_Acuaria/Prefabs/Fish`; el placeholder conserva una referencia de depuración.
La lógica multi-especie, el registro y la separación no cambian.
