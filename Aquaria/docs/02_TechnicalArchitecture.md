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

## Decisiones de la fundación

- `ApplicationBootstrap` es el único componente runtime: verifica que vive en
  `Bootstrap` y no navega ni conserva estado global.
- Las escenas se generan con API de Editor para evitar edición manual frágil de YAML.
- La escena de muestra y la configuración URP de la plantilla se conservan sin cambios.
- Los managers sugeridos se posponen: sin casos de uso actuales serían clases vacías.
