# Instrucciones para tu JRPG 3D en Unity

¡Hola! Aquí tienes los scripts base para tu juego JRPG en 3D. Sigue estos pasos para configurarlos en tu proyecto de Unity.

## 1. Configuración del Proyecto
1.  Abre tu proyecto en Unity (3D Core).
2.  Copia la carpeta `Assets` generada aquí dentro de la carpeta `Assets` de tu proyecto.

## 2. Generación Automática de Escena (Recomendado)
Para empezar rápido, he creado un script que configura todo por ti.
1.  En el menú superior de Unity, verás una nueva opción: `Tools` > `JRPG` > `Create 3D Test Scene`.
2.  Al hacer clic, se creará un suelo, un Jugador (Cápsula Azul), un Enemigo (Cubo Rojo) y dos NPCs (Cilindros Verdes), además de configurar la cámara.

## 3. Importación de Personajes (Quaternius Assets)
Si has descargado el pack de modelos de Quaternius:
1.  Arrastra la carpeta descomprimida dentro de la carpeta `Assets` de Unity.
2.  Asegúrate de tener la escena de prueba abierta.
3.  Ve al menú superior: `Tools` > `JRPG` > `Apply Quaternius Assets`.
4.  El script buscará automáticamente modelos compatibles (ej: "Warrior", "Skeleton", "Civilian") y reemplazará las cápsulas/cubos por los modelos 3D reales.

## 4. Configuración Manual y Notas Importantes

### Jugador (Player)
*   Usa un componente `Rigidbody`.
*   Asegúrate de que en `Constraints` del Rigidbody, estén marcadas las opciones **Freeze Rotation X** y **Freeze Rotation Z** para que la cápsula no se caiga al moverse.
*   **Tag:** El objeto debe tener el Tag `Player`.

### Interacciones
*   Para que el jugador detecte NPCs u objetos, el script usa una `Interact Layer`.
*   Por defecto está configurada en "Everything" para facilitar las pruebas, pero para mayor control:
    1.  Crea una nueva Layer llamada `Interactable`.
    2.  Asigna esta Layer a tus NPCs.
    3.  En el `PlayerController`, cambia la `Interact Layer` a `Interactable`.

### Combate (Enemigos)
*   El cubo rojo (o modelo de enemigo) tiene el script `EnemyOverworld`.
*   Al tocarlo con el jugador, el enemigo desaparecerá (simulando victoria/inicio de combate) y verás un mensaje en la consola.

¡Disfruta construyendo tu mundo 3D!
