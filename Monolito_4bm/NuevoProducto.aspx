<%@ Page Title="Nuevo Producto" Language="C#" MasterPageFile="~/PrincipalMaster.master"
    AutoEventWireup="true" CodeBehind="NuevoProducto.aspx.cs" Inherits="Monolito_4bm.NuevoProducto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.min.css" />
    <style>
        .img-miniatura { width: 100px; height: 100px; object-fit: cover; margin: 5px; border: 1px solid #ddd; }
        .delete-btn { position: absolute; top: -5px; right: -5px; }
        .contenedor-imagen { position: relative; display: inline-block; margin-right: 15px; margin-bottom: 15px; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hfMsgType" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfMsgText" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfRedirectUrl" runat="server" ClientIDMode="Static" />

    <h2 class="mb-4"><i class="fas fa-box me-2"></i>Nuevo Producto</h2>

    <div class="row">
        <div class="col-md-6">
            <div class="card">
                <div class="card-body">
                    <div class="mb-3">
                        <label class="form-label">Nombre *</label>
                        <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" />
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Cantidad *</label>
                        <asp:TextBox ID="txtCantidad" runat="server" CssClass="form-control" TextMode="Number" min="0" />
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Precio *</label>
                        <!-- Quitamos TextMode="Number" para evitar problemas con separadores -->
                        <asp:TextBox ID="txtPrecio" runat="server" CssClass="form-control" />
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Proveedor</label>
                        <asp:DropDownList ID="ddlProveedor" runat="server" CssClass="form-select" AppendDataBoundItems="True">
                            <asp:ListItem Value="">Sin proveedor</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <hr />
                    <h5>Imágenes del producto (máx. 5, 5 MB c/u)</h5>
                    <div class="mb-3">
                        <asp:FileUpload ID="fuImagen" runat="server" AllowMultiple="true" CssClass="form-control" />
                        <asp:Button ID="btnAgregarImagen" runat="server" Text="Previsualizar imagen(es)" CssClass="btn btn-sm btn-primary mt-2"
                            OnClick="btnAgregarImagen_Click" CausesValidation="false" />
                    </div>

                    <h6>Imágenes nuevas (aún no guardadas)</h6>
                    <asp:Repeater ID="rptNuevasImagenes" runat="server" OnItemCommand="rptNuevasImagenes_ItemCommand">
                        <ItemTemplate>
                            <div class="contenedor-imagen">
                                <img src='<%# Eval("DataUri") %>' class="img-miniatura" alt="Previsualización" />
                                <asp:LinkButton ID="lnkQuitarNueva" runat="server" CommandName="QuitarNueva" CommandArgument='<%# Eval("Indice") %>'
                                    CssClass="btn btn-sm btn-danger delete-btn"
                                    OnClientClick="return confirmarBorrado(this);">X</asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Label ID="lblSinNuevas" runat="server" Text="No hay imágenes." Visible="false" />

                    <hr />
                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar Producto" CssClass="btn btn-success" OnClick="btnGuardar_Click" />
                    <asp:HyperLink ID="hlCancelar" runat="server" NavigateUrl="~/Productos.aspx" CssClass="btn btn-secondary ms-2">Cancelar</asp:HyperLink>
                </div>
            </div>
        </div>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script>
        function confirmarBorrado(btn) {
            if (btn.getAttribute("data-confirmed") === "true") {
                btn.setAttribute("data-confirmed", "false");
                return true;
            }
            Swal.fire({
                title: '¿Estás seguro?',
                text: "Esta foto será eliminada.",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: 'Sí, borrar la foto',
                cancelButtonText: 'Cancelar'
            }).then((result) => {
                if (result.isConfirmed) {
                    btn.setAttribute("data-confirmed", "true");
                    btn.click();
                }
            });
            return false;
        }

        window.addEventListener('load', function () {
            var t = document.getElementById('hfMsgType').value;
            var m = document.getElementById('hfMsgText').value;
            var r = document.getElementById('hfRedirectUrl').value;
            if (t && m) {
                Swal.fire({
                    icon: t,
                    title: t === 'success' ? 'Éxito' : (t === 'error' ? 'Error' : 'Atención'),
                    text: m,
                    confirmButtonColor: '#0f3460'
                }).then(function () { if (r) window.location.href = r; });
            }
        });
    </script>
</asp:Content>