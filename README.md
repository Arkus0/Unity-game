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
3.  ¡Listo! Verás que aparece una **pequeña aldea** con:
    *   **Floor Grid:** Baldosas de suelo.
    *   **Houses:** Bloques marrones simulando casas.
    *   **Player:** Una cápsula azul (Tu personaje).
    *   **Enemy_Slime:** Un cubo rojo (Un enemigo).
    *   **Villager:** Cilindros verdes (NPCs para hablar).
    *   **Main Camera:** La cámara colocada en vista aérea (Top-Down).

---

## 4. Importar Assets de Quaternius

Si quieres usar los modelos del pack *Modular Character Outfits* y *Medieval Village MegaKit*:

1.  Descarga los packs desde la web de Quaternius (versión FBX recomendada).
2.  Descomprime los archivos y busca las carpetas con los modelos `.fbx`.
3.  Arrastra esas carpetas dentro de `Assets` en tu Unity.
4.  Asegúrate de que tienes la **Escena de Prueba** abierta.
5.  Ve al menú superior: `Tools` > `JRPG` > `Apply Quaternius Assets`.
6.  El script intentará buscar automáticamente modelos para:
    *   Personajes: "Warrior", "Monster", "Civilian".
    *   Entorno: "Floor_Stone", "House_Type1".
    *   *Nota:* Si el script no encuentra los modelos, intentará usar nombres genéricos. Si aun así falla, revisa que los archivos .fbx estén importados correctamente en la carpeta Assets.

---

## 5. Configuración de Animaciones

El script añade automáticamente un componente `PlayerAnimator` al jugador, pero necesitas crear el **Controlador de Animación**.

1.  En la carpeta Assets, haz clic derecho > `Create` > `Animator Controller`. Llámalo `PlayerAnim`.
2.  Abre la ventana **Animator**.
3.  Crea dos parámetros (pestaña Parameters):
    *   `IsMoving` (Bool)
    *   `InputX` (Float)
    *   `InputY` (Float)
4.  Arrastra tus animaciones (Idle, Run) al Animator.
5.  Crea transiciones entre Idle y Run usando la condición `IsMoving` (true para correr, false para parar).
6.  Asigna este `PlayerAnim` al componente `Animator` de tu personaje (el modelo visual hijo del objeto Player).

---

## 6. Ajustes Finales Manuales

Después de generar la escena, verifica estos puntos para asegurar que todo funcione perfecto:

### El Jugador (Player)
1.  Selecciona el objeto `Player` en la jerarquía.
2.  En el **Inspector** (derecha), busca el componente `Rigidbody`.
3.  Despliega **Constraints**.
4.  Marca:
    *   ✅ Freeze Rotation X
    *   ✅ Freeze Rotation Z
5.  Asegúrate de que en la parte superior del Inspector, el **Tag** sea `Player`.
6.  En el componente `Player Controller (Script)`:
    *   Busca `Interact Layer`.
    *   Selecciona la capa `Interactable` (o `Everything` si quieres probar rápido).

### Los NPCs (Villager)
1.  Selecciona los objetos `Villager`.
2.  Arriba a la derecha en el Inspector, cambia su **Layer** a `Interactable`.
3.  Si quieres cambiar lo que dicen, busca el script `NPC` y edita el texto en `Dialogue Text`.

---

## 7. Controles y Cómo Jugar

Dale al botón **Play** (▶) en la parte superior central.

*   **Movimiento:** Usa `W`, `A`, `S`, `D` o las `Flechas` del teclado.
*   **Interactuar:** Acércate a un NPC (Verde) y pulsa la tecla `E` o `Espacio`.
*   **Combatir:** Camina hacia el enemigo (Rojo) y tócalo.

---

## Solución de Problemas Comunes

**P: Mi personaje no se anima.**
R: Asegúrate de haber creado el `Animator Controller` como se explica en la sección 5 y haberlo asignado al campo `Controller` dentro del componente `Animator` del modelo visual.

**P: Mi personaje atraviesa el suelo.**
R: Asegúrate de que el objeto `Floor` tiene un `Mesh Collider`.

**P: El personaje se tumba o rueda.**
R: Revisa las `Constraints` del `Rigidbody` en el Player.
