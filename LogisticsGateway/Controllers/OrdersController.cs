using Microsoft.AspNetCore.Mvc;
using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace LogisticsGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{

    private const string pagosConnectionString = "";
    private const string inventarioConnectionString = "";

    private const string pagosQueueName = "serviciopagos";
    private const string inventarioQueueName = "servicioinventario";


    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearPedidoDto dto)
    {
        // Serializar el mensaje
        string mensajeJson = JsonSerializer.Serialize(dto);

        // Enviar a la cola de pagos
        await using (var clientPagos = new ServiceBusClient(pagosConnectionString))
        {
            var sender = clientPagos.CreateSender(pagosQueueName);
            await sender.SendMessageAsync(new ServiceBusMessage(mensajeJson));
        }

        // Enviar a la cola de inventario
        await using (var clientInventario = new ServiceBusClient(inventarioConnectionString))
        {
            var sender = clientInventario.CreateSender(inventarioQueueName);
            await sender.SendMessageAsync(new ServiceBusMessage(mensajeJson));
        }

        return NoContent(); // 204
    }

    public class CrearPedidoDto
    {
        public string ClienteId { get; set; }
        public List<ProductoDto> Productos { get; set; }
        public DireccionDto DireccionEntrega { get; set; }
        public string TipoPago { get; set; }
        public string NotasAdicionales { get; set; }
    }

    public class ProductoDto
    {
        public string ProductoId { get; set; }
        public int Cantidad { get; set; }
    }

    public class DireccionDto
    {
        public string Calle { get; set; }
        public string Numero { get; set; }
        public string Ciudad { get; set; }
        public string Departamento { get; set; }
        public string Referencia { get; set; }
    }
}
