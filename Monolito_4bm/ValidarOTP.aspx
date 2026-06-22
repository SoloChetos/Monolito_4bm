<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ValidarOTP.aspx.cs" Inherits="Monolito_4bm.ValidarOTP" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Validar OTP - Óptica 4BM</title>
    
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.min.css" />
    
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }

        body { 
            font-family: 'Inter', sans-serif; 
            min-height: 100vh; 
            display: flex; 
            align-items: center; 
            justify-content: center; 
            background-color: #fafafa; 
            padding: 20px; 
            color: #212529;
        }

        .otp-container { 
            width: 100%; 
            max-width: 420px; 
            background: #fff; 
            border-radius: 8px; 
            box-shadow: 0 10px 30px rgba(0,0,0,0.03); 
            border: 1px solid #eaeaea;
            padding: 40px 30px; 
            text-align: center; 
            position: relative; 
        }

        .shield-icon { 
            width: 60px; 
            height: 60px; 
            border-radius: 50%; 
            background: #f8f9fa; 
            border: 1px solid #eaeaea;
            display: flex; 
            align-items: center; 
            justify-content: center; 
            margin: 0 auto 20px; 
        }
        .shield-icon i { color: #1a1a1a; font-size: 24px; }

        h2 { font-size: 20px; font-weight: 600; color: #212529; margin-bottom: 8px; letter-spacing: -0.5px; }
        .sub { font-size: 14px; color: #777; margin-bottom: 24px; }

        .otp-input { 
            font-size: 28px; 
            text-align: center; 
            letter-spacing: 12px; 
            font-weight: 600; 
            border-radius: 4px; 
            border: 2px solid #eaeaea; 
            padding: 12px; 
            margin-bottom: 20px; 
            box-shadow: none;
            color: #212529;
            transition: all 0.2s;
        }
        .otp-input:focus { 
            border-color: #212529; 
            box-shadow: 0 0 0 3px rgba(33, 37, 41, 0.1); 
            outline: none;
        }

        .btn-verify { 
            width: 100%; 
            padding: 12px; 
            background-color: #212529; 
            border: 1px solid #212529; 
            border-radius: 4px; 
            color: #fff; 
            font-size: 14px; 
            font-weight: 500; 
            cursor: pointer; 
            margin-bottom: 12px; 
            transition: all 0.2s; 
        }
        .btn-verify:hover { background-color: #343a40; border-color: #343a40; }

        .btn-secondary-action { 
            width: 100%; 
            padding: 10px; 
            background: transparent; 
            border: 1px solid #ddd; 
            border-radius: 4px; 
            color: #555; 
            font-size: 14px; 
            font-weight: 500; 
            cursor: pointer; 
            margin-bottom: 12px; 
            transition: all 0.2s; 
        }
        .btn-secondary-action:hover { border-color: #212529; color: #212529; background-color: #fcfcfc; }

        .form-group { margin-bottom: 20px; text-align: left; }
        .form-group label { font-size: 13px; color: #555; margin-bottom: 6px; display: block; font-weight: 500;}
        .form-text { font-size: 12px; }

        #reader { width: 100%; margin-top: 10px; border-radius: 4px; overflow: hidden; display: none; border: 1px solid #eaeaea; }
        #reader video { border-radius: 4px; }

        .cancel-link {
            color: #555;
            font-size: 13px;
            text-decoration: none;
            transition: color 0.2s;
            display: inline-block;
            margin-top: 10px;
        }
        .cancel-link:hover { color: #000; text-decoration: underline; }
    </style>
</head>
<body>
    <form id="form1" runat="server" autocomplete="off">
        <asp:HiddenField ID="hfMsgType" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hfMsgText" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hfRedirectUrl" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hfScannedCode" runat="server" ClientIDMode="Static" />

        <div class="otp-container">
            <div class="shield-icon"><i class="fas fa-shield-halved"></i></div>
            <h2>Validación OTP</h2>
            <p class="sub">Ingresa el código enviado a tu correo, escanea el QR o sube una imagen</p>

            <asp:TextBox ID="txtOTP" runat="server" CssClass="form-control otp-input" MaxLength="6" placeholder="000000" ClientIDMode="Static" autocomplete="off" />

            <button type="button" class="btn-secondary-action" id="btnScanQR">
                <i class="fas fa-camera me-1"></i> Escanear QR con cámara
            </button>
            <div id="reader"></div>

            <div class="form-group">
                <label>O sube una imagen del código QR:</label>
                <asp:FileUpload ID="fileQR" runat="server" accept="image/*" CssClass="form-control form-control-sm" />
                <small class="form-text text-muted">Formatos: JPG, PNG, BMP, GIF.</small>
            </div>

            <asp:Button ID="btnVerificar" runat="server" Text="Verificar Código" CssClass="btn-verify" OnClick="btnVerificar_Click" />
            <asp:Button ID="btnReenviar" runat="server" Text="Reenviar Código" CssClass="btn-secondary-action" OnClick="btnReenviar_Click" />
            
            <a href="Login.aspx" class="cancel-link">Cancelar y volver al Login</a>
        </div>
    </form>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script src="https://unpkg.com/html5-qrcode"></script>
    <script>
        // SweetAlert alineado al diseño corporativo
        function mostrarMensaje() {
            var tipo = document.getElementById('hfMsgType').value;
            var texto = document.getElementById('hfMsgText').value;
            var url = document.getElementById('hfRedirectUrl').value;
            if (tipo && texto) {
                Swal.fire({
                    icon: tipo,
                    title: tipo === 'success' ? 'Éxito' : tipo === 'error' ? 'Error' : 'Atención',
                    text: texto,
                    confirmButtonColor: '#212529',
                    customClass: { popup: 'border-radius-8' }
                }).then(function () {
                    if (url) window.location.href = url;
                });
                document.getElementById('hfMsgType').value = '';
                document.getElementById('hfMsgText').value = '';
                document.getElementById('hfRedirectUrl').value = '';
            }
        }
        window.addEventListener('load', mostrarMensaje);

        // ---- Escáner QR en vivo ----
        let html5QrCode = null;
        let isScanning = false;

        document.getElementById('btnScanQR').addEventListener('click', function () {
            const reader = document.getElementById('reader');
            if (isScanning) {
                if (html5QrCode) html5QrCode.stop().then(() => {
                    reader.style.display = 'none';
                    this.innerHTML = '<i class="fas fa-camera me-1"></i> Escanear QR con cámara';
                    isScanning = false;
                }).catch(err => console.log(err));
                return;
            }

            reader.style.display = 'block';
            this.innerHTML = '<i class="fas fa-spinner fa-pulse me-1"></i> Abriendo cámara...';

            html5QrCode = new Html5Qrcode("reader");
            const config = { fps: 10, qrbox: { width: 250, height: 250 } };

            html5QrCode.start(
                { facingMode: "environment" },
                config,
                (decodedText, decodedResult) => {
                    document.getElementById('hfScannedCode').value = decodedText;
                    html5QrCode.stop().then(() => {
                        reader.style.display = 'none';
                        document.getElementById('btnScanQR').innerHTML = '<i class="fas fa-camera me-1"></i> Escanear QR con cámara';
                        isScanning = false;
                        document.getElementById('<%= btnVerificar.ClientID %>').click();
                    }).catch(err => console.log(err));
                },
                (errorMessage) => { }
            ).then(() => {
                document.getElementById('btnScanQR').innerHTML = '<i class="fas fa-stop me-1"></i> Detener escaneo';
                isScanning = true;
            }).catch(err => {
                alert("Error al acceder a la cámara: " + err);
                reader.style.display = 'none';
                document.getElementById('btnScanQR').innerHTML = '<i class="fas fa-camera me-1"></i> Escanear QR con cámara';
                isScanning = false;
            });
        });
    </script>
</body>
</html>