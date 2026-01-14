# Instrucciones para tu JRPG 2D en Unity

¡Hola! Aquí tienes los scripts base para tu juego. Sigue estos pasos para configurarlos en tu proyecto de Unity.

## 1. Configuración del Proyecto
1.  Abre tu proyecto en Unity (2D Core).
2.  Copia la carpeta `Assets` generada aquí dentro de la carpeta `Assets` de tu proyecto.

## 2. Configuración del Jugador (Player)
1.  Crea un nuevo Sprite o GameObject en la escena para tu jugador.
2.  Añádele un componente `Rigidbody2D`.
    *   **Importante:** En el Rigidbody2D, ajusta `Gravity Scale` a `0` (para que no se caiga en la vista top-down).
    *   Marca `Freeze Rotation Z` en "Constraints".
3.  Añádele un componente `BoxCollider2D` o `CircleCollider2D` para las colisiones.
4.  Añade el script `PlayerController` (arrástralo desde `Assets/Scripts/Player`).
    *   Puedes ajustar la velocidad (`Move Speed`) en el Inspector.
5.  **Tag:** Asegúrate de que el GameObject del jugador tenga el Tag `Player` (arriba a la derecha en el Inspector).

## 3. Configuración de Interacciones (NPCs / Objetos)
1.  Crea un objeto (ej: un Sprite de un aldeano).
2.  Añádele un `BoxCollider2D`.
3.  Añade el script `NPC`.
    *   Escribe el texto que quieras en el campo `Dialogue Text`.
4.  **Layers:**
    *   Ve a la esquina superior derecha -> Layers -> Edit Layers.
    *   Crea una nueva Layer llamada `Interactable` (por ejemplo, en la Layer 6).
    *   Asigna esta Layer a tu objeto NPC.
5.  **Volver al Player:**
    *   En el script `PlayerController` del jugador, busca `Interaction Settings`.
    *   En `Interact Layer`, selecciona la layer `Interactable` que acabas de crear.
    *   Ahora, al acercarte y pulsar **E** o **Espacio**, verás el mensaje en la consola.

## 4. Configuración de Enemigos (Encuentros)
1.  Crea un objeto vacío en la escena y llámalo `GameManager`.
2.  Añádele el script `EncounterManager`.
3.  Crea un enemigo en el mapa (Sprite).
4.  Añádele un `BoxCollider2D` y el script `EnemyOverworld`.
    *   Asegúrate de que el Collider no sea "Trigger" si quieres que el jugador choque contra él (o márcalo como Trigger si quieres que lo atraviese al iniciar combate, en cuyo caso habría que cambiar `OnCollisionEnter2D` a `OnTriggerEnter2D` en el script).
    *   El script actual usa `OnCollisionEnter2D`, así que asegúrate de que **no** sea Trigger.
5.  Cuando el jugador toque al enemigo, verás un mensaje en la consola indicando el inicio de la batalla.

¡Mucha suerte con tu desarrollo!
