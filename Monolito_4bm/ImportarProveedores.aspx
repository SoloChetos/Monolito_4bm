<%@ Page Title="Importar Proveedores" Language="C#" MasterPageFile="~/PrincipalMaster.master" AutoEventWireup="true" CodeBehind="ImportarProveedores.aspx.cs" Inherits="Monolito_4bm.ImportarProveedores" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.min.css" />
    <style>
        .btn-action { background-color: #212529; color: #fff; border: none; transition: background-color 0.2s; }
        .btn-action:hover { background-color: #343a40; color: #fff; }
        .form-check-input:checked { background-color: #212529; border-color: #212529; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hfMsgType" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfMsgText" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfRedirectUrl" runat="server" ClientIDMode="Static" />

    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2 class="fw-bold text-dark m-0"><i class="fas fa-file-import me-2" style="color:#555;"></i>Importar Proveedores</h2>
        <a href="Proveedores.aspx" class="btn btn-outline-secondary btn-sm"><i class="fas fa-arrow-left me-1"></i> Volver a Proveedores</a>
    </div>

   <%-- ===== CHECKBOXES SIEMPRE VISIBLES ===== --%>
<div class="row mb-4 p-3 bg-light rounded border">
    <div class="col-md-4">
        <div class="form-check">
            <asp:CheckBox ID="chkSoloNuevos" runat="server" CssClass="form-check-input" ClientIDMode="Static" />
            <label class="form-check-label text-dark fw-medium" for="chkSoloNuevos">Solo nuevos proveedores</label>
        </div>
    </div>
    <div class="col-md-4">
        <div class="form-check">
            <asp:CheckBox ID="chkActualizarExistentes" runat="server" CssClass="form-check-input" Checked="true" ClientIDMode="Static" />
            <label class="form-check-label text-dark fw-medium" for="chkActualizarExistentes">Actualizar proveedores existentes (por ID)</label>
        </div>
    </div>
    <div class="col-md-4">
        <div class="form-check">
            <asp:CheckBox ID="chkEliminarNoIncluidos" runat="server" CssClass="form-check-input" ClientIDMode="Static" />
            <label class="form-check-label text-dark fw-medium" for="chkEliminarNoIncluidos">Eliminar proveedores que no están en el archivo</label>
        </div>
    </div>
    <div class="col-12 mt-2">
        <small class="text-danger"><i class="fas fa-info-circle"></i> Si desmarca <strong>todas</strong> las casillas, se realizará un borrado total (físico) de proveedores y se reiniciarán los IDs.</small>
        <br />
        <small class="text-muted"><i class="fas fa-lightbulb"></i> Al marcar <strong>"Solo nuevos"</strong> se ignoran los IDs del Excel y los demás checkboxes; simplemente se agregan los registros sin modificar los existentes.</small>
    </div>
</div>

    <%-- ============ PANEL DE CARGA ============ --%>
    <asp:Panel ID="pnlCarga" runat="server">
        <div class="card border-0 shadow-sm mb-4">
            <div class="card-body p-4">
                <p class="text-muted mb-4">Descargue la plantilla, complete los datos y súbala aquí. Formatos aceptados: <strong>.xlsx</strong> o <strong>.csv</strong>.</p>

                <div class="mb-4">
                    <asp:Button ID="btnDescargarPlantilla" runat="server" Text="Descargar plantilla Excel" CssClass="btn btn-outline-secondary btn-sm" OnClick="btnDescargarPlantilla_Click" />
                </div>

                <div class="mb-4">
                    <label class="form-label fw-semibold">Seleccione el archivo</label>
                    <asp:FileUpload ID="fuArchivo" runat="server" CssClass="form-control" accept=".xlsx,.xls,.csv" />
                </div>

                <div>
                    <asp:Button ID="btnCargar" runat="server" Text="Cargar y Previsualizar" CssClass="btn btn-action px-4 py-2" OnClick="btnCargar_Click" />
                </div>
            </div>
        </div>
    </asp:Panel>

    <%-- ============ PANEL DE PREVISUALIZACIÓN ============ --%>
    <asp:Panel ID="pnlPreview" runat="server" Visible="false">
        <div class="card border-0 shadow-sm">
            <div class="card-body p-4">
                <h5 class="card-title fw-bold mb-3">Previsualización de Datos</h5>
                <p class="text-muted small">Revise los datos antes de confirmar.</p>

                <div class="table-responsive mb-4" style="max-height: 400px; overflow-y: auto;">
                    <asp:GridView ID="gvPreview" runat="server" AutoGenerateColumns="True" CssClass="table table-sm table-striped table-hover align-middle border" HeaderStyle-CssClass="table-dark sticky-top" />
                </div>

                <div class="d-flex gap-2">
                    <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar Importación" CssClass="btn btn-success px-4" UseSubmitBehavior="false" OnClientClick="return confirmarImportacion(this);" OnClick="btnConfirmar_Click" />
                    <asp:Button ID="btnCancelarPreview" runat="server" Text="Cancelar" CssClass="btn btn-outline-danger px-4" OnClick="btnCancelarPreview_Click" />
                </div>
            </div>
        </div>
    </asp:Panel>

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script>

        // Deshabilitar los otros checkboxes cuando "Solo nuevos" esté marcado
        document.addEventListener('DOMContentLoaded', function () {
            var chkNuevos = document.getElementById('chkSoloNuevos');
            var chkActualizar = document.getElementById('chkActualizarExistentes');
            var chkEliminar = document.getElementById('chkEliminarNoIncluidos');
            if (chkNuevos && chkActualizar && chkEliminar) {
                chkNuevos.addEventListener('change', function () {
                    if (this.checked) {
                        chkActualizar.checked = false;
                        chkEliminar.checked = false;
                        chkActualizar.disabled = true;
                        chkEliminar.disabled = true;
                    } else {
                        chkActualizar.disabled = false;
                        chkEliminar.disabled = false;
                    }
                });
            }
        });

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

        function confirmarImportacion(btn) {
            if (btn.dataset.confirmed === 'true') return true;

            var chkActualizar = document.getElementById('chkActualizarExistentes');
            var chkEliminar = document.getElementById('chkEliminarNoIncluidos');

            if (!chkActualizar || !chkEliminar) {
                Swal.fire({
                    icon: 'error',
                    title: 'Error interno',
                    text: 'No se pudieron leer las opciones de importación. Recargue la página.',
                    confirmButtonColor: '#212529'
                });
                return false;
            }

            var actualizarMarcado = chkActualizar.checked;
            var eliminarMarcado = chkEliminar.checked;
            var mensaje = "Se procesarán los datos del archivo cargado.";
            var icono = "info";

            if (!actualizarMarcado && !eliminarMarcado) {
                mensaje = "¡ATENCIÓN! Ambas casillas están desmarcadas. Esto ELIMINARÁ TODOS LOS PROVEEDORES ACTUALES de la base de datos de forma irreversible antes de importar los nuevos.";
                icono = "warning";
            }

            Swal.fire({
                title: '¿Confirmar importación?',
                text: mensaje,
                icon: icono,
                showCancelButton: true,
                confirmButtonColor: '#212529',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Sí, importar',
                cancelButtonText: 'Cancelar'
            }).then((result) => {
                if (result.isConfirmed) {
                    __doPostBack('<%= btnConfirmar.UniqueID %>', '');
                }
            });

            return false;
        }
    </script>
</asp:Content>