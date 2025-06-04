<%@ Page Title="Consultar Datos"
         Language="C#"
         MasterPageFile="~/Site.Master"
         AutoEventWireup="true"
         CodeBehind="ConsultarDatos.aspx.cs"
         Inherits="EntregasMicroservicios.ConsultarDatos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>📊 Consulta de Bases de Datos</h2>

    <asp:Button ID="btnPagos" runat="server" Text="Ver Pedidos (Pagos)" OnClick="btnPagos_Click" CssClass="button" />
    <asp:Button ID="btnInventario" runat="server" Text="Ver Productos (Inventario)" OnClick="btnInventario_Click" CssClass="button" />
    <asp:Button ID="btnEntregas" runat="server" Text="Ver Direcciones (Entregas)" OnClick="btnEntregas_Click" CssClass="button" />

    <hr />

    <asp:GridView ID="gvDatos" runat="server" AutoGenerateColumns="true" CssClass="grid" />
</asp:Content>