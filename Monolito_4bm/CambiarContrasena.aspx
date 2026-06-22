<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CambiarContrasena.aspx.cs" Inherits="Monolito_4bm.CambiarContrasena" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Cambiar Contraseña</title>
    <link href="https://fonts.googleapis.com/css2?family=Open+Sans:wght@400;600;700&display=swap" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.min.css" />
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body {
            font-family: 'Open Sans', sans-serif; min-height: 100vh;
            display: flex; align-items: center; justify-content: center;
            background: linear-gradient(135deg, #1a1a2e 0%, #16213e 50%, #0f3460 100%);
            padding: 20px;
        }
        .change-wrapper { width: 100%; max-width: 450px; }
        .change-card {
            background: #fff; border-radius: 16px;
            box-shadow: 0 8px 32px rgba(0,0,0,0.3);
            padding: 35px 30px; text-align: center;
        }
        .form-control:focus { border-color: #0f3460; box-shadow: 0 0 0 0.2rem rgba(15,52,96,0.15); }
        .btn-primary { background: #0f3460; border: none; }
        .btn-primary:hover { background: #1a4a80; }
        .lock-icon {
            width: 56px; height: 56px;
            background: linear-gradient(135deg, #e8a87c, #d4956a);
            border-radius: 50%; display: flex; align-items: center; justify-content: center;
            margin: 0 auto 18px; box-shadow: 0 4px 12px rgba(232,168,124,0.4);
        }
        .lock-icon i { color: #fff; font-size: 22px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:HiddenField ID="hfMsgType" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hfMsgText" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hfRedirectUrl" runat="server" ClientIDMode="Static" />

        <div class="change-wrapper">
            <div class="change-card">
                <div class="lock-icon"><i class="fas fa-key"></i></div>
                <h3 class="mb-3">Cambiar contraseña</h3>
                <p class="text-muted mb-4">Ingresa la clave temporal que recibiste por WhatsApp y define tu nueva contraseña.</p>

                <div class="mb-3 text-start">
                    <label class="form-label">Clave temporal</label>
                    <asp:TextBox ID="txtClaveTemporal" runat="server" CssClass="form-control" required="required" />
                </div>
                <div class="mb-3 text-start">
                    <label class="form-label">Nueva contraseña</label>
                    <asp:TextBox ID="txtNuevaPassword" runat="server" CssClass="form-control" TextMode="Password" required="required" />
                </div>
                <div class="mb-3 text-start">
                    <label class="form-label">Confirmar nueva contraseña</label>
                    <asp:TextBox ID="txtConfirmarPassword" runat="server" CssClass="form-control" TextMode="Password" required="required" />
                </div>

                <asp:Button ID="btnCambiar" runat="server" Text="Cambiar contraseña" CssClass="btn btn-primary w-100" OnClick="btnCambiar_Click" />
                
                <br /><br />
                <a href="Login.aspx" style="color:#e8a87c; font-size:13px;">Volver al Login</a>
            </div>
        </div>
    </form>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script>
        window.addEventListener('load', function () {
            var t = document.getElementById('hfMsgType').value;
            var m = document.getElementById('hfMsgText').value;
            var r = document.getElementById('hfRedirectUrl').value;
            if (t && m) {
                Swal.fire({
                    icon: t,
                    title: t === 'success' ? '¡Éxito!' : 'Error',
                    text: m,
                    confirmButtonColor: '#0f3460'
                }).then(function () { if (r) window.location.href = r; });
            }
        });
    </script>
</body>
</html>