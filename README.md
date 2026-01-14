# Instrucciones para tu JRPG 3D en Unity

¡Hola! Aquí tienes los scripts base para tu juego JRPG en 3D. Sigue estos pasos para configurarlos en tu proyecto de Unity.

## 1. Configuración del Proyecto
1.  Abre tu proyecto en Unity (3D Core).
2.  Copia la carpeta `Assets` generada aquí dentro de la carpeta `Assets` de tu proyecto.

## 2. Generación Automática de Escena (Recomendado)
Para empezar rápido, he creado un script que configura todo por ti.
1.  En el menú superior de Unity, verás una nueva opción: `Tools` > `JRPG` > `Create 3D Test Scene`.
2.  Al hacer clic, se creará un suelo, un Jugador (Cápsula Azul), un Enemigo (Cubo Rojo) y un NPC (Cilindro Verde), además de configurar la cámara.

## 3. Configuración Manual y Notas Importantes

### Jugador (Player)
*   Usa un componente `Rigidbody`.
*   Asegúrate de que en `Constraints` del Rigidbody, estén marcadas las opciones **Freeze Rotation X** y **Freeze Rotation Z** para que la cápsula no se caiga al moverse.
*   **Tag:** El objeto debe tener el Tag `Player`.

### Interacciones
*   Para que el jugador detecte NPCs u objetos, necesitamos configurar las **Layers**.
*   Ve a la esquina superior derecha -> Layers -> Edit Layers.
*   Crea una nueva Layer llamada `Interactable` (por ejemplo, en la Layer 6).
*   Asigna esta Layer a tus NPCs (Cilindro Verde).
*   Selecciona al Jugador, busca el script `PlayerController` y en `Interact Layer`, selecciona `Interactable`.

### Combate (Enemigos)
*   El cubo rojo tiene el script `EnemyOverworld`.
*   Al tocarlo con el jugador, verás un mensaje en la consola ("Starting Battle...").
*   En un juego real, aquí es donde cargarías la escena de combate.

¡Disfruta construyendo tu mundo 3D!
