<%@ Page Title="Mi Panel" Language="C#" MasterPageFile="~/Site.User.Master" AutoEventWireup="true" CodeBehind="UserPanel.aspx.cs" Inherits="Monolito_4bm.UserPanel" %>
<asp:Content ID="HeadCnt" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .welcome-card { background: linear-gradient(135deg, #1a1a2e, #0f3460); color: #fff; border-radius: 12px; padding: 40px; text-align: center; box-shadow: 0 8px 24px rgba(0,0,0,0.2); }
        .welcome-card h2 { font-weight: 700; margin-bottom: 10px; }
        .welcome-card .avatar-lg { width: 100px; height: 100px; border-radius: 50%; border: 4px solid #e8a87c; margin-bottom: 16px; object-fit: cover; }
        .feature-card { border-radius: 12px; border: none; box-shadow: 0 4px 12px rgba(0,0,0,0.08); transition: transform 0.3s; cursor: pointer; text-decoration: none; color: inherit; display: block; }
        .feature-card:hover { transform: translateY(-4px); box-shadow: 0 8px 20px rgba(0,0,0,0.12); color: inherit; }
        .feature-card .card-body { text-align: center; padding: 30px 20px; }
        .feature-card i { font-size: 40px; margin-bottom: 12px; }
    </style>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="welcome-card mb-4">
        <asp:Image ID="imgPerfilGrande" runat="server" CssClass="avatar-lg" />
        <h2>Bienvenido, <asp:Label ID="lblNombre" runat="server" /></h2>
        <p style="opacity:0.8;">Panel de usuario - Monolito4bm</p>
    </div>

    <div class="row g-4 mt-2">
        <div class="col-md-4">
            <a href="Juego.aspx" class="feature-card card">
                <div class="card-body">
                    <i class="fas fa-gamepad" style="color:#e8a87c;"></i>
                    <h5>Juego Memory</h5>
                    <p class="text-muted mb-0">Pon a prueba tu memoria</p>
                </div>
            </a>
        </div>
        <div class="col-md-4">
            <a href="ConfigurarOTP.aspx" class="feature-card card">
                <div class="card-body">
                    <i class="fas fa-shield-alt" style="color:#28a745;"></i>
                    <h5>Seguridad OTP</h5>
                    <p class="text-muted mb-0">Configura autenticaci&#243;n 2FA</p>
                </div>
            </a>
        </div>
        <div class="col-md-4">
            <a href="Login.aspx" class="feature-card card" onclick="<%= "Session.Clear();" %>">
                <div class="card-body">
                    <i class="fas fa-sign-out-alt" style="color:#dc3545;"></i>
                    <h5>Cerrar Sesi&#243;n</h5>
                    <p class="text-muted mb-0">Salir del sistema</p>
                </div>
            </a>
        </div>
    </div>
</asp:Content>
