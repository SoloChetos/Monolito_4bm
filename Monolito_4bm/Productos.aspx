<%@ Page Title="Productos" Language="C#" MasterPageFile="~/PrincipalMaster.master" AutoEventWireup="true" CodeBehind="Productos.aspx.cs" Inherits="Monolito_4bm.Productos" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.min.css" />
    <style>
        /* ========== Estilos (sin cambios) ========== */
        .page-container { background: transparent; }
        .filter-card {
            background: #fff; border: 1px solid #eaeaea; border-radius: 8px;
            padding: 1.5rem; margin-bottom: 1.5rem;
        }
        .filter-card .form-label { font-weight: 500; color: #555; font-size: 0.85rem; margin-bottom: 0.3rem; }
        .filter-card .form-control, .filter-card .form-select {
            border: 1px solid #eaeaea; border-radius: 4px; font-size: 0.9rem;
            color: #212529; background-color: #fff; padding: 0.5rem 0.75rem;
        }
        .filter-card .form-control:focus, .filter-card .form-select:focus { border-color: #212529; box-shadow: none; }
        .btn-action {
            background-color: #1a1a1a; border: 1px solid #1a1a1a; color: #fff;
            font-weight: 500; border-radius: 4px; padding: 0.5rem 1.2rem; font-size: 0.9rem;
            transition: all 0.2s; margin-right: 0.5rem; letter-spacing: 0.3px;
        }
        .btn-action:hover { background-color: #333; border-color: #333; color: #fff; }
        .btn-action-success { background-color: #1a1a1a; border-color: #1a1a1a; color: #fff; }
        .btn-action-success:hover { background-color: #000; border-color: #000; }
        .btn-action-info { background-color: transparent; border-color: #ccc; color: #333; }
        .btn-action-info:hover { background-color: #f8f9fa; border-color: #1a1a1a; color: #1a1a1a; }
        .btn-action-warning { background-color: transparent; border-color: #ccc; color: #555; }
        .btn-action-warning:hover { background-color: #f8f9fa; border-color: #555; color: #1a1a1a; }
        .btn-action-danger { background-color: transparent; border-color: #e0b4b4; color: #d9534f; }
        .btn-action-danger:hover { background-color: #fff8f8; border-color: #d9534f; color: #c9302c; }
        .btn-icon {
            display: inline-flex; align-items: center; justify-content: center;
            width: 32px; height: 32px; padding: 0; border-radius: 50%;
            font-size: 0.95rem; margin: 0 2px; transition: all 0.2s;
            background-color: transparent; border: none;
        }
        .btn-icon-info, .btn-icon-dark { color: #666; }
        .btn-icon-info:hover, .btn-icon-dark:hover { background-color: #f0f0f0; color: #1a1a1a; }
        .btn-icon-secondary { color: #888; }
        .btn-icon-secondary:hover { background-color: #f0f0f0; color: #333; }
        .btn-icon-danger { color: #c9302c; }
        .btn-icon-danger:hover { background-color: #fff0f0; color: #ac2925; }
        .product-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(240px, 1fr));
            gap: 1.5rem;
            margin-top: 1rem;
        }
        .gmo-card {
            background: #fff; border: 1px solid #eaeaea; border-radius: 4px;
            display: flex; flex-direction: column; position: relative;
            transition: box-shadow 0.3s, border-color 0.3s;
        }
        .gmo-card:hover { box-shadow: 0 8px 24px rgba(0,0,0,0.06); border-color: #ccc; }
        .gmo-card-badge {
            position: absolute; top: 10px; left: 10px;
            background: #f8f9fa; color: #555; font-size: 0.75rem; font-weight: 600;
            padding: 4px 8px; border: 1px solid #eaeaea; border-radius: 2px;
            text-transform: uppercase; z-index: 2;
        }
        .gmo-card-select {
            position: absolute; top: 10px; right: 10px; z-index: 2;
            transform: scale(1.2);
        }
        .gmo-card-img-container {
            height: 180px; display: flex; align-items: center; justify-content: center;
            background: #fff; border-bottom: 1px solid #f5f5f5; padding: 1rem;
        }
        .gmo-card-img {
            max-width: 100%; max-height: 100%; object-fit: contain;
        }
        .gmo-card-body {
            padding: 1rem; flex-grow: 1; text-align: center;
        }
        .gmo-card-brand {
            font-size: 0.75rem; color: #888; text-transform: uppercase;
            letter-spacing: 1px; margin-bottom: 0.2rem;
        }
        .gmo-card-title {
            font-size: 0.95rem; font-weight: 500; color: #1a1a1a; margin-bottom: 0.3rem;
        }
        .gmo-card-price {
            font-size: 1.1rem; font-weight: 700; color: #1a1a1a; margin-bottom: 0;
        }
        .gmo-card-footer {
            padding: 0.6rem 1rem; background: #fcfcfc; border-top: 1px solid #f0f0f0;
            display: flex; justify-content: center; gap: 4px;
        }
        .pagination-container {
            display: flex; justify-content: center; align-items: center; gap: 1rem;
            margin-top: 2rem;
        }
        .pagination-container .btn-pager {
            background: transparent; border: 1px solid #eaeaea; color: #333;
            border-radius: 4px; padding: 0.4rem 1rem;
        }
        .pagination-container .btn-pager:hover { background: #f8f9fa; }
        .pagination-container .page-number { color: #666; font-size: 0.9rem; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hfMsgType" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfMsgText" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfRedirectUrl" runat="server" ClientIDMode="Static" />
    <asp:ScriptManager ID="ScriptManager1" runat="server" />

    <h2 class="mb-4 fw-bold text-dark"><i class="fas fa-boxes me-2" style="color:#555;"></i>Gestión de Productos</h2>

    <!-- Filtros -->
    <div class="filter-card">
        <div class="row g-3">
            <div class="col-md-3">
                <label class="form-label">Buscar por nombre</label>
                <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control" placeholder="Nombre del producto..." />
            </div>
            <div class="col-md-2">
                <label class="form-label">Proveedor</label>
                <asp:DropDownList ID="ddlProveedorFiltro" runat="server" CssClass="form-select" AppendDataBoundItems="True">
                    <asp:ListItem Value="">Todos los proveedores</asp:ListItem>
                    <asp:ListItem Value="-1">Sin proveedor</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-2">
                <label class="form-label">Estado</label>
                <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select">
                    <asp:ListItem Value="todos">Todos</asp:ListItem>
                    <asp:ListItem Value="activos" Selected="True">Activos</asp:ListItem>
                    <asp:ListItem Value="inactivos">Inactivos</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-2">
                <label class="form-label">Precio mín.</label>
                <asp:TextBox ID="txtPrecioMin" runat="server" CssClass="form-control" placeholder="0.00" />
            </div>
            <div class="col-md-2">
                <label class="form-label">Precio máx.</label>
                <asp:TextBox ID="txtPrecioMax" runat="server" CssClass="form-control" placeholder="0.00" />
            </div>
            <div class="col-md-2">
                <label class="form-label">Ordenar</label>
                <asp:DropDownList ID="ddlOrden" runat="server" CssClass="form-select">
                    <asp:ListItem Value="recientes">Más recientes</asp:ListItem>
                    <asp:ListItem Value="antiguos">Más antiguos</asp:ListItem>
                    <asp:ListItem Value="precio_asc">Precio ascendente</asp:ListItem>
                    <asp:ListItem Value="precio_desc">Precio descendente</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-1 d-flex align-items-end">
                <asp:Button ID="btnBuscar" runat="server" Text="Filtrar" CssClass="btn btn-action w-100" OnClick="btnBuscar_Click" />
            </div>
        </div>
    </div>

    <!-- Acciones masivas -->
    <div class="d-flex flex-wrap gap-2 mb-4">
        <asp:Button ID="btnNuevo" runat="server" Text="+ Nuevo Producto" CssClass="btn btn-action btn-action-success" OnClick="btnNuevo_Click" />
        <asp:Button ID="btnImportar" runat="server" Text="Importar Excel" CssClass="btn btn-action btn-action-info" OnClick="btnImportar_Click" />
        <asp:Button ID="btnEliminarSeleccion" runat="server" Text="Eliminar seleccionados" CssClass="btn btn-action btn-action-warning" OnClientClick="return false;" OnClick="btnEliminarSeleccion_Click" />
        <asp:Button ID="btnEliminarTodos" runat="server" Text="Eliminar todos" CssClass="btn btn-action btn-action-danger" OnClientClick="return false;" OnClick="btnEliminarTodos_Click" />
    </div>

    <!-- Contenido dinámico -->
    <asp:UpdatePanel ID="upProductos" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="btnEliminarSeleccion" />
            <asp:PostBackTrigger ControlID="btnEliminarTodos" />
            <asp:PostBackTrigger ControlID="btnBuscar" />
            <asp:PostBackTrigger ControlID="btnPagAnterior" />
            <asp:PostBackTrigger ControlID="btnPagSiguiente" />
            <asp:PostBackTrigger ControlID="btnNuevo" />
            <asp:PostBackTrigger ControlID="btnImportar" />
        </Triggers>
        <ContentTemplate>
            <div class="product-grid">
                <asp:Repeater ID="rptProductos" runat="server"
                    OnItemCommand="rptProductos_ItemCommand"
                    OnItemDataBound="rptProductos_ItemDataBound">
                    <ItemTemplate>
                        <div class="gmo-card">
                            <span class="gmo-card-badge">Stock: <%# Eval("pro_cantidad") %></span>
                            <asp:CheckBox ID="chkSeleccion" runat="server" CssClass="gmo-card-select" data-proid='<%# Eval("pro_id") %>' />
                            <div class="gmo-card-img-container">
                                <asp:Image ID="imgPrincipal" runat="server" CssClass="gmo-card-img" ImageUrl='<%# ObtenerUrlImagen(Eval("pro_imagen_principal")) %>' />
                            </div>
                            <div class="gmo-card-body">
                                <p class="gmo-card-brand"><%# ObtenerProveedor(Eval("prov_nombre")) %></p>
                                <h5 class="gmo-card-title"><%# Eval("pro_nombre") %></h5>
                                <p class="gmo-card-price"><%# Eval("pro_precio", "{0:C}") %></p>
                                <small class="text-muted">ID: <%# Eval("pro_id") %></small>
                            </div>
                            <div class="gmo-card-footer">
                                <a href='ProductoDetalle.aspx?id=<%# Eval("pro_id") %>' class="btn btn-icon btn-icon-info" title="Ver"><i class="fas fa-eye"></i></a>
                                <a href='ProductoEditar.aspx?id=<%# Eval("pro_id") %>' class="btn btn-icon btn-icon-dark" title="Editar"><i class="fas fa-edit"></i></a>
                                <asp:LinkButton ID="lnkToggleEstado" runat="server" CssClass="btn btn-icon btn-icon-secondary"
                                    CommandName="ToggleEstado" CommandArgument='<%# Eval("pro_id") + "|" + Eval("pro_estado") %>'
                                    ToolTip='<%# Eval("pro_estado").ToString() == "A" ? "Desactivar" : "Activar" %>'>
                                    <i id="iconoToggle" runat="server"></i>
                                </asp:LinkButton>
                                <asp:LinkButton ID="lnkEliminarFisico" runat="server" CssClass="btn btn-icon btn-icon-danger"
                                    CommandName="EliminarFisico" CommandArgument='<%# Eval("pro_id") %>'
                                    ToolTip="Eliminar físicamente">
                                    <i class="fas fa-trash-alt"></i>
                                </asp:LinkButton>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Label ID="lblSinResultados" runat="server" CssClass="text-muted" Visible="false" Text="No se encontraron productos." />
            </div>

            <div class="pagination-container" id="pnlPaginacion" runat="server">
                <asp:Button ID="btnPagAnterior" runat="server" Text="← Anterior" CssClass="btn-pager" OnClick="btnPagAnterior_Click" />
                <span class="page-number">Página <asp:Literal ID="litPaginaActual" runat="server" /> de <asp:Literal ID="litTotalPaginas" runat="server" /></span>
                <asp:Button ID="btnPagSiguiente" runat="server" Text="Siguiente →" CssClass="btn-pager" OnClick="btnPagSiguiente_Click" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script>
        // Confirmaciones para botones masivos
        document.addEventListener('DOMContentLoaded', function () {
            var btnElimSel = document.getElementById('<%= btnEliminarSeleccion.ClientID %>');
            if (btnElimSel) {
                btnElimSel.addEventListener('click', function (e) {
                    e.preventDefault();
                    var checks = document.querySelectorAll('.gmo-card-select input[type=checkbox]:checked');
                    if (checks.length === 0) {
                        Swal.fire({
                            icon: 'warning',
                            title: 'Sin selección',
                            text: 'Seleccione por lo menos un producto.',
                            confirmButtonColor: '#212529'
                        });
                        return;
                    }
                    Swal.fire({
                        title: '¿Eliminar seleccionados?',
                        text: "Se eliminarán físicamente los productos marcados.",
                        showCancelButton: true,
                        confirmButtonColor: '#212529',
                        confirmButtonText: 'Sí, eliminar'
                    }).then((result) => { if (result.isConfirmed) __doPostBack('<%= btnEliminarSeleccion.UniqueID %>', ''); });
                });
            }

            var btnElimTodos = document.getElementById('<%= btnEliminarTodos.ClientID %>');
            if (btnElimTodos) {
                btnElimTodos.addEventListener('click', function (e) {
                    e.preventDefault();
                    Swal.fire({
                        title: '¿Eliminar TODOS los productos?',
                        text: "Se borrarán físicamente todos los registros y se reiniciará el contador de IDs.",
                        showCancelButton: true,
                        confirmButtonColor: '#212529',
                        confirmButtonText: 'Sí, eliminar todo'
                    }).then((result) => { if (result.isConfirmed) __doPostBack('<%= btnEliminarTodos.UniqueID %>', ''); });
                });
            }
        });

        function confirmarToggle(boton, estadoActual) {
            var accion = estadoActual === 'A' ? 'desactivar' : 'activar';
            var mensaje = estadoActual === 'A' ? '¿Está seguro de poner el producto inactivo?' : '¿Está seguro de poner el producto activo?';
            Swal.fire({
                title: '¿Cambiar estado?', text: mensaje,
                showCancelButton: true, confirmButtonColor: '#212529', confirmButtonText: 'Sí, ' + accion
            }).then((result) => { if (result.isConfirmed) __doPostBack(boton, ''); });
            return false;
        }

        function confirmarEliminacionFisica(boton) {
            Swal.fire({
                title: '¿Eliminar físicamente?', text: "Esta acción borrará el producto y sus imágenes permanentemente.",
                showCancelButton: true, confirmButtonColor: '#212529', confirmButtonText: 'Sí, eliminar'
            }).then((result) => { if (result.isConfirmed) __doPostBack(boton, ''); });
            return false;
        }
    </script>
</asp:Content>