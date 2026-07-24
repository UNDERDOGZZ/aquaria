# Cámara e input

## Arquitectura

`PointerInputReader` captura dispositivos del Input System y produce un
`CameraInputState` por frame. `AquariumCameraController` interpreta ese estado,
limita sus objetivos mediante `CameraMotionMath` y actualiza el rig con suavizado
independiente del framerate. `AquariumBounds` y `AquariumVolume` describen el tanque
sin conocer la cámara.

## Controles

### Mouse en Editor

- Arrastre izquierdo: órbita horizontal y vertical.
- Arrastre derecho o central: pan horizontal y vertical limitado.
- Rueda: zoom suavizado.

### Touch

- Arrastre con un dedo: órbita.
- Arrastre paralelo con dos dedos: pan.
- Pinza con dos dedos: zoom.

Al cambiar la cantidad de dedos se descarta el primer frame para evitar saltos.

## Límites configurables

En `AquariumCameraController > Config` pueden ajustarse velocidades horizontal y
vertical, sensibilidades de zoom y pan, suavizado, distancias mínima/máxima, ángulos
horizontal/vertical, límites de pan, posición de respaldo, rotación y distancia inicial.
Los valores de ejecución no se guardan en la configuración.

## Jerarquía esperada

```text
AquariumScene
├── Environment
│   └── AquariumTank
│       ├── AquariumInterior
│       ├── Base
│       ├── Background
│       └── Frames
├── CameraRig
│   └── CameraPivot
│       └── Main Camera
├── Lighting
│   └── Main Light
└── Systems
```

Algunas piezas del marco son hermanos individuales en el prefab para facilitar su
ajuste; no existe un contenedor `Frames` adicional.

## Ajuste y reset

Selecciona `CameraRig` y modifica `Config` en el Inspector. Para restablecer durante
Play Mode, abre el menú contextual de `AquariumCameraController` y elige
**Reset Camera**. El método público `ResetCamera()` queda disponible para una UI futura.

## Editor frente a móvil

Ambos entornos alimentan el mismo estado neutral de cámara. El mouse asigna botones a
gestos; móvil usa uno o dos contactos. No se cambia la orientación desde código y el
prototipo está encuadrado para formato horizontal.

Una UI futura deberá usar un Canvas y posicionar sus paneles dentro de
`Screen.safeArea`. El lector podrá ignorar punteros sobre UI cuando exista esa capa;
no se introduce todavía una dependencia de UI.

## Decisiones y limitaciones

- Configuración embebida en Inspector en lugar de ScriptableObject global.
- Sin Cinemachine, shaders, colliders ni postprocesado.
- La cámara orbita solo frente al tanque y el pan tiene alcance pequeño.
- No hay exclusión de gestos sobre UI porque aún no existe UI interactiva.
- La sensibilidad debe validarse en dispositivos de diferentes densidades de pantalla.

## Prueba manual

1. Abre `Aquarium.unity` y entra en Play Mode.
2. Comprueba órbita, zoom y pan con mouse, incluyendo sus extremos.
3. En el Inspector ejecuta **Reset Camera** y confirma el encuadre inicial.
4. Cambia repetidamente entre uno y dos contactos en un dispositivo y comprueba que
   no haya saltos.
5. Verifica en formato horizontal que el tanque nunca desaparezca completamente.
6. Ejecuta los tests EditMode desde Test Runner.
