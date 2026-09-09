# FilmRecipe Lab

Aplicación de escritorio para Windows 11 destinada a gestionar y editar recetas de la Fujifilm X-T50 almacenadas en perfiles FP1 de Fujifilm RAW Studio.

## Estructura

```text
src/
	FilmRecipeLab.Domain/          Modelo de receta y reglas del dominio
	FilmRecipeLab.Application/    Casos de uso y contratos de servicios
	FilmRecipeLab.Infrastructure/ Lectura/escritura FP1 y OCR offline
	FilmRecipeLab.Presentation/   Aplicación WPF, vistas y ViewModels

tests/
	FilmRecipeLab.Tests/           Pruebas automatizadas

Resources/
	DemoFile.FP1                   Perfil de referencia
```

La aplicación está limitada intencionadamente a la Fujifilm X-T50. No se busca crear un modelo genérico para otras cámaras.

## Comandos

```powershell
dotnet build FilmRecipeLab.sln
dotnet test FilmRecipeLab.sln
dotnet run --project src/FilmRecipeLab.Presentation
```
