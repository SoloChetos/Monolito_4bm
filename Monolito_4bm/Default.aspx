<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/PrincipalMaster.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Monolito_4bm.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        /* Estilos minimalistas para el Dashboard - Identidad GMO */
        .dashboard-header {
            color: #1a1a1a;
            letter-spacing: -0.5px;
            font-weight: 300;
        }
        .dashboard-header span {
            font-weight: 600;
        }
        
        .dash-card {
            background-color: #ffffff;
            border: 1px solid #eaeaea;
            border-radius: 6px;
            padding: 2rem 1.5rem;
            text-align: center;
            text-decoration: none;
            color: #333333;
            display: block;
            transition: all 0.3s ease;
            height: 100%;
        }
        
        .dash-card:hover {
            box-shadow: 0 10px 30px rgba(0, 0, 0, 0.05);
            border-color: #cccccc;
            color: #000000;
            transform: translateY(-3px);
        }

        .dash-card-icon {
            font-size: 2.5rem;
            color: #1a1a1a;
            margin-bottom: 1rem;
        }

        .dash-card-title {
            font-size: 1.1rem;
            font-weight: 500;
            margin-bottom: 0.5rem;
        }

        .dash-card-desc {
            font-size: 0.85rem;
            color: #888888;
        }
    </style>

    <div class="container mt-5 mb-5">
        <div class="row mb-5 text-center">
            <div class="col-12">
                <h2 class="dashboard-header">Bienvenido, <span><asp:Label ID="lblNombre" runat="server" /></span></h2>
                <p class="text-muted">Selecciona el módulo con el que deseas trabajar hoy.</p>
            </div>
        </div>

        <%-- Panel para Administrador --%>
        <asp:Panel ID="pnlAdmin" runat="server" Visible="false">
            <div class="row g-4 justify-content-center">
                
                <div class="col-12 col-sm-6 col-md-4">
                    <a href="Productos.aspx" class="dash-card">
                        <i class="fas fa-glasses dash-card-icon"></i>
                        <h3 class="dash-card-title">Productos</h3>
                        <p class="dash-card-desc">Administra el catálogo de armazones, lentes e inventario.</p>
                    </a>
                </div>

                <div class="col-12 col-sm-6 col-md-4">
                    <a href="Proveedores.aspx" class="dash-card">
                        <i class="fas fa-boxes dash-card-icon"></i>
                        <h3 class="dash-card-title">Proveedores</h3>
                        <p class="dash-card-desc">Gestiona las marcas y distribuidores de la óptica.</p>
                    </a>
                </div>

                <div class="col-12 col-sm-6 col-md-4">
                    <a href="AdminUsuarios.aspx" class="dash-card">
                        <i class="fas fa-users dash-card-icon"></i>
                        <h3 class="dash-card-title">Usuarios</h3>
                        <p class="dash-card-desc">Controla los accesos y roles del sistema.</p>
                    </a>
                </div>

            </div>
        </asp:Panel>

        <%-- Panel para Usuario Normal --%>
        <asp:Panel ID="pnlUsuario" runat="server" Visible="false">
            <div class="row justify-content-center g-4">
                
                <div class="col-12 col-sm-6 col-md-4">
                    <a href="Juego.aspx" class="dash-card">
                        <i class="fas fa-gamepad dash-card-icon"></i>
                        <h3 class="dash-card-title">Juego Interactivo</h3>
                        <p class="dash-card-desc">Accede al módulo de entretenimiento.</p>
                    </a>
                </div>
                
            </div>
        </asp:Panel>
    </div>
</asp:Content>