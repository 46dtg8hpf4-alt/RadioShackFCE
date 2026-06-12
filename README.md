Integrantes: Bruno Rancaño - Ivan Mammes - Francisco Regoli Freire 

Descripción del Proyecto: Sistema de comercio electrónico desarrollado mediante una arquitectura de microservicios. Cada dominio funcional fue implementado como una API independiente con comunicación HTTP entre servicios. 

Microservicios Implementados 
• Users API 
• Products API 
• Orders API 
• Cart API 
• Notifications API 

Tecnologías Utilizadas 
• .NET 10 
• ASP.NET Core Web API 
• SQLite 
• Dapper 
• Swagger/OpenAPI 
• Serilog 

Cómo Ejecutar el Proyecto 
1. Clonar el repositorio. 
2. Abrir RadioShackFCE.sln. 
3. Ejecutar dotnet restore. 
4. Ejecutar cada API mediante dotnet run. 
5. Acceder a Swagger para realizar pruebas. 

Comunicación entre Microservicios 
• Orders → Users y Products. 
• Cart → Products. 
• Notifications → Users. 
• Products → Orders para validar PRD-004. 

Documentación Adjunta  
• Diagrama de arquitectura. 
• Capturas de Swagger, evidencia de validaciones y códigos de error.
