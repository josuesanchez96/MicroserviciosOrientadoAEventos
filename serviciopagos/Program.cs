using Azure.Messaging.ServiceBus;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    const string pagosConnectionString = "";
    const string pagosQueueName = "serviciopagos";

    const string entregasConnectionString = "";
    const string entregasQueueName = "servicioentregas";

    const string mysqlConnectionString = "server=localhost;user=root;password=;database=pagosdb";

    static async Task Main(string[] args)
    {
        var client = new ServiceBusClient(pagosConnectionString);
        var processor = client.CreateProcessor(pagosQueueName, new ServiceBusProcessorOptions());

        processor.ProcessMessageAsync += async msgArgs =>
        {
            try
            {
                string body = msgArgs.Message.Body.ToString(); // <-- JSON recibido
                Console.WriteLine($"📥 Mensaje recibido en serviciopagos:\n{body}");

                // Deserializar solo para insertar en base de datos
                var pedido = JsonSerializer.Deserialize<PedidoDto>(body);

                using var conn = new MySqlConnection(mysqlConnectionString);
                await conn.OpenAsync();

                var cmdPedido = new MySqlCommand(
                    "INSERT INTO Pedidos (ClienteId, TipoPago, NotasAdicionales) VALUES (@ClienteId, @TipoPago, @Notas)", conn);
                cmdPedido.Parameters.AddWithValue("@ClienteId", pedido.ClienteId);
                cmdPedido.Parameters.AddWithValue("@TipoPago", pedido.TipoPago);
                cmdPedido.Parameters.AddWithValue("@Notas", pedido.NotasAdicionales);
                await cmdPedido.ExecuteNonQueryAsync();
                long pedidoId = cmdPedido.LastInsertedId;

                foreach (var prod in pedido.Productos)
                {
                    var cmdProd = new MySqlCommand(
                        "INSERT INTO ProductosPedido (PedidoId, ProductoId, Cantidad) VALUES (@PedidoId, @ProductoId, @Cantidad)", conn);
                    cmdProd.Parameters.AddWithValue("@PedidoId", pedidoId);
                    cmdProd.Parameters.AddWithValue("@ProductoId", prod.ProductoId);
                    cmdProd.Parameters.AddWithValue("@Cantidad", prod.Cantidad);
                    await cmdProd.ExecuteNonQueryAsync();
                }

                var dir = pedido.DireccionEntrega;
                var cmdDir = new MySqlCommand(
                    "INSERT INTO DireccionesEntrega (PedidoId, Calle, Numero, Ciudad, Departamento, Referencia) VALUES (@PedidoId, @Calle, @Numero, @Ciudad, @Departamento, @Referencia)", conn);
                cmdDir.Parameters.AddWithValue("@PedidoId", pedidoId);
                cmdDir.Parameters.AddWithValue("@Calle", dir.Calle);
                cmdDir.Parameters.AddWithValue("@Numero", dir.Numero);
                cmdDir.Parameters.AddWithValue("@Ciudad", dir.Ciudad);
                cmdDir.Parameters.AddWithValue("@Departamento", dir.Departamento);
                cmdDir.Parameters.AddWithValue("@Referencia", dir.Referencia);
                await cmdDir.ExecuteNonQueryAsync();

                Console.WriteLine("✅ Pedido guardado en pagosdb.");

                // ENVIAR JSON SIN MODIFICAR
                await using var entregaClient = new ServiceBusClient(entregasConnectionString);
                var sender = entregaClient.CreateSender(entregasQueueName);
                await sender.SendMessageAsync(new ServiceBusMessage(body)); // mismo body sin modificar
                Console.WriteLine("✅ JSON reenviado a servicioentregas.");

                await msgArgs.CompleteMessageAsync(msgArgs.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        };

        processor.ProcessErrorAsync += async errorArgs =>
        {
            Console.WriteLine($"❌ Error de procesador: {errorArgs.Exception.Message}");
            await Task.CompletedTask;
        };

        await processor.StartProcessingAsync();
        Console.WriteLine("🔄 Escuchando... Presiona ENTER para salir.");
        Console.ReadLine();
        await processor.StopProcessingAsync();
    }

    public class PedidoDto
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
