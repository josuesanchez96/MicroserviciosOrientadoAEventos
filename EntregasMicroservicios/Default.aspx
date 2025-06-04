<%@ Page Title="Home Page"
         Language="C#"
         MasterPageFile="~/Site.Master"
         AutoEventWireup="true"
         Async="true"
         CodeBehind="Default.aspx.cs"
         Inherits="EntregasMicroservicios._Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .form-group {
            margin-bottom: 12px;
        }

        label {
            display: block;
            font-weight: bold;
            margin-bottom: 4px;
        }

        input[type="text"], textarea {
            width: 300px;
            padding: 6px;
            border: 1px solid #ccc;
            border-radius: 4px;
        }

        textarea {
            resize: vertical;
        }

        .section-title {
            margin-top: 20px;
            font-size: 1.2em;
            font-weight: bold;
            color: #333;
        }

        .button {
            margin-top: 16px;
            padding: 8px 16px;
            font-size: 1em;
            background-color: #28a745;
            color: white;
            border: none;
            border-radius: 4px;
        }

        #lblResultado {
            margin-top: 16px;
            display: block;
            font-weight: bold;
        }
    </style>

    <div class="form-group section-title">Datos del Cliente</div>
    <div class="form-group">
        <label for="txtClienteId">Cliente ID:</label>
        <asp:TextBox ID="txtClienteId" runat="server" Text="CL123456" />
    </div>

    <div class="form-group section-title">Productos</div>
    <div class="form-group">
        <label for="txtProd1Id">Producto 1 ID:</label>
        <asp:TextBox ID="txtProd1Id" runat="server" Text="PROD01" />
        <label for="txtProd1Qty">Cantidad:</label>
        <asp:TextBox ID="txtProd1Qty" runat="server" Text="2" />
    </div>

    <div class="form-group">
        <label for="txtProd2Id">Producto 2 ID:</label>
        <asp:TextBox ID="txtProd2Id" runat="server" Text="PROD02" />
        <label for="txtProd2Qty">Cantidad:</label>
        <asp:TextBox ID="txtProd2Qty" runat="server" Text="1" />
    </div>

    <div class="form-group section-title">Dirección de Entrega</div>
    <div class="form-group">
        <label for="txtCalle">Calle:</label>
        <asp:TextBox ID="txtCalle" runat="server" Text="5a Avenida" />
    </div>
    <div class="form-group">
        <label for="txtNumero">Número:</label>
        <asp:TextBox ID="txtNumero" runat="server" Text="123" />
    </div>
    <div class="form-group">
        <label for="txtCiudad">Ciudad:</label>
        <asp:TextBox ID="txtCiudad" runat="server" Text="Ciudad de Guatemala" />
    </div>
    <div class="form-group">
        <label for="txtDepartamento">Departamento:</label>
        <asp:TextBox ID="txtDepartamento" runat="server" Text="Guatemala" />
    </div>
    <div class="form-group">
        <label for="txtReferencia">Referencia:</label>
        <asp:TextBox ID="txtReferencia" runat="server" Text="Frente a la tienda azul" />
    </div>

    <div class="form-group section-title">Pago y Notas</div>
    <div class="form-group">
        <label for="txtTipoPago">Tipo de Pago:</label>
        <asp:TextBox ID="txtTipoPago" runat="server" Text="tarjeta" />
    </div>
    <div class="form-group">
        <label for="txtNotas">Notas Adicionales:</label>
        <asp:TextBox ID="txtNotas" runat="server" Text="Por favor, entregar antes de las 5 PM" TextMode="MultiLine" Rows="3" />
    </div>

    <asp:Button ID="btnCrear" runat="server" CssClass="button" Text="Crear pedido" OnClick="btnCrear_Click" />
    <asp:Label ID="lblResultado" runat="server" ForeColor="Green" />

</asp:Content>
