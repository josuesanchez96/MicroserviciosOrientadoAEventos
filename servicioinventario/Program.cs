using Azure.Messaging.ServiceBus;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    const string queueConnectionString = "";
    const string queueName = "servicioinventario";

    const string mysqlConnectionString = "server=localhost;user=root;password=;database=inventariodb";

    static async Task Main(string[] args)
    {
        var client = new ServiceBusClient(queueConnectionString);
        var processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());

        processor.ProcessMessageAsync += async msgArgs =>
        {
            try
            {
                string body = msgArgs.Message.Body.ToString();
                Console.WriteLine($"📥 Mensaje recibido en servicioinventario:\n{body}");

                var pedido = JsonSerializer.Deserialize<PedidoDto>(body);

                using var conn = new MySqlConnection(mysqlConnectionString);
                await conn.OpenAsync();

                // Insertar pedido
                var cmdPedido = new MySqlCommand(
                    "INSERT INTO Pedidos (ClienteId, TipoPago, NotasAdicionales) VALUES (@ClienteId, @TipoPago, @Notas)", conn);
                cmdPedido.Parameters.AddWithValue("@ClienteId", pedido.ClienteId);
                cmdPedido.Parameters.AddWithValue("@TipoPago", pedido.TipoPago);
                cmdPedido.Parameters.AddWithValue("@Notas", pedido.NotasAdicionales);
                await cmdPedido.ExecuteNonQueryAsync();
                long pedidoId = cmdPedido.LastInsertedId;

                // Insertar productos
                foreach (var prod in pedido.Productos)
                {
                    var cmdProd = new MySqlCommand(
                        "INSERT INTO ProductosPedido (PedidoId, ProductoId, Cantidad) VALUES (@PedidoId, @ProductoId, @Cantidad)", conn);
                    cmdProd.Parameters.AddWithValue("@PedidoId", pedidoId);
                    cmdProd.Parameters.AddWithValue("@ProductoId", prod.ProductoId);
                    cmdProd.Parameters.AddWithValue("@Cantidad", prod.Cantidad);
                    await cmdProd.ExecuteNonQueryAsync();
                }

                // Insertar dirección
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

                Console.WriteLine("✅ Pedido insertado en inventariodb.");
                await msgArgs.CompleteMessageAsync(msgArgs.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al procesar mensaje: {ex.Message}");
                // Podrías usar msgArgs.DeadLetterMessageAsync() si deseas marcarlo como no procesable
            }
        };

        processor.ProcessErrorAsync += async errorArgs =>
        {
            Console.WriteLine($"❌ Error del procesador: {errorArgs.Exception.Message}");
            await Task.CompletedTask;
        };

        await processor.StartProcessingAsync();
        Console.WriteLine("🔄 Servicio 'servicioinventario' escuchando... Presiona ENTER para salir.");
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
