# Arquitectura técnica

## Módulos previstos

- `Core`: entrada y coordinación de alto nivel.
- `Simulation`: reglas del ecosistema independientes de presentación cuando sea posible.
- `Environment`, `Fish` y `Plants`: capacidades de dominio y sus adaptadores Unity.
- `Save`: persistencia offline futura.
- `UI` e `Input`: presentación e interacción sin contener reglas de simulación.
- `Utilities`: utilidades pequeñas, generales y justificadas por uso real.

## Límites y dependencias

La dirección deseada es `UI/Input -> aplicación -> dominio`. El dominio no debe
depender de UI ni de escenas. La representación visual consume estado de simulación,
pero no lo define. Las dependencias entre módulos deberán ser explícitas y mínimas.

Los `ScriptableObject` se reservarán para datos configurables; no almacenarán estado
mutable de una partida. Se evitarán singletons globales, service locators, eventos
globales y managers con responsabilidades heterogéneas.

## Estructura

Los assets propios viven en `Assets/_Acuaria`, separados por arte, audio, datos,
prefabs, escenas, scripts, settings, tests y UI. Los assets de la plantilla permanecen
fuera de esta raíz hasta que exista una decisión explícita sobre ellos.

## Separación de assemblies

- `Acuaria.Runtime`: código que puede formar parte del juego.
- `Acuaria.Editor`: herramientas limitadas al Editor y dependientes de Runtime.
- `Acuaria.Tests.EditMode` y `Acuaria.Tests.PlayMode`: límites preparados para pruebas,
  excluidos de builds mediante referencias de tests.

Se usa un solo assembly runtime para evitar fragmentación prematura.

## Cámara y prototipo de acuario

El Sprint 2 añade tres capas separadas:

- `PointerInputReader` traduce mouse o touch a `CameraInputState`, sin mover objetos.
- `AquariumCameraController` consume ese estado, aplica configuración y representa la vista.
- `AquariumBounds` modela el volumen sin depender de cámara, input ni escenas.

`CameraMotionMath` contiene límites deterministas que pueden probarse en EditMode.
`AquariumVolume` adapta los límites locales de un tanque a coordenadas de mundo.
La configuración de cámara se serializa dentro del componente; no se crea un asset
global porque actualmente existe una sola cámara y el estado de ejecución permanece
separado de la configuración.

## Primer pez

El sistema del Sprint 3 mantiene separadas cuatro responsabilidades:

- `FishSpecies`: `ScriptableObject` con datos configurables de una especie; nunca
  contiene estado de una partida.
- `Fish`: estado dinámico de una instancia, incluyendo posición, dirección,
  velocidad, destino y temporizador.
- `FishMovementLogic`: movimiento puro y testeable, selección acotada de destino,
  giro gradual y permanencia dentro de `AquariumBounds`.
- `FishMovement` y `FishSpawner`: adaptación al ciclo de vida de Unity,
  representación visual e instanciación de un solo ejemplar.

La lógica consume `FishMovementSettings`, una copia inmutable derivada de
`FishSpecies`. Esto evita que la simulación escriba sobre el asset y permite probarla
sin escenas ni tiempo real. El spawner queda preparado para cambiar su política en el
futuro, pero el Sprint 3 limita deliberadamente la población a una instancia.

## Múltiples peces y especies

Sprint 4 amplía el mismo modelo: `FishSpecies` define configuración y prefab,
`Fish` conserva identidad y estado por instancia, `FishSpawnPlanner` produce planes
deterministas y `FishSpawner` los materializa. `FishRegistry` es una dependencia de
escena pequeña que mantiene una lista de solo lectura para separación; no contiene
movimiento ni es singleton.

La orientación estable vive en `FishOrientationLogic`. El objeto raíz mantiene
`Vector3.up`, mientras `Visual` aplica únicamente oscilación y bank visual. La
separación usa un recorrido directo sobre hasta 20 peces, sin física, LINQ ni
allocations por frame.

## Integración visual Fish Alive

La integración mantiene el límite de propiedad de Sprint 4. El prefab raíz de cada
pez pertenece a Acuaria y contiene `FishMovement`; su hijo `Visual` aloja el rig,
`SkinnedMeshRenderer` y `Animator` procedentes de Fish Alive.
`FishAnimationController` traduce la velocidad simulada al parámetro visual
`swimSpeed`. No calcula movimiento, evasión, navegación ni orientación.

Los prefabs fuente de `Assets/DenysAlmaral/FishAlive` son dependencias de solo lectura.
Los `FishSpecies` apuntan a adaptadores bajo `_Acuaria`, nunca a prefabs externos.

## Decisiones de la fundación

- `ApplicationBootstrap` es el único componente runtime: verifica que vive en
  `Bootstrap` y no navega ni conserva estado global.
- Las escenas se generan con API de Editor para evitar edición manual frágil de YAML.
- La escena de muestra y la configuración URP de la plantilla se conservan sin cambios.
- Los managers sugeridos se posponen: sin casos de uso actuales serían clases vacías.
