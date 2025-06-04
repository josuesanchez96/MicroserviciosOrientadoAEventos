using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace EntregasMicroservicios
{
    public partial class ConsultarDatos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e) { }

        private void CargarDatos(string connectionString, string query)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                var da = new MySqlDataAdapter(query, conn);
                var dt = new System.Data.DataTable();
                da.Fill(dt);
                gvDatos.DataSource = dt;
                gvDatos.DataBind();
            }
        }

        protected void btnPagos_Click(object sender, EventArgs e)
        {
            string conn = "server=localhost;user=root;password=;database=pagosdb";
            string query = "SELECT * FROM Pedidos";
            CargarDatos(conn, query);
        }

        protected void btnInventario_Click(object sender, EventArgs e)
        {
            string conn = "server=localhost;user=root;password=;database=inventariodb";
            string query = "SELECT * FROM ProductosPedido";
            CargarDatos(conn, query);
        }

        protected void btnEntregas_Click(object sender, EventArgs e)
        {
            string conn = "server=localhost;user=root;password=;database=entregasdb";
            string query = "SELECT * FROM DireccionesEntrega";
            CargarDatos(conn, query);
        }
    }
}