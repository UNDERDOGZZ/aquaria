# Acuaria

Acuaria es un proyecto mobile-first de simulación de acuarios y aquascaping para
futuras versiones en Android e iOS. Incluye un acuario navegable y una población
provisional de tres especies; todavía no contiene necesidades, agua, guardado ni UI funcional.

## Requisitos

- Unity `6000.5.5f1` (revisión `d16e074b49fd`).
- Unity Hub recomendado.
- No instalar ni actualizar paquetes al abrir el proyecto.

## Abrir el proyecto

1. En Unity Hub, elegir **Add > Add project from disk**.
2. Seleccionar la carpeta `Aquaria` que contiene `Assets`, `Packages` y `ProjectSettings`.
3. Abrir con Unity 6000.5.5f1.

## Estructura principal

- `Assets/_Acuaria`: contenido propio organizado por disciplina y módulo.
- `Assets/_Acuaria/Scenes`: `Bootstrap`, `MainMenu` y `Aquarium`.
- `Assets/_Acuaria/Prefabs`: tanque provisional y rig reutilizable de cámara.
- `Assets/_Acuaria/Scripts`: código runtime bajo namespaces `Acuaria.*`.
- `Assets/_Acuaria/Editor`: herramientas que no forman parte del juego.
- `Assets/_Acuaria/Tests`: assemblies separados para EditMode y PlayMode.
- `docs`: visión, diseño provisional, arquitectura, arte, roadmap y estándares.

Los assets de muestra y la configuración URP generados por la plantilla se conservan.

## Estado actual

Sprint 4 implementado: 14 peces de tres especies usan identidad individual,
profundidad preferida, orientación estable y separación ligera. Consulta
`docs/08_FishSystem.md` y `docs/09_MultiFishAndSpecies.md`.

## Colaborar

1. Leer `docs/06_CodexInstructions.md` y el documento del área a modificar.
2. Revisar cambios locales antes de trabajar.
3. No cambiar paquetes ni configuración global sin aprobación.
4. Mantener cambios pequeños y código bajo namespaces `Acuaria`.
5. Ejecutar compilación y pruebas disponibles antes de proponer un commit.
