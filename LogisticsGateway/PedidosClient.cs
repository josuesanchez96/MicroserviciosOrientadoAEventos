using System.Net.Http.Json;

namespace LogisticsGateway;

public class PedidosClient
{
    private readonly HttpClient _http;
    public PedidosClient(HttpClient http) => _http = http;

    public async Task<Guid> CrearPedidoAsync(object dto)
    {
        var resp = await _http.PostAsJsonAsync("api/pedidos", dto);
        resp.EnsureSuccessStatusCode();
        var body = await resp.Content.ReadFromJsonAsync<CreatePedidoResponse>();
        return body!.Id;
    }

    private record CreatePedidoResponse(Guid Id, decimal Total);
}
