<%@ Page Title="Editar Producto" Language="C#" MasterPageFile="~/PrincipalMaster.master"
    AutoEventWireup="true" CodeBehind="ProductoEditar.aspx.cs" Inherits="Monolito_4bm.ProductoEditar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.min.css" />
    <style>
        .img-miniatura { width: 100px; height: 100px; object-fit: cover; margin: 5px; border: 1px solid #ddd; }
        .delete-btn { position: absolute; top: -5px; right: -5px; }
        .contenedor-imagen { position: relative; display: inline-block; margin-right: 15px; margin-bottom: 15px; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hfMsgType"     runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfMsgText"     runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfRedirectUrl" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfProducId"    runat="server" Value="0" />

    <h2 class="mb-4"><i class="fas fa-box me-2"></i>Editar Producto</h2>

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
                        <asp:TextBox ID="txtCantidad" runat="server" CssClass="form-control"
                                     TextMode="Number" min="0" />
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Precio *</label>
                        <asp:TextBox ID="txtPrecio" runat="server" CssClass="form-control"
                                     TextMode="Number" step="0.01" min="0" />
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Proveedor</label>
                        <asp:DropDownList ID="ddlProveedor" runat="server" CssClass="form-select"
                                          AppendDataBoundItems="True">
                            <asp:ListItem Value="">Sin proveedor</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <hr />
                    <h5>Imágenes actuales</h5>
                    <asp:Repeater ID="rptImagenesExistentes" runat="server"
                                  OnItemCommand="rptImagenesExistentes_ItemCommand">
                        <ItemTemplate>
                            <div class="contenedor-imagen">
                                <img src='<%# ResolveUrl("~/" + Eval("img_ruta")) %>' 
                                     class="img-miniatura" 
                                     alt="Imagen actual" />
                                <asp:LinkButton ID="lnkEliminarExistente" runat="server"
                                                CommandName="EliminarExistente"
                                                CommandArgument='<%# Container.ItemIndex %>'
                                                CssClass="btn btn-sm btn-danger delete-btn"
                                                OnClientClick="return confirmarBorrado(this);">X</asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Label ID="lblSinImagenes" runat="server" Text="No hay imágenes." Visible="false" />

                    <h5 class="mt-3">Agregar nuevas imágenes</h5>
                    <div class="mb-3">
                        <asp:FileUpload ID="fuImagen" runat="server" CssClass="form-control" AllowMultiple="true" />
                        <asp:Button ID="btnAgregarImagen" runat="server"
                                    Text="Previsualizar imagen(es)" CssClass="btn btn-sm btn-primary mt-2"
                                    OnClick="btnAgregarImagen_Click" CausesValidation="false" />
                    </div>

                    <h6>Imágenes nuevas (aún no guardadas)</h6>
                    <asp:Repeater ID="rptNuevasImagenes" runat="server"
                                  OnItemCommand="rptNuevasImagenes_ItemCommand">
                        <ItemTemplate>
                            <div class="contenedor-imagen">
                                <img src='<%# Eval("DataUri") %>' class="img-miniatura" alt="Previsualización" />
                                <asp:LinkButton ID="lnkQuitarNueva" runat="server"
                                                CommandName="QuitarNueva"
                                                CommandArgument='<%# Eval("Indice") %>'
                                                CssClass="btn btn-sm btn-danger delete-btn"
                                                OnClientClick="return confirmarBorrado(this);">X</asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Label ID="lblSinNuevas" runat="server" Text="No hay imágenes nuevas." Visible="false" />

                    <hr />
                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar Cambios"
                                CssClass="btn btn-success" OnClick="btnGuardar_Click" />
                    <asp:HyperLink ID="hlCancelar" runat="server"
                                   NavigateUrl="~/Productos.aspx"
                                   CssClass="btn btn-secondary ms-2">Cancelar</asp:HyperLink>
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
            var hfType = document.getElementById('hfMsgType');
            var hfText = document.getElementById('hfMsgText');
            var hfUrl = document.getElementById('hfRedirectUrl');

            var t = hfType ? hfType.value : '';
            var m = hfText ? hfText.value : '';
            var r = hfUrl ? hfUrl.value : '';

            if (t && m) {
                hfType.value = '';
                hfText.value = '';
                hfUrl.value = '';

                Swal.fire({
                    icon: t,
                    title: t === 'success' ? 'Éxito' : (t === 'error' ? 'Error' : 'Atención'),
                    text: m,
                    confirmButtonColor: '#0f3460'
                }).then(function () {
                    if (r) window.location.href = r;
                });
            }
        });
    </script>
</asp:Content>