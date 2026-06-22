<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RecuperarClave.aspx.cs" Inherits="Monolito_4bm.RecuperarClave" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Recuperar Clave</title>
    <link href="https://fonts.googleapis.com/css2?family=Open+Sans:wght@400;600;700&amp;display=swap" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.min.css" />
    <style>
        body { font-family:'Open Sans',sans-serif; min-height:100vh; display:flex; align-items:center; justify-content:center; background:linear-gradient(135deg,#1a1a2e,#16213e,#0f3460); padding:20px; }
        .recover-container { width:100%; max-width:460px; background:#fff; border-radius:12px; box-shadow:0 8px 32px rgba(0,0,0,0.3); padding:35px 30px; }
        .icon-wrap { width:60px;height:60px;border-radius:50%;background:linear-gradient(135deg,#e8a87c,#d4956a);display:flex;align-items:center;justify-content:center;margin:0 auto 18px;box-shadow:0 4px 12px rgba(232,168,124,0.4); }
        .icon-wrap i { color:#fff;font-size:24px; }
        h2 { font-size:22px;font-weight:700;color:#333;text-align:center;margin-bottom:6px; }
        .subtitle { font-size:13px;color:#888;text-align:center;margin-bottom:24px; }
        .form-floating { margin-bottom:16px; }
        .form-floating .form-control { border-radius:8px;border:1px solid #ddd;font-size:14px; }
        .form-floating .form-control:focus { border-color:#e8a87c;box-shadow:0 0 0 3px rgba(232,168,124,0.15); }
        .btn-send { width:100%;padding:12px;background:linear-gradient(135deg,#e8a87c,#d4956a);border:none;border-radius:8px;color:#fff;font-size:15px;font-weight:600;cursor:pointer;transition:all 0.3s; }
        .btn-send:hover { background:linear-gradient(135deg,#d4956a,#c4855a);transform:translateY(-1px); }
        .btn-back { display:block;text-align:center;margin-top:14px;color:#e8a87c;text-decoration:none;font-size:14px;font-weight:600; }
        .btn-back:hover { color:#d4956a;text-decoration:underline; }
    </style>
</head>
<body>
    <form id="form1" runat="server" autocomplete="off">
        <asp:HiddenField ID="hfMsgType" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hfMsgText" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hfRedirectUrl" runat="server" ClientIDMode="Static" />

        <div class="recover-container">
            <div class="icon-wrap"><i class="fas fa-key"></i></div>
            <h2>Recuperar Contrase&#241;a</h2>
            <p class="subtitle">Ingrese su nick o correo. Se enviar&#225; una clave temporal a su email.</p>

            <div class="form-floating">
                <asp:TextBox ID="txtNickCorreo" runat="server" CssClass="form-control" placeholder="Nick o Correo" ClientIDMode="Static" autocomplete="off" />
                <label>Nick o Correo electr&#243;nico</label>
            </div>

            <asp:Button ID="btnEnviarClave" runat="server" Text="Enviar Clave Temporal" CssClass="btn-send"
                OnClick="btnEnviarClave_Click" />

            <a href="CambiarClave.aspx" class="btn-back"><i class="fas fa-exchange-alt"></i> Ya tengo mi clave temporal</a>
            <a href="Login.aspx" class="btn-back"><i class="fas fa-arrow-left"></i> Volver al Login</a>
        </div>
    </form>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script>
        window.addEventListener('load', function() {
            var t = document.getElementById('hfMsgType').value;
            var m = document.getElementById('hfMsgText').value;
            var r = document.getElementById('hfRedirectUrl').value;
            if (t && m) {
                Swal.fire({ icon: t, title: t === 'success' ? '\u00a1Enviado!' : 'Error', text: m, confirmButtonColor: t === 'success' ? '#28a745' : '#d33'
                }).then(function() { if (r) window.location.href = r; });
                document.getElementById('hfMsgType').value = ''; document.getElementById('hfMsgText').value = '';
            }
        });
    </script>
</body>
</html>
