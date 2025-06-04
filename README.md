## Instrucciones para correrlo localmente

### Requisitos previos
1.	Visual Studio 2022:  
.NET Framework Developer Pack (4.8)
2.	MySQL Server con las siguientes bases de datos creadas:  
    - pagosdb
    - inventariodb
    - entregasdb
3.	Azure Service Bus configurado con las siguientes colas:  
    - serviciopagos
    - servicioinventario
    - servicioentregas
4.	Cadenas de conexión correctas en cada archivo web.config o app.config, tanto para MySQL como para Azure Service Bus.


### Clonar el repositorio:
Para obtener el código fuente completo de la solución realice lo siguiente:
* Abra una terminal
* Ejecute el siguiente comando para clonar el repositorio:  
git clone https://github.com/josuesanchez96/MicroserviciosOrientadoAEventos.git
* Configure sus claves de las colas de Service Bus en los siguientes proyectos:
    - servicioentregas/Program.cs
    - serviciopagos/Program.cs
    - LogisticsGateway/Controllers/OrdersController.cs
    - servicioinventario/Program.cs

### Ejecución local

1.	Abrir la solución completa (.sln) en Visual Studio 2022.
2.	Establecer como proyecto de inicio el proyecto EntregaMicroservicios (Web Forms).
3.	Hacer clic en el botón “Iniciar” o presionar F5.
4.	El navegador abrirá automáticamente Default.aspx, desde donde se puede:
    - Ingresar pedidos.
    - Consultar datos desde ConsultaDatos.aspx.

### Flujo de trabajo una vez iniciado
1.	El formulario (Default.aspx) genera un JSON con la información del pedido.
2.	Este JSON se envía al Web API (LogisticsGateway), al endpoint /api/orders.
3.	El Web API enruta el mensaje a:
    - Cola serviciopagos
    - Cola servicioinventario
4.	Los microservicios de consola escuchan estas colas y:
    - serviciopagos inserta en pagosdb y reenvía a servicioentregas.
    - servicioinventario inserta en inventariodb.
    - servicioentregas inserta en entregasdb (tabla DireccionesEntrega).

# Uso de buenas prácticas
Durante el desarrollo del proyecto se aplicaron varias buenas prácticas para asegurar un código limpio, seguro y fácilmente mantenible.
### Archivo .gitignore
Se configuró adecuadamente el archivo .gitignore para excluir:  
- Claves de Azure Service Bus.
- Archivos sensibles como .env.
### Manejo de credenciales
Se identificó las cadenas donde contenían claves y se remplazó de forma manual antes de publicar el proyecto en el repositorio.
### Organización del proyecto
El código fuente se encuentra modularizado en cinco proyectos separados.
### Comentarios y documentación
Se documentaron las secciones clave del código con comentarios explicativos.


