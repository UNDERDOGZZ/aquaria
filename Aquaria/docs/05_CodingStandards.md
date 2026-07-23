# Estándares de código

## C# y nombres

- Usar las convenciones de .NET: `PascalCase` para tipos, métodos y propiedades;
  `camelCase` para variables y parámetros; `_camelCase` para campos privados.
- Un tipo principal por archivo y nombre de archivo igual al tipo.
- Todos los tipos propios usan namespaces con prefijo `Acuaria`.
- Preferir `private` y `sealed` cuando la extensión no sea una necesidad real.

## Serialización y datos

- Mantener campos serializados privados con `[SerializeField]`.
- No exponer estado mutable mediante campos públicos.
- Usar `ScriptableObject` para configuración compartida, nunca como partida guardada.
- Validar referencias de Inspector en los límites donde se consumen.

## Dependencias

- Pasar dependencias explícitamente en C# puro y por composición controlada en Unity.
- Evitar singletons, service locators y buses globales por defecto.
- UI no contiene reglas de simulación ni modifica internamente su estado.

## MonoBehaviour

- Usarlo solo cuando se necesite ciclo de vida, escena, coroutine o serialización Unity.
- No crear `Awake`, `Start` o `Update` vacíos.
- Evitar `Update`; preferir eventos, comandos o temporizadores con frecuencia justificada.
- Mantener componentes pequeños y sin navegación implícita entre escenas.

## Logging y errores

- Registrar errores accionables con contexto, sin spam por frame.
- No ocultar excepciones; capturarlas solo si existe recuperación o contexto adicional.
- Las condiciones esperables deben modelarse, no tratarse como excepciones.

## Pruebas

- Probar reglas de dominio en EditMode sin escenas cuando sea posible.
- Reservar PlayMode para integración con ciclo de vida y escenas.
- Cada corrección de defecto debería incluir una prueba de regresión cuando exista el sistema.
