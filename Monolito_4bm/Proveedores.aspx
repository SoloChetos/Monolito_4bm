<%@ Page Title="Proveedores" Language="C#" MasterPageFile="~/PrincipalMaster.master" AutoEventWireup="true" CodeBehind="Proveedores.aspx.cs" Inherits="Monolito_4bm.Proveedores" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.min.css" />
    <style>
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

        .table-wrapper { background: #fff; border-radius: 12px; box-shadow: 0 4px 16px rgba(0,0,0,0.08); padding: 25px; margin-top: 20px; }
        .table thead th { background-color: #0f3460; color: #fff; font-weight: 600; border-bottom: none; }
        .badge-active { background-color: #d4edda; color: #155724; padding: 6px 12px; border-radius: 20px; font-weight: 600; }
        .badge-inactive { background-color: #f8d7da; color: #721c24; padding: 6px 12px; border-radius: 20px; font-weight: 600; }
        .pagination-container {
            display: flex; justify-content: center; align-items: center; gap: 1rem; margin-top: 1.5rem;
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

    <h2 class="mb-4 fw-bold text-dark"><i class="fas fa-truck me-2" style="color:#555;"></i>Gestión de Proveedores</h2>

    <!-- Filtros -->
    <div class="filter-card">
        <div class="row g-3">
            <div class="col-md-4">
                <label class="form-label">Buscar por nombre</label>
                <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control" placeholder="Nombre del proveedor..." />
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
                <label class="form-label">Ordenar</label>
                <asp:DropDownList ID="ddlOrden" runat="server" CssClass="form-select">
                    <asp:ListItem Value="recientes">Más recientes</asp:ListItem>
                    <asp:ListItem Value="normal">Orden normal (A-Z)</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-2">
                <asp:Button ID="btnBuscar" runat="server" Text="Filtrar" CssClass="btn btn-action w-100" OnClick="btnBuscar_Click" />
            </div>
        </div>
    </div>

    <!-- Acciones masivas -->
    <div class="d-flex flex-wrap gap-2 mb-4">
        <asp:Button ID="btnNuevo" runat="server" Text="+ Nuevo Proveedor" CssClass="btn btn-action btn-action-success" OnClick="btnNuevo_Click" />
        <asp:Button ID="btnImportar" runat="server" Text="Importar Excel" CssClass="btn btn-action btn-action-info" OnClick="btnImportar_Click" />
        <asp:Button ID="btnEliminarSeleccion" runat="server" Text="Eliminar seleccionados" CssClass="btn btn-action btn-action-warning" OnClientClick="return false;" OnClick="btnEliminarSeleccion_Click" />
        <asp:Button ID="btnEliminarTodos" runat="server" Text="Eliminar todos" CssClass="btn btn-action btn-action-danger" OnClientClick="return false;" OnClick="btnEliminarTodos_Click" />
    </div>

    <!-- Tabla de proveedores -->
    <div class="table-wrapper table-responsive">
        <asp:GridView ID="gvProveedores" runat="server" AutoGenerateColumns="False" AllowPaging="False"
            CssClass="table table-hover align-middle" GridLines="None"
            DataKeyNames="prov_id"
            EmptyDataText="No hay proveedores registrados."
            OnRowCommand="gvProveedores_RowCommand"
            OnRowDataBound="gvProveedores_RowDataBound">
            <Columns>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:CheckBox ID="chkTodos" runat="server" onclick="javascript:SelectAll(this);" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:CheckBox ID="chkSeleccion" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="prov_id" HeaderText="ID" ItemStyle-Width="60px" />
                <asp:BoundField DataField="prov_nombre" HeaderText="Nombre" />
                <asp:TemplateField HeaderText="Estado" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label ID="lblEstado" runat="server"
                            Text='<%# Eval("prov_estado").ToString() == "A" ? "Activo" : "Inactivo" %>'
                            CssClass='<%# Eval("prov_estado").ToString() == "A" ? "badge-active" : "badge-inactive" %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Acciones" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <a href='ProveedorEditar.aspx?id=<%# Eval("prov_id") %>' class="btn btn-sm btn-primary" title="Editar"><i class="fas fa-edit"></i></a>
                        <asp:LinkButton ID="lnkToggleEstado" runat="server" CssClass="btn btn-sm btn-secondary"
                            CommandName="ToggleEstado" CommandArgument='<%# Eval("prov_id") + "|" + Eval("prov_estado") %>'
                            ToolTip='<%# Eval("prov_estado").ToString() == "A" ? "Desactivar" : "Activar" %>'>
                            <i class='<%# Eval("prov_estado").ToString() == "A" ? "fas fa-toggle-on" : "fas fa-toggle-off" %>'></i>
                        </asp:LinkButton>
                        <asp:LinkButton ID="lnkEliminarFisico" runat="server" CssClass="btn btn-sm btn-danger"
                            CommandName="EliminarFisico" CommandArgument='<%# Eval("prov_id") %>'
                            ToolTip="Eliminar físicamente">
                            <i class="fas fa-trash"></i>
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <!-- Paginación manual -->
    <div class="pagination-container" id="pnlPaginacion" runat="server">
        <asp:Button ID="btnPagAnterior" runat="server" Text="← Anterior" CssClass="btn-pager" OnClick="btnPagAnterior_Click" />
        <span class="page-number">Página <asp:Literal ID="litPaginaActual" runat="server" /> de <asp:Literal ID="litTotalPaginas" runat="server" /></span>
        <asp:Button ID="btnPagSiguiente" runat="server" Text="Siguiente →" CssClass="btn-pager" OnClick="btnPagSiguiente_Click" />
    </div>

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script>
        // ── Mostrar mensajes del servidor ──────────────────────────
        window.addEventListener('load', function () {
            var t = document.getElementById('hfMsgType').value;
            var m = document.getElementById('hfMsgText').value;
            var r = document.getElementById('hfRedirectUrl').value;
            if (t && m) {
                Swal.fire({
                    icon: t,
                    title: t === 'success' ? 'Éxito' : (t === 'error' ? 'Error' : 'Atención'),
                    text: m,
                    confirmButtonColor: '#212529'
                }).then(function () { if (r) window.location.href = r; });
                document.getElementById('hfMsgType').value = '';
                document.getElementById('hfMsgText').value = '';
                document.getElementById('hfRedirectUrl').value = '';
            }
        });

        function SelectAll(checkbox) {
            var grid = document.getElementById('<%= gvProveedores.ClientID %>');
            var inputs = grid.getElementsByTagName('input');
            for (var i = 0; i < inputs.length; i++) {
                if (inputs[i].type === 'checkbox') {
                    inputs[i].checked = checkbox.checked;
                }
            }
        }

        // ── Eliminar seleccionados / todos ─────────────────────────
        document.addEventListener('DOMContentLoaded', function () {
            var btnElimSel = document.getElementById('<%= btnEliminarSeleccion.ClientID %>');
            if (btnElimSel) {
                btnElimSel.addEventListener('click', function (e) {
                    e.preventDefault();
                    var checks = document.querySelectorAll('#<%= gvProveedores.ClientID %> input[type=checkbox]:checked');
                    // Ignorar el checkbox del encabezado
                    var seleccionados = Array.from(checks).filter(function(c) {
                        return c.id.indexOf('chkSeleccion') > -1;
                    });
                    if (seleccionados.length === 0) {
                        Swal.fire({
                            icon: 'warning',
                            title: 'Sin selección',
                            text: 'Seleccione por lo menos un proveedor.',
                            confirmButtonColor: '#212529'
                        });
                        return;
                    }
                    Swal.fire({
                        title: '¿Eliminar seleccionados?',
                        text: "Se eliminarán físicamente los proveedores marcados. Los productos asociados quedarán 'Sin proveedor'.",
                        showCancelButton: true,
                        confirmButtonColor: '#d33',
                        confirmButtonText: 'Sí, eliminar'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            __doPostBack('<%= btnEliminarSeleccion.UniqueID %>', '');
                        }
                    });
                });
            }

            var btnElimTodos = document.getElementById('<%= btnEliminarTodos.ClientID %>');
            if (btnElimTodos) {
                btnElimTodos.addEventListener('click', function (e) {
                    e.preventDefault();
                    Swal.fire({
                        title: '¿Eliminar TODOS los proveedores?',
                        text: "Se borrarán físicamente todos los proveedores y se reiniciará el contador de IDs. Los productos asociados quedarán 'Sin proveedor'.",
                        showCancelButton: true,
                        confirmButtonColor: '#d33',
                        confirmButtonText: 'Sí, eliminar todo'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            __doPostBack('<%= btnEliminarTodos.UniqueID %>', '');
                        }
                    });
                });
            }
        });

        function confirmarToggle(boton, estadoActual) {
            var accion = estadoActual === 'A' ? 'desactivar' : 'activar';
            var mensaje = estadoActual === 'A' ? '¿Está seguro de poner el proveedor inactivo?' : '¿Está seguro de poner el proveedor activo?';
            Swal.fire({
                title: '¿Cambiar estado?', text: mensaje,
                showCancelButton: true, confirmButtonColor: '#0f3460', confirmButtonText: 'Sí, ' + accion
            }).then((result) => { if (result.isConfirmed) __doPostBack(boton, ''); });
            return false;
        }

        function confirmarEliminacionFisica(boton) {
            Swal.fire({
                title: '¿Eliminar físicamente?', text: "Esta acción borrará el proveedor permanentemente. Los productos asociados quedarán 'Sin proveedor'.",
                showCancelButton: true, confirmButtonColor: '#d33', confirmButtonText: 'Sí, eliminar'
            }).then((result) => { if (result.isConfirmed) __doPostBack(boton, ''); });
            return false;
        }
    </script>
</asp:Content>