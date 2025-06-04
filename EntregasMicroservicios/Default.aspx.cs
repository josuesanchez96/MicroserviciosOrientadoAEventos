using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;        // instala el paquete System.Text.Json en NuGet
using System.Threading.Tasks;
using System.Web.UI;

namespace EntregasMicroservicios
{
    public partial class _Default : Page
    {
        private static readonly HttpClient _http = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7172")
        };

        protected async void btnCrear_Click(object sender, EventArgs e)
        {
            try
            {
                var dto = new
                {
                    clienteId = txtClienteId.Text,
                    productos = new[]
                    {
                new { productoId = txtProd1Id.Text, cantidad = int.Parse(txtProd1Qty.Text) },
                new { productoId = txtProd2Id.Text, cantidad = int.Parse(txtProd2Qty.Text) }
            },
                    direccionEntrega = new
                    {
                        calle = txtCalle.Text,
                        numero = txtNumero.Text,
                        ciudad = txtCiudad.Text,
                        departamento = txtDepartamento.Text,
                        referencia = txtReferencia.Text
                    },
                    tipoPago = txtTipoPago.Text,
                    notasAdicionales = txtNotas.Text
                };

                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var resp = await _http.PostAsync("api/orders", content);

                if (resp.IsSuccessStatusCode)
                {
                    lblResultado.Text = "Pedido enviado correctamente.";
                }
                else
                {
                    lblResultado.Text = $"Error {resp.StatusCode}: {await resp.Content.ReadAsStringAsync()}";
                }
            }
            catch (Exception ex)
            {
                lblResultado.Text = $"Excepción: {ex.Message}";
            }
        }

    }
}
