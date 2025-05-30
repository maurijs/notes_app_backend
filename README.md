# Notes Manager
Notes Manager es una aplicación web moderna que permite a los usuarios registrados gestionar sus notas personales de forma eficiente.

## Funcionalidades principales
✅ Registro e inicio de sesión de usuarios con autenticación JWT segura.
📝 Crear, editar y eliminar notas.
🏷️ Asignar múltiples etiquetas (tags) a cada nota.
🔍 Buscar notas por título, contenido o etiquetas.
📂 Archivar y desarchivar notas.
🧠 Filtrado por etiquetas clickeables.
📅 Las notas se ordenan automáticamente por fecha de creación (más recientes arriba).
🔒 Cada usuario ve únicamente sus propias notas, tanto activas como archivadas.

## Arquitectura y estructura
-- Frontend como SPA (Single Page Application) usando React + Vite + TypeScript, comunicándose con el backend vía API RESTful usando Axios.

-- Backend desacoplado, estructurado en capas (Controllers, Services, Repositories), siguiendo buenas prácticas y principios SOLID.

-- Persistencia de datos en PostgreSQL con Entity Framework Core 8 como ORM.

-- Gestión de autenticación y autorización basada en JSON Web Tokens (JWT).

-- Seguridad reforzada con BCrypt para el almacenamiento seguro de contraseñas.
