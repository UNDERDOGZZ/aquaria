# Integración Fish Alive

## Alcance

Se integran las muestras `freshWater_guppy` y `marine_clownfish` como apariencia,
rig y animación. La simulación continúa siendo propiedad exclusiva de Acuaria:
`FishSpawner`, `FishRegistry`, `FishMovement`, límites, profundidad, separación y
orientación no dependen de la lógica runtime del paquete.

`Assets/DenysAlmaral/FishAlive` se trata como solo lectura. No se modificaron sus
prefabs, scripts, materiales, meshes, animaciones ni metadatos.

## Adaptadores

- `Fish_Guppy.prefab`
- `Fish_Clownfish.prefab`

Cada adaptador usa esta jerarquía:

```text
Fish_<Species>               (+Z forward, +Y up)
├── FishMovement
├── FishAnimationController
└── Visual                   (posición 0, rotación identity, escala 1)
    └── FishAliveModel
        ├── Animator         (applyRootMotion = false)
        └── rig + SkinnedMeshRenderer
```

Los modelos fuente ya emplean raíz `+Z` y escala de importación 1, por lo que no se
aplica corrección de rotación ni escala en `Visual`. La escala final proviene
únicamente del rango de cada `FishSpecies`: Guppy `0.80–1.00`, Clownfish `0.90–1.12`.
Esto evita multiplicar una corrección visual por la escala aleatoria de instancia.

Al crear el adaptador se eliminan todos los `MonoBehaviour` externos, `Rigidbody` y
`Collider`. Se conserva el controlador Animator original y sus clips `idle`, `swim`,
giros y `bite`; el estado usado por el puente es `Swim`. El parámetro `swimSpeed`
recibe la velocidad normalizada de Acuaria, `turn` se mantiene neutral en `0.5`, y
root motion permanece desactivado.

El material compartido Fish Alive usa `Universal Render Pipeline/Lit`, por lo que es
compatible con el pipeline actual. Los renderers reales no reciben el tinte
`MaterialPropertyBlock` del placeholder y conservan textura y color originales.

## Especies y escena

- Guppy: 6 instancias, mayor agilidad y zona de profundidad amplia.
- Clownfish: 4 instancias, ritmo y giro más pausados.
- Neon Tetra placeholder: 2 instancias para conservar una referencia visual de
  depuración y regresión.

Los dos assets reales apuntan a prefabs adaptadores bajo `_Acuaria`. El placeholder
original y Angelfish siguen disponibles como assets, aunque Angelfish no forma parte
de la población inicial.

## Rendimiento móvil

Los adaptadores no añaden física, búsquedas globales ni materiales instanciados.
Cada pez añade un `Animator`, un `SkinnedMeshRenderer` y una actualización pequeña de
dos parámetros. Para la población prototipo de 12 peces se mantiene la separación
O(n²) de Sprint 4. Antes de aumentar notablemente la población se debe perfilar CPU
de Animator, skinning y overdraw en dispositivo objetivo.

## Validación manual

1. Ejecuta `Acuaria > Integrate Fish Alive Samples` si necesitas regenerar assets.
2. Abre `Assets/_Acuaria/Scenes/Aquarium.unity` y entra en Play Mode.
3. Confirma 12 hijos bajo `RuntimeFish`: 6 Guppy, 4 Clownfish y 2 Neon.
4. Comprueba que ambos peces reales muestran textura, deformación del rig y nado.
5. Observa desde frente y lateral que el morro sigue `+Z`, sin desplazamiento doble,
   inversión, roll permanente ni root motion.
6. Revisa techo, suelo y esquinas durante varios minutos.
7. Ejecuta EditMode tests y verifica que los adaptadores no contienen física ni
   `MonoBehaviour` externos.

La validación batch cubre compilación, estructura y referencias. La calidad visual
en movimiento debe confirmarse manualmente en Play Mode y en un dispositivo móvil.
