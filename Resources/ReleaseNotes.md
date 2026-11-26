# Release notes

Cada release usa únicamente el contenido situado bajo el encabezado cuya versión coincide con el input del workflow. Añade una nueva sección por versión antes de publicar.

## 1.3

### Añadido
- Añadidos controles para `Color Chrome FX` y `Color Chrome FX Blue`, con niveles `OFF`, `WEAK` y `STRONG`.
- Agrupados el modo, la temperatura Kelvin y los desplazamientos rojo/azul en la sección de balance de blancos.

### Mejorado
- `Temperature` se muestra como `Kelvin fijo (Fixed Kelvin)` y los ajustes de desplazamiento se identifican como `WB Shift R` y `WB Shift B`.
- El selector de balance de blancos muestra la etiqueta visible en lugar de la representación interna de la opción.

## 1.2

### Mejorado
- El selector de simulación muestra los nombres visibles de Fujifilm en lugar de los códigos internos del XML. La aplicación sigue guardando los códigos FP1 correspondientes.
- Corregida la presentación del valor seleccionado en el selector de simulación: ahora muestra la etiqueta visible y no el nombre interno del objeto.
- Reorganizado el editor para separar mejor las etiquetas de los controles de la columna contigua.
- Agrupados el modo, la temperatura Kelvin y los desplazamientos rojo/azul en la sección de balance de blancos.
- `Temperature` se muestra como `Kelvin fijo (Fixed Kelvin)` y los ajustes de desplazamiento se identifican como `WB Shift R` y `WB Shift B`.

### Añadido 
- Añadida la creación de recetas X-T50 desde cero con valores iniciales FP1.
- Un borrador nuevo aparece como pendiente de guardar; `Guardar copia nueva` crea su primer archivo sin generar una entrada duplicada.
- Añadidos controles para `Color Chrome FX` y `Color Chrome FX Blue` con niveles `OFF`, `WEAK` y `STRONG`.

## 1.1

### Añadido 
- Autogeneracion de Release notes 

### Corregido
- Mejorado el contraste de los botones al pasar el ratón o pulsarlos.
- Cambiados los tooltips a fondo oscuro y texto claro para hacerlos legibles con el tema oscuro.

## 1.0

### Primera versión
- Aplicación de escritorio WPF para Windows 11, enfocada exclusivamente en recetas Fujifilm X-T50.
- Importación de uno o varios perfiles `.FP1` y exportación compatible con Fujifilm RAW Studio.
- Edición visual de los parámetros de receta, con sliders y valores legibles para Exposure Bias.
- Lectura y escritura que conserva metadatos y propiedades XML no editadas.
- Escaneo de la carpeta de perfiles configurada y recarga de cambios externos.
- Guardado de copias nuevas, actualización de perfiles y borrado con confirmación.
- Indicador de cambios pendientes en la lista de perfiles.
- Publicación autónoma como ejecutable para Windows x64.
