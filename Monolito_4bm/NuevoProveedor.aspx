<%@ Page Title="Nuevo Proveedor" Language="C#" MasterPageFile="~/PrincipalMaster.master" AutoEventWireup="true" CodeBehind="NuevoProveedor.aspx.cs" Inherits="Monolito_4bm.NuevoProveedor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.min.css" />
    <style>
        /* Hereda la tipografía y colores de la master */
        .page-container {
            max-width: 600px;
            margin: 0 auto;
        }
        .card-nuevo {
            background: #ffffff;
            border: 1px solid #eaeaea;
            border-radius: 8px;
            padding: 2rem;
        }
        .form-label {
            font-weight: 500;
            color: #555;
            font-size: 0.9rem;
            margin-bottom: 0.4rem;
        }
        .form-control {
            border: 1px solid #eaeaea;
            border-radius: 4px;
            font-size: 0.95rem;
            color: #212529;
            background-color: #fff;
            padding: 0.6rem 0.9rem;
        }
        .form-control:focus {
            border-color: #212529;
            box-shadow: none;
        }
        .btn-dark {
            background-color: #212529;
            border: 1px solid #212529;
            color: #fff;
            font-weight: 500;
            border-radius: 4px;
            padding: 0.6rem 1.8rem;
            transition: all 0.2s;
        }
        .btn-dark:hover {
            background-color: #343a40;
            border-color: #343a40;
        }
        .back-link {
            color: #777;
            font-size: 0.9rem;
            text-decoration: none;
        }
        .back-link:hover {
            color: #333;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hfMsgType" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfMsgText" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfRedirectUrl" runat="server" ClientIDMode="Static" />

    <div class="page-container mt-4">
        <nav aria-label="breadcrumb" class="mb-4">
            <ol class="breadcrumb" style="background:none; padding:0; margin:0;">
                <li class="breadcrumb-item"><a href="Default.aspx" class="text-secondary text-decoration-none">Inicio</a></li>
                <li class="breadcrumb-item"><a href="Proveedores.aspx" class="text-secondary text-decoration-none">Proveedores</a></li>
                <li class="breadcrumb-item active text-dark" aria-current="page">Nuevo proveedor</li>
            </ol>
        </nav>

        <div class="card-nuevo">
            <h1 class="h5 fw-bold text-dark mb-4">
                <i class="fas fa-truck me-2" style="color:#555;"></i>Registrar nuevo proveedor
            </h1>

            <div class="mb-4">
                <label for="<%= txtNombre.ClientID %>" class="form-label">Nombre del proveedor <span class="text-danger">*</span></label>
                <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" placeholder="Ej: Óptica Visión Clara S.A." />
                <div id="errorNombre" class="invalid-feedback" style="display:none;">El nombre es obligatorio.</div>
            </div>

            <div class="d-flex justify-content-between align-items-center">
                <asp:Button ID="btnGuardar" runat="server" Text="Guardar proveedor" CssClass="btn btn-dark" OnClick="btnGuardar_Click" />
                <asp:HyperLink ID="hlCancelar" runat="server" NavigateUrl="~/Proveedores.aspx" CssClass="back-link">
                    <i class="fas fa-arrow-left me-1"></i>Cancelar y volver
                </asp:HyperLink>
            </div>
        </div>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script>
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
            }
        });
    </script>
</asp:Content>