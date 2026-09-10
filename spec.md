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
| `FilmSimulation` | Código/opción | `Provia`,`Velvia`,`Astia`,`Classic`,`Reala`,`NEGAhi`,`NEGAStd`,`ClassicNEGA`,`NostalgicNEGA`,`Eterna`,`BleachBypass`,`Acros`,`AcrosYe`,`AcrosG`,`AcrosR`,`BW`,`BYe`,`BG`,`Sepia` | - | `Provia` | Son los códigos internos observados en los perfiles FP1. La etiqueta visible de la cámara puede ser distinta. |
| `DynamicRange` | Entero/opción | `100`,`200`,`400` | - | `400` | Valores observados en los perfiles. `AUTO` queda pendiente de confirmar en el XML. |
| `WideDRange` | Entero/opción | Pendiente | Pendiente | `0` | Rango dinámico ampliado (no lo veo en la camara )|
| `ExposureBias` | Código/número | `-2` a `+3` | `1/3` | `0` | Rango visible en la X-T50. El XML codifica el signo y el valor como texto: `P0P00` = `0`, `P0P33` = `+1/3`, `P0P67` = `+2/3`, `P1P00` = `+1`, `P1P33` = `+1 1/3`, `P3P00` = `+3`, `M0P67` = `-2/3` y `M2P00` = `-2`. |
| `HighlightTone` | Número | `-2`,`4` | `0,5`  | `0` | Tono de altas luces |
| `ShadowTone` | Número | `-2`,`4` | `0,5`  | `0` | Tono de sombras |
| `Color` | Número | `-4`,`4` | `1` | `0` | Saturación/color |
| `Sharpness` | Número | `-4`,`4` | `1` | `0` | Nitidez |
| `NoisReduction` | Número | `-4`,`4` | `1` | `0` | Mantener el nombre original del XML |
| `Clarity` | Número | `-5`,`5` | `1` | `0` | Claridad |
| `WhiteBalance` | Código/opción | `INVALID`,`Auto`,`Auto_Ambience`,`Auto_White`,`Custom1`,`Custom2`,`Custom3`,`Daylight`,`FLight1`,`FLight2`,`FLight3`,`Incand`,`Shade`,`Temperature`,`UWater` | - | `INVALID` | Códigos internos observados en los perfiles. |
| `WBShiftR` | Número | `-9`,`9` | `1` | `0` | Ajuste de balance hacia rojo |
| `WBShiftB` | Número | `-9`,`9` | `1` | `0` | Ajuste de balance hacia azul |
| `WBColorTemp` | Temperatura (Kelvin) | `0K` o de `10000K` a `2500K` | `100k` | `0K` | Temperatura de la luz. cuando se utiliza WhiteBalance -> Temperature  -> `10000K` a `2500K` sino `0K` |
| `GrainEffect` | Opción | `OFF`,`STRONG`,`WEAK` | - | `OFF` | Valores observados en los perfiles. |
| `GrainEffectSize` | Opción | `LARGE`,`SMALL` | - | `SMALL` | Tamaño del grano |
| `ChromeEffect` | Opción | `OFF`,`STRONG` | - | `OFF` | Valores observados en los perfiles. `WEAK` no aparece en los ejemplos. |
| `ColorChromeBlue` | Opción | `OFF`,`STRONG`,`WEAK` | - | `OFF` | Valores observados en los perfiles. |
| `SmoothSkinEffect` | Opción | `OFF` | - | `OFF` | Solo aparece `OFF` en los ejemplos; valores activos quedan pendientes. |
| `BlackImageTone` | Número | `-18`,`18` | 1 | `0` | Tono monocromático (solo vale en peliculas especificas) |
| `MonochromaticColor_RG` | Número | `-18`,`18` | 1 | `0` | Color monocromático (solo vale en peliculas especificas ) |
| `LensModulationOpt` | Opción | Pendiente | - | `ON` | Optimización de modulación del objetivo (no lo veo en la camara ) |
| `ColorSpace` | Código/opción | `AdobeRGB`,`sRGB` | - | `sRGB` | El XML usa `AdobeRGB`, sin espacio. |

Parámetros de captura conservados en el perfil, pero inicialmente fuera del editor de recetas:

| Campo XML | Valor observado | Editable inicialmente |
|---|---|---|
| `FileType` | `JPG` | No |
| `ImageSize` | `L3x2` | No |
| `ImageQuality` | `Fine` | No |
| `ShootingCondition` | `OFF` | No |
| `WBShootCond` | `ON` | No |
| `DigitalTeleConv` | `OFF` | No |

`DigitalTeleConv` se conserva en el XML por compatibilidad, pero la aplicación lo fuerza siempre a `OFF` y no lo muestra como un parámetro editable.


Películas que aceptan `BlackImageTone` y `MonochromaticColor_RG` según los perfiles aportados:
`Acros`,`AcrosYe`,`AcrosG`,`AcrosR`,`BW`,`BYe`,`BG`,`Sepia`

### Nombres de recetas observados

El atributo XML `label` contiene el nombre personalizado de la receta, no la simulación de película. En los perfiles de ejemplo aparecen:

`Classic Retro`, `DisparoOriginalDr400%`, `loco1` a `loco24`, `Mio 1`, `Ninh Van Bay`, `Pro neg`, `Shibuya` y `ZL Portra 400 bis`.

Estos nombres deben tratarse como datos de usuario y no como valores cerrados del modelo. La simulación se obtiene exclusivamente de `FilmSimulation`.
