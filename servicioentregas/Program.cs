using Azure.Messaging.ServiceBus;
using MySql.Data.MySqlClient;
using System;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    const string queueConnectionString = "";
    const string queueName = "servicioentregas";

    const string mysqlConnectionString = "server=localhost;user=root;password=;database=entregasdb";

    static async Task Main(string[] args)
    {
        var client = new ServiceBusClient(queueConnectionString);
        var processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());

        processor.ProcessMessageAsync += async msgArgs =>
        {
            try
            {
                string body = msgArgs.Message.Body.ToString();
                Console.WriteLine($"📦 Mensaje recibido en servicioentregas:\n{body}");

                // Deserializar solo para la dirección
                var pedido = JsonSerializer.Deserialize<PedidoDto>(body);
                var dir = pedido.DireccionEntrega;

                using var conn = new MySqlConnection(mysqlConnectionString);
                await conn.OpenAsync();

                var cmd = new MySqlCommand(
                    "INSERT INTO DireccionesEntrega (Calle, Numero, Ciudad, Departamento, Referencia) VALUES (@Calle, @Numero, @Ciudad, @Departamento, @Referencia)", conn);
                cmd.Parameters.AddWithValue("@Calle", dir.Calle);
                cmd.Parameters.AddWithValue("@Numero", dir.Numero);
                cmd.Parameters.AddWithValue("@Ciudad", dir.Ciudad);
                cmd.Parameters.AddWithValue("@Departamento", dir.Departamento);
                cmd.Parameters.AddWithValue("@Referencia", dir.Referencia);
                await cmd.ExecuteNonQueryAsync();

                Console.WriteLine("✅ Dirección insertada en entregasdb.");
                await msgArgs.CompleteMessageAsync(msgArgs.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error procesando mensaje: {ex.Message}");
            }
        };

        processor.ProcessErrorAsync += async errorArgs =>
        {
            Console.WriteLine($"❌ Error del procesador: {errorArgs.Exception.Message}");
            await Task.CompletedTask;
        };

        await processor.StartProcessingAsync();
        Console.WriteLine("🔄 Servicio 'servicioentregas' escuchando... Presiona ENTER para salir.");
        Console.ReadLine();
        await processor.StopProcessingAsync();
    }

    public class PedidoDto
    {
        public DireccionDto DireccionEntrega { get; set; }
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
