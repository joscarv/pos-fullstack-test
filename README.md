Sistema POS (Point of Sale) desarrollado como prueba tenica Fullstack con:
- Backend: ASP.NET Core Web API
- Frontend: ASP.NET Core MVC (Razor)
- Base de datos: SQL Server
- Arquitectura: API + Web (BFF/Proxy)
- Comunicación: HTTPS desde el navegador

El sistema permite:
- Administración de productos
- Control de inventario
- Registro de ventas
- Cobro en efectivo y tarjeta (simulado)
- Generación de ticket
- Manejo de impuestos (IVA / IEPS)

Requisitos
- .NET SDK 8.0
- SQL Server (LocalDB o instancia local)
- Visual Studio 2022
- Navegador moderno (Chrome / Edge)

Cómo ejecutar el proyecto:
- Abrir el proyecto desde Visual Studio.
- En Pos.Api/appsettings.json cambiar credenciales de SQL Server:
"ConnectionStrings": {
  "PosDb": "Server=127.0.0.1;Database=PosCcK;User Id=UserPOS;Password=Test123;TrustServerCertificate=True;"
}
- Ejecuta migraciones desde Visual Studio: 
    - Posicionandote en el proyecto Pos.Infrastructure
    - Abrir Consola del Administrador de Paquetes
    - Ejecutar los siguientes comandos para crear Database y cargar Productos: 
        - Add-Migration InitialCreate -Project Pos.Infrastructure -StartupProject Pos.Api -OutputDir Data/Migrations
        - Update-Database -Project Pos.Infrastructure -StartupProject Pos.Api
- Ejecutar el proyecto, en automatico arranca la API en ASP.NET Core y Front en ASP.Net Core MVC
Url de Web: https://localhost:7263/
Url de Api con Swagger: https://localhost:7129/swagger/index.html

Prueba Swagger:
- GET  /api/products
    Busca por SKU o nombre
- POST /api/products 
    {
        "sku": "901183",
        "name": "Cerveza oscura negra modelo 355ml",
        "price": 25,
        "iepsRate": 0.26,
        "stock": 30
    }
- GET /api/products/{id}
    Busca pos Id de articulo los cuales van del 1 en adelante.
- PUT /api/products/{id}
    {
        "sku": "112233",
        "name": "Coca Cola 600ml",
        "price": 21,
        "iepsRate": 0,
        "stock": 100
    }
- DELETE /api/products/{id}
    Elimina por id de articulo los cuales van del 1 en adelante.

- POST /api/sales
    {
        "items": [
            {
                "productId": 1,
                "quantity": 2
            },
            {
                "productId": 3,
                "quantity": 3
            },
            {
                "productId": 6,
                "quantity": 1
            }
        ]
    }
- GET /api/sales/{id}
    Busca transaccion por Id las cuales van del 1 en adelante.
- POST /api/sales/{id}/payments
    Escribe el Id de la transaccion que quieras pagar
    {
        "payments": [
            {
                "method": "CASH",
                "amount": 200,
                "reference": ""
            }
        ]
    }
    Otro caso para CARD
    {
        "payments": [
            {
                "method": "CARD",
                "amount": 200,
                "reference": "FAIL"
            }
        ]
    }
    Otro caso para CARD
    {
        "payments": [
            {
                "method": "CARD",
                "amount": 200,
                "reference": "111445"
            }
        ]
    }

-POST /api/sales/{id}/cancel
    Cancela Transaccion por Id las cuales van del 1 en adelante. Este metodo regresa los articulos al Stock


Autor:
Oscar Juarez
Prueba técnica Fullstack .NET


