# Guía de Instalación y Desarrollo: JRPG 3D en Unity

¡Bienvenido a la base de tu proyecto JRPG! Este repositorio contiene todos los scripts necesarios para configurar el movimiento, la interacción y el sistema de encuentros de tu juego.

Sigue esta guía paso a paso para configurar tu entorno en Unity.

---

## 1. Configuración Inicial del Proyecto

### Paso 1: Crear el Proyecto
1.  Abre **Unity Hub**.
2.  Crea un **Nuevo Proyecto**.
3.  Selecciona la plantilla **3D Core**.
4.  Ponle nombre a tu juego y crea el proyecto.

### Paso 2: Importar los Scripts
1.  Descarga los archivos de este repositorio.
2.  Copia la carpeta `Assets` (que contiene `Scripts`) y pégala dentro de la carpeta `Assets` de tu proyecto de Unity.
    *   *Ruta final esperada:* `TuProyecto/Assets/Scripts/...`

---

## 2. Configuración Esencial (¡Muy Importante!)

Unity necesita que configures algunas "etiquetas" y "capas" para que los scripts sepan qué es el jugador y qué son los objetos interactuables.

### Configurar Tags (Etiquetas)
1.  En Unity, ve al menú superior: `Edit` > `Project Settings` > `Tags and Layers`.
2.  Despliega la sección **Tags**.
3.  Asegúrate de que existe el Tag `Player` (Unity suele traerlo por defecto).
4.  Si no existe, pulsa el botón `+` y crea uno llamado exactamente `Player`.

### Configurar Layers (Capas)
1.  En la misma ventana (`Tags and Layers`), despliega la sección **Layers**.
2.  Busca una ranura vacía (normalmente la Layer 6 o la 7) y escribe el nombre: `Interactable`.
    *   Esto servirá para identificar NPCs, cofres, puertas, etc.

---

## 3. Generar la Escena de Prueba

Para que no empieces con una pantalla vacía, hemos creado una herramienta automática.

1.  En el menú superior de Unity, busca la nueva pestaña: `Tools`.
2.  Selecciona `JRPG` > `Create 3D Test Scene`.
3.  ¡Listo! Verás que aparecen varios objetos en tu escena:
    *   **Floor:** Un suelo gris.
    *   **Player:** Una cápsula azul (Tu personaje).
    *   **Enemy_Slime:** Un cubo rojo (Un enemigo).
    *   **Villager:** Cilindros verdes (NPCs para hablar).
    *   **Main Camera:** La cámara colocada en vista aérea (Top-Down).

---

## 4. Importar Modelos 3D (Quaternius Assets)

Si quieres usar los personajes del pack *Modular Character Outfits*:

1.  Descarga el pack desde la web de Quaternius.
2.  Descomprime el archivo y busca la carpeta con los modelos `.fbx`.
3.  Arrastra esa carpeta dentro de `Assets` en tu Unity.
    *   *Recomendación:* Llámala `QuaterniusModels` para tenerlo ordenado.
4.  Asegúrate de que tienes la **Escena de Prueba** abierta.
5.  Ve al menú superior: `Tools` > `JRPG` > `Apply Quaternius Assets`.
6.  El script intentará buscar automáticamente modelos llamados "Warrior", "Monster", "Civilian", etc. y reemplazará las cápsulas de colores por estos personajes.
    *   *Nota:* Si el script no encuentra los modelos exactos, te avisará. En ese caso, puedes asignar los modelos manualmente arrastrándolos sobre los objetos en la jerarquía.

---

## 5. Ajustes Finales Manuales

Después de generar la escena, verifica estos puntos para asegurar que todo funcione perfecto:

### El Jugador (Player)
1.  Selecciona el objeto `Player` en la jerarquía.
2.  En el **Inspector** (derecha), busca el componente `Rigidbody`.
3.  Despliega **Constraints**.
4.  Marca:
    *   ✅ Freeze Rotation X
    *   ✅ Freeze Rotation Z
    *   *(Esto evita que el personaje se caiga de cara al chocar con algo)*.
5.  Asegúrate de que en la parte superior del Inspector, el **Tag** sea `Player`.
6.  En el componente `Player Controller (Script)`:
    *   Busca `Interact Layer`.
    *   Selecciona la capa `Interactable` (o `Everything` si quieres probar rápido).

### Los NPCs (Villager)
1.  Selecciona los objetos `Villager`.
2.  Arriba a la derecha en el Inspector, cambia su **Layer** a `Interactable`.
    *   Unity te preguntará si quieres cambiar también a los hijos. Di que **Sí**.
3.  Si quieres cambiar lo que dicen, busca el script `NPC` y edita el texto en `Dialogue Text`.

---

## 6. Controles y Cómo Jugar

Dale al botón **Play** (▶) en la parte superior central.

*   **Movimiento:** Usa `W`, `A`, `S`, `D` o las `Flechas` del teclado.
*   **Interactuar:** Acércate a un NPC (Verde) y pulsa la tecla `E` o `Espacio`.
    *   *Resultado:* Verás el mensaje de diálogo en la consola de Unity (abajo a la izquierda).
*   **Combatir:** Camina hacia el enemigo (Rojo) y tócalo.
    *   *Resultado:* El enemigo desaparecerá y la consola dirá "Starting Battle...".

---

## Solución de Problemas Comunes

**P: Mi personaje atraviesa el suelo.**
R: Asegúrate de que el objeto `Floor` tiene un `Mesh Collider` (o `Box Collider`) y el `Player` tiene un `Capsule Collider`.

**P: El personaje se tumba o rueda.**
R: Revisa las `Constraints` del `Rigidbody` en el Player. Debes congelar la rotación en X y Z.

**P: Pulso 'E' pero no habla con el NPC.**
R:
1. Revisa que el NPC tenga la Layer `Interactable`.
2. Revisa que el `PlayerController` tenga marcada esa misma Layer en `Interact Layer`.
3. Asegúrate de estar lo suficientemente cerca y mirando hacia el NPC.

**P: Los modelos de Quaternius se ven rosas.**
R: Eso significa que falta el Material o el Shader. Normalmente Unity los importa bien, pero si pasa, crea un nuevo Material, asígnalo al modelo y ponle un color o textura.
