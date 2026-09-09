# FlimRecipe LAb

## Objetivo
-Aplicación de escritorio para gestionar y editar recetas de la Fujifilm X-T50,
-Almacenadas en archivos xml de "Fujifilm RAW Studio" ,  ver Resources/DemoFile.FP1
-Editar/crear nuevas recetas con una interfaz visual intuitiva
-Capcidad de importar recetas desde capturas de pantalla mediante OCR

## Alcance version inicial
-Aplicación de escritorio para gestionar y editar recetas de la Fujifilm X-T50,
-Almacenadas en archivos xml de "Fujifilm RAW Studio" ,  ver Resources/DemoFile.FP1
-Editar/crear nuevas recetas con una interfaz visual intuitiva

## Fuera de alcance
-Capcidad de importar recetas desde capturas de pantalla mediante OCR

## Reglas y restricciones
- Rendimiento aceptable
- Seguridad , 100% offline
- Compatibilidad, Windows 11 mediante binarios compilados
- Cámara soportada, exclusivamente Fujifilm X-T50
- No se requiere compatibilidad genérica con otras cámaras Fujifilm
- Framework previsto, WPF sobre C#/.NET
- Arquitectura, separación Model-View-ViewModel con servicios independientes por funcionalidad

## Modelo de receta
Los metadatos identificativos del archivo se conservan durante la importación y exportación, pero no forman parte de la receta editable:

| Campo XML | Descripción | Editable |
|---|---|---|
| `device` | Modelo de cámara. Debe ser `X-T50` | No |
| `version` | Versión del perfil de la cámara | No |
| `label` | Nombre de la receta | Sí |
| `SerialNumber` | Número de serie de la cámara | No |
| `IOPCode` | Identificador interno del perfil | No |
| `TetherRAWConditonCode` | Código de condiciones RAW | No |

Parámetros editables de la receta:

| Campo XML | Tipo | Rango u opciones | Paso | Valor inicial | Notas |
|---|---|---|---|---|---|
| `FilmSimulation` | Texto/opción | `Provia`,`Velvia`,`Astia`,`Clasic Chrome`,`Reala Ace`,`Pro Neg Hi`,`Pro Neg Std`,`Neg Clasico`,`Neg Nostalgico`,`Eterna`,`Eterna bleach`,`Acros`,`Monocromo`,`Sepia` | - | `Provia` | Simulación de película ( falta verificar nombres estan en español por el menu de la camara , pero igul internamente van en ingles o abreviados) |
| `DynamicRange` | Entero/opción | `AUTO`,`100`,`200`,`400` | - | `400` | Rango dinámico |
| `WideDRange` | Entero/opción | Pendiente | Pendiente | `0` | Rango dinámico ampliado (no lo veo en la camara )|
| `ExposureBias` | Número | `-3`,`3` | `1/3` | `0` | Compensación de exposición (dial fisico)|
| `HighlightTone` | Número | `-2`,`4` | `0,5`  | `0` | Tono de altas luces |
| `ShadowTone` | Número | `-2`,`4` | `0,5`  | `0` | Tono de sombras |
| `Color` | Número | `-4`,`4` | `1` | `0` | Saturación/color |
| `Sharpness` | Número | `-4`,`4` | `1` | `0` | Nitidez |
| `NoisReduction` | Número | `-4`,`4` | `1` | `0` | Mantener el nombre original del XML |
| `Clarity` | Número | `-5`,`5` | `1` | `0` | Claridad |
| `WhiteBalance` | Opción | Pendiente | - | `INVALID` | Balance de blancos (falta verificar , salen las escenas tipicas en español) |
| `WBShiftR` | Número | `-9`,`9` | `1` | `0` | Ajuste de balance hacia rojo |
| `WBShiftB` | Número | `-9`,`9` | `1` | `0` | Ajuste de balance hacia azul |
| `WBColorTemp` | Temperatura | Pendiente | Pendiente | `0K` | Temperatura de color (no lo veo en la camara )|
| `GrainEffect` | Opción | `OFF`,`STRONG`,`WEAK` | - | `OFF` | Efecto de grano |
| `GrainEffectSize` | Opción | `LARGE`,`SMALL` | - | `SMALL` | Tamaño del grano (flata verificar) |
| `ChromeEffect` | Opción | `OFF`,`STRONG`,`WEAK` | - | `OFF` | Efecto Chrome |
| `ColorChromeBlue` | `OFF`,`STRONG`,`WEAK` | - | `OFF` | Efecto Color Chrome Blue |
| `SmoothSkinEffect` | Opción | `OFF`,`STRONG`,`WEAK` | - | `OFF` | Suavizado de piel |
| `BlackImageTone` | Número | `-18`,`18` | 1 | `0` | Tono monocromático (solo vale en peliculas especificas) |
| `MonochromaticColor_RG` | Número | `-18`,`18` | 1 | `0` | Color monocromático (solo vale en peliculas especificas ) |
| `LensModulationOpt` | Opción | Pendiente | - | `ON` | Optimización de modulación del objetivo (no lo veo en la camara ) |
| `ColorSpace` | Opción | `Adobe RGB`,`sRGB` | - | `sRGB` | Espacio de color |
| `DigitalTeleConv` | Opción | `2`,`1.4`,`OFF` | - | `OFF` | Teleconvertidor digital (flata verificar) |

Parámetros de captura conservados en el perfil, pero inicialmente fuera del editor de recetas:

| Campo XML | Valor observado | Editable inicialmente |
|---|---|---|
| `FileType` | `JPG` | No |
| `ImageSize` | `L3x2` | No |
| `ImageQuality` | `Fine` | No |
| `ShootingCondition` | `OFF` | No |
| `WBShootCond` | `ON` | No |


Peliculas que aceptan BlackImageTone y MonochromaticColor_RG
`Acros`,`Monocromo`
