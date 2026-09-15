# TechLogistics S.A.

## Sistema de Gestión Logística de Inventarios en Tiempo Real

TechLogistics S.A. es una aplicación web desarrollada con **ASP.NET Core Blazor** para la gestión y monitoreo de inventarios de cuatro centros de distribución regionales: Guatemala, San Salvador, Tegucigalpa y San José.

El proyecto surge como respuesta a las limitaciones de un sistema monolítico heredado que presenta latencias superiores a 3.5 segundos en actualizaciones de inventario y no permite la operación fuera de línea de los agentes de despacho.

La nueva solución utiliza una arquitectura moderna basada en **Blazor, ASP.NET Core, PostgreSQL, API REST, gRPC, JWT y almacenamiento local**, incorporando Render Modes interactivos de servidor y WebAssembly.

## Objetivos del proyecto

La solución busca:

* Monitorear inventarios en tiempo real.
* Reducir la dependencia de un sistema monolítico.
* Permitir operación mediante Interactive Server e Interactive WebAssembly.
* Proporcionar operación offline para las operaciones de despacho.
* Gestionar productos, inventarios e historial mediante servicios REST.
* Transmitir actualizaciones de inventario mediante gRPC.
* Implementar un State Container para compartir estado entre componentes.
* Proteger información mediante autenticación JWT y autorización por roles.
* Aplicar principios SOLID y separación de responsabilidades.
* Validar la solución mediante pruebas automatizadas.

## Arquitectura del proyecto

La solución está organizada en tres proyectos principales:

```text
TechLogisticsProyecto
│
├── TechLogistics
│   └── Backend ASP.NET Core
│
├── TechLogistics.Client
│   └── Frontend Blazor WebAssembly
│
└── TechLogistics.Tests
    └── Pruebas automatizadas
```

### Backend — TechLogistics

El proyecto `TechLogistics` contiene:

* ASP.NET Core.
* API REST.
* Entity Framework Core.
* PostgreSQL.
* Modelos de datos.
* Servicios de negocio.
* Autenticación JWT.
* Autorización por roles.
* Servicios gRPC.
* Simulación de inventario.
* Notificaciones de actualización.
* Renderizado Interactive Server.

### Frontend — TechLogistics.Client

El proyecto `TechLogistics.Client` contiene:

* Componentes Blazor WebAssembly.
* Componentes reutilizables.
* State Container.
* Persistencia local.
* Operación offline.
* Sincronización de operaciones.
* Caché local.
* Consumo de API REST y servicios del backend.
* Renderizado Interactive WebAssembly.

### Pruebas — TechLogistics.Tests

El proyecto `TechLogistics.Tests` contiene:

* Pruebas unitarias con xUnit.
* Pruebas de componentes con bUnit.
* Pruebas de integración con `WebApplicationFactory`.
* Pruebas de servicios y controladores.
* Pruebas de sincronización offline.
* Medición de cobertura mediante Coverlet.

## 1. Arquitectura de componentes y Render Modes

La aplicación utiliza una **Blazor Web App con Render Modes intercalados**.

Se implementan:

* `InteractiveServer` para funcionalidades que requieren interacción y actualización desde el servidor.
* `InteractiveWebAssembly` para funcionalidades que requieren ejecución en el navegador y operación local.

El Dashboard utiliza interacción del lado del servidor para el monitoreo del inventario, mientras que el cliente WebAssembly permite ejecutar funcionalidades directamente en el navegador.

La solución también incorpora componentes reutilizables para evitar duplicación de código y facilitar el mantenimiento de la interfaz.

## 2. Gestión de estado y servicios REST/gRPC

### State Container

El sistema implementa el patrón **State Container** mediante servicios desacoplados.

El estado de inventario se administra mediante servicios que utilizan `INotifyPropertyChanged`, permitiendo notificar cambios a los componentes interesados sin establecer dependencias directas entre ellos.

### Persistencia local

Se utiliza **ProtectedLocalStorage** para la persistencia de preferencias del usuario.

Para las operaciones offline se utiliza almacenamiento local mediante `localStorage`.

Las operaciones realizadas sin conexión se mantienen pendientes hasta que el cliente recupera comunicación con el backend.

### Sincronización offline

`OfflineSyncService` procesa las operaciones pendientes y las envía nuevamente a la API.

Se soportan:

* Creación.
* Actualización.
* Eliminación.

Las operaciones sincronizadas correctamente son eliminadas del almacenamiento local.

### API REST

El backend expone servicios REST para la gestión de:

* Productos.
* Inventario.
* Historial de movimientos.
* Centros de distribución.

Las operaciones de inventario incluyen validaciones de existencia, stock negativo y registros duplicados.

### gRPC

Se implementa un canal gRPC para la transmisión de información de telemetría y actualizaciones de inventario.

El sistema utiliza streaming para comunicar cambios relacionados con el inventario.

## 3. Autenticación y autorización por roles

La aplicación utiliza autenticación mediante **JWT (JSON Web Token)**.

Se implementan los siguientes roles:

* `GerenteBodega`
* `AgenteCampo`

La autorización se aplica tanto en los endpoints protegidos como en los componentes de interfaz.

Las funcionalidades administrativas pueden restringirse mediante autorización basada en roles y componentes `AuthorizeView`.

## 4. Calidad y principios SOLID

La solución aplica separación de responsabilidades mediante:

* Controladores para exposición de endpoints.
* Servicios para lógica de negocio.
* Modelos para representación de datos.
* Servicios independientes para estado y almacenamiento.
* Interfaces para desacoplar dependencias.
* Inyección de dependencias.

Como parte de la aplicación de SOLID, `OfflineSyncService` utiliza la interfaz `IOfflineStorageService`, evitando depender directamente de una implementación concreta del almacenamiento.

## Pruebas automatizadas

El proyecto utiliza tres mecanismos principales de pruebas:

### xUnit

Se utiliza para validar controladores, servicios y lógica de negocio.

### bUnit

Se utiliza para validar componentes Blazor y su comportamiento durante el renderizado e interacción.

### Pruebas de integración

Se utiliza `WebApplicationFactory` para probar los endpoints de la aplicación ASP.NET Core.

### Resultado final de ejecución

La última ejecución produjo:

```text
Total de pruebas: 63
Pruebas correctas: 63
Pruebas con errores: 0
Pruebas omitidas: 0
```

Porcentaje de aprobación:

```text
100% de las pruebas ejecutadas correctamente
```

### Cobertura

La medición final mediante Coverlet produjo:

```text
Line coverage:   17.8%
Branch coverage: 11.7%
```

La cobertura global incluye código generado automáticamente por Razor, Protobuf/gRPC y otros componentes que no representan directamente lógica de negocio desarrollada manualmente.

Por esta razón, el porcentaje global no representa por sí solo la cobertura de las funcionalidades críticas. Se realizaron pruebas específicas sobre controladores, servicios, componentes Blazor y endpoints de integración.

## Tecnologías utilizadas

* .NET 10
* ASP.NET Core
* Blazor
* Blazor WebAssembly
* C#
* Entity Framework Core
* PostgreSQL
* API REST
* gRPC
* JWT
* ProtectedLocalStorage
* JavaScript / localStorage
* xUnit
* bUnit
* Microsoft.AspNetCore.Mvc.Testing
* Git
* GitHub

## Requisitos

Para ejecutar el proyecto se requiere:

* .NET 10 SDK.
* Visual Studio Code o IDE compatible con .NET.
* Git.
* PostgreSQL 17.
* Docker Desktop, opcionalmente para ejecutar PostgreSQL mediante contenedor.

## Configuración de PostgreSQL

La aplicación utiliza PostgreSQL como sistema de persistencia.

Configuración utilizada durante el desarrollo:

```text
Host: localhost
Port: 5433
Database: techlogistics
User: postgres
```

Las cadenas de conexión se encuentran configuradas mediante los archivos de configuración del backend.

## Ejecución del backend

**Backend — `TechLogistics`**

Desde PowerShell:

```powershell
cd C:\Users\patza\TechLogisticsProyecto\TechLogistics
dotnet run
```

El backend se ejecuta actualmente en:

```text
http://localhost:5134
```

## Ejecución del frontend

**Frontend — `TechLogistics.Client`**

En otra terminal:

```powershell
cd C:\Users\patza\TechLogisticsProyecto\TechLogistics.Client
dotnet run
```

El cliente se ejecuta actualmente en:

```text
http://localhost:5085
```

## Ejecución de pruebas

Desde la raíz del proyecto:

```powershell
cd C:\Users\patza\TechLogisticsProyecto
dotnet test .\TechLogistics.Tests\TechLogistics.Tests.csproj
```

Para generar cobertura:

```powershell
dotnet test .\TechLogistics.Tests\TechLogistics.Tests.csproj --collect:"XPlat Code Coverage"
```

El resultado de cobertura se genera en:

```text
TechLogistics.Tests\TestResults\
```

## Estructura principal

```text
TechLogisticsProyecto
│
├── TechLogistics
│   ├── Controllers
│   ├── Components
│   ├── Data
│   ├── DTOs
│   ├── Migrations
│   ├── Models
│   ├── Protos
│   ├── Services
│   ├── Program.cs
│   └── README.md
│
├── TechLogistics.Client
│   ├── Components
│   ├── Models
│   ├── Pages
│   ├── Services
│   ├── wwwroot
│   └── Program.cs
│
└── TechLogistics.Tests
    ├── Components
    ├── Controllers
    ├── Integration
    ├── Services
    └── TechLogistics.Tests.csproj
```

## Repositorio

Repositorio GitHub:

https://github.com/MGsam2/TechLogistics

Para clonar:

```powershell
git clone https://github.com/MGsam2/TechLogistics.git
```

## Estado del proyecto

El proyecto cuenta con la implementación de los cuatro pilares establecidos en el encargo.

### Arquitectura de Componentes y Render Modes

* Interactive Server.
* Interactive WebAssembly.
* Componentes reutilizables.
* Separación Backend/Frontend.

### Gestión de Estado y Servicios REST/gRPC

* State Container.
* `INotifyPropertyChanged`.
* `ProtectedLocalStorage`.
* Operación offline.
* Sincronización local.
* API REST.
* gRPC y streaming.

### Autenticación y Autorización

* JWT.
* Roles `GerenteBodega` y `AgenteCampo`.
* Protección de endpoints.
* `AuthorizeView`.

### Pruebas y Calidad

* xUnit.
* bUnit.
* Pruebas de integración.
* 63 pruebas ejecutadas.
* 63 pruebas correctas.
* 0 errores.
* 0 pruebas omitidas.
* Aplicación de principios SOLID.

## Documentación adicional

El proyecto se complementa con:

* `log.txt`: registro del avance durante las cinco semanas.
* Diagramas técnicos de arquitectura.
* Memoria técnica de arquitectura en formato PDF.
