<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CambiarClave.aspx.cs" Inherits="Monolito_4bm.CambiarClave" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Cambiar Contrase&#241;a</title>
    <link href="https://fonts.googleapis.com/css2?family=Open+Sans:wght@400;600;700&amp;display=swap" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.min.css" />
    <style>
        body { font-family:'Open Sans',sans-serif; min-height:100vh; display:flex; align-items:center; justify-content:center; background:linear-gradient(135deg,#1a1a2e,#16213e,#0f3460); padding:20px; }
        .change-container { width:100%; max-width:460px; background:#fff; border-radius:12px; box-shadow:0 8px 32px rgba(0,0,0,0.3); padding:35px 30px; }
        .icon-wrap { width:60px;height:60px;border-radius:50%;background:linear-gradient(135deg,#28a745,#20c997);display:flex;align-items:center;justify-content:center;margin:0 auto 18px; }
        .icon-wrap i { color:#fff;font-size:24px; }
        h2 { font-size:22px;font-weight:700;color:#333;text-align:center;margin-bottom:6px; }
        .subtitle { font-size:13px;color:#888;text-align:center;margin-bottom:24px; }
        .form-floating { margin-bottom:16px; }
        .form-floating .form-control { border-radius:8px;border:1px solid #ddd;font-size:14px; }
        .form-floating .form-control:focus { border-color:#28a745;box-shadow:0 0 0 3px rgba(40,167,69,0.15); }
        .btn-change { width:100%;padding:12px;background:linear-gradient(135deg,#28a745,#20c997);border:none;border-radius:8px;color:#fff;font-size:15px;font-weight:600;cursor:pointer;transition:all 0.3s; }
        .btn-change:hover { opacity:0.9;transform:translateY(-1px); }
        .btn-back { display:block;text-align:center;margin-top:14px;color:#e8a87c;text-decoration:none;font-size:14px;font-weight:600; }
    </style>
</head>
<body>
    <form id="form1" runat="server" autocomplete="off">
        <asp:HiddenField ID="hfMsgType" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hfMsgText" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hfRedirectUrl" runat="server" ClientIDMode="Static" />

        <div class="change-container">
            <div class="icon-wrap"><i class="fas fa-lock-open"></i></div>
            <h2>Cambiar Contrase&#241;a</h2>
            <p class="subtitle">Ingrese la clave temporal enviada a su correo y su nueva contrase&#241;a</p>

            <div class="form-floating">
                <asp:TextBox ID="txtNick" runat="server" CssClass="form-control" placeholder="Nick" ClientIDMode="Static" autocomplete="off" />
                <label>Tu Nick</label>
            </div>
            <div class="form-floating">
                <asp:TextBox ID="txtClaveTemporal" runat="server" CssClass="form-control" placeholder="Clave temporal" ClientIDMode="Static" autocomplete="off" />
                <label>Clave Temporal</label>
            </div>
            <div class="form-floating">
                <asp:TextBox ID="txtNuevaPass" runat="server" CssClass="form-control" TextMode="Password" placeholder="Nueva" ClientIDMode="Static" autocomplete="new-password" />
                <label>Nueva Contrase&#241;a</label>
            </div>
            <div class="form-floating">
                <asp:TextBox ID="txtConfirmarPass" runat="server" CssClass="form-control" TextMode="Password" placeholder="Confirmar" ClientIDMode="Static" autocomplete="new-password" />
                <label>Confirmar Contrase&#241;a</label>
            </div>

            <asp:Button ID="btnCambiar" runat="server" Text="Cambiar Contrase&#241;a" CssClass="btn-change" OnClick="btnCambiar_Click" />
            <a href="Login.aspx" class="btn-back"><i class="fas fa-arrow-left"></i> Volver al Login</a>
        </div>
    </form>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script>
        window.addEventListener('load', function() {
            var t = document.getElementById('hfMsgType').value, m = document.getElementById('hfMsgText').value, r = document.getElementById('hfRedirectUrl').value;
            if (t && m) { Swal.fire({ icon:t, title:t==='success'?'\u00a1Listo!':'Error', text:m, confirmButtonColor:t==='success'?'#28a745':'#d33' }).then(function(){if(r)window.location.href=r;}); document.getElementById('hfMsgType').value=''; document.getElementById('hfMsgText').value=''; }
        });
    </script>
</body>
</html>
