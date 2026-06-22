<%@ Page Title="Configurar OTP" Language="C#" MasterPageFile="~/Site.User.Master" AutoEventWireup="true" CodeBehind="ConfigurarOTP.aspx.cs" Inherits="Monolito_4bm.ConfigurarOTP" %>
<asp:Content ID="HeadCnt" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .otp-card { background:#fff; border-radius:12px; box-shadow:0 4px 16px rgba(0,0,0,0.1); padding:30px; max-width:500px; margin:0 auto; }
        .otp-card h3 { font-weight:700; color:#333; margin-bottom:8px; }
        .otp-card .sub { font-size:13px; color:#888; margin-bottom:20px; }
        .qr-box { text-align:center; padding:20px; background:#f8f9fa; border-radius:8px; margin-bottom:16px; }
        .qr-box img { max-width:250px; border-radius:8px; }
        .secret-box { background:#e9ecef; padding:10px; border-radius:6px; text-align:center; font-family:monospace; font-size:16px; letter-spacing:2px; font-weight:700; margin-bottom:16px; }
        .btn-otp { width:100%;padding:12px;background:linear-gradient(135deg,#28a745,#20c997);border:none;border-radius:8px;color:#fff;font-size:15px;font-weight:600;cursor:pointer; }
        .btn-otp:hover { opacity:0.9; }
        .status-active { color:#28a745; font-weight:700; }
        .status-inactive { color:#dc3545; font-weight:700; }
    </style>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hfMsg" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfMsgType" runat="server" ClientIDMode="Static" />

    <div class="otp-card">
        <h3><i class="fas fa-qrcode"></i> Autenticaci&#243;n OTP (2FA)</h3>
        <p class="sub">Escanea el c&#243;digo QR con Google Authenticator u otra app compatible.</p>

        <asp:Panel ID="pnlEstado" runat="server">
            <div class="alert alert-info">
                Estado OTP: <asp:Label ID="lblEstado" runat="server" />
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlConfigurar" runat="server">
            <asp:Button ID="btnGenerar" runat="server" Text="&#128272; Generar QR" CssClass="btn-otp mb-3" OnClick="btnGenerar_Click" />
        </asp:Panel>

        <asp:Panel ID="pnlQR" runat="server" Visible="false">
            <div class="qr-box">
                <asp:Image ID="imgQR" runat="server" />
            </div>
            <p class="text-center" style="font-size:13px;color:#666;">Secreto (manual):</p>
            <div class="secret-box">
                <asp:Label ID="lblSecret" runat="server" />
            </div>

            <div class="form-floating mb-3">
                <asp:TextBox ID="txtCodigoOTP" runat="server" CssClass="form-control" placeholder="Codigo" MaxLength="6"
                    style="font-size:20px;text-align:center;letter-spacing:6px;font-weight:700;" ClientIDMode="Static" />
                <label>C&#243;digo OTP (6 d&#237;gitos)</label>
            </div>
            <asp:Button ID="btnVerificar" runat="server" Text="&#9989; Verificar y Activar" CssClass="btn-otp" OnClick="btnVerificar_Click" />
        </asp:Panel>
    </div>
</asp:Content>
<asp:Content ID="ScriptCnt" ContentPlaceHolderID="ScriptContent" runat="server">
    <script>
        window.addEventListener('load', function() {
            var t = document.getElementById('hfMsgType').value, m = document.getElementById('hfMsg').value;
            if (t && m) { Swal.fire({ icon:t, title:t==='success'?'\u00a1Listo!':'Error', text:m, confirmButtonColor:t==='success'?'#28a745':'#d33' }); document.getElementById('hfMsgType').value=''; document.getElementById('hfMsg').value=''; }
        });
    </script>
</asp:Content>
