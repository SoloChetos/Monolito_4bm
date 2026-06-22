<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Monolito_4bm.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml" lang="es">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Iniciar Sesión - Óptica 4BM</title>
    
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
            background-color: #fafafa; /* Fondo claro y limpio */
            padding: 20px;
            color: #212529;
        }
        
        .login-wrapper { width: 100%; max-width: 400px; }
        
        .login-container {
            background: #fff; 
            border-radius: 8px;
            border: 1px solid #eaeaea;
            box-shadow: 0 10px 30px rgba(0,0,0,0.03); /* Sombra casi imperceptible */
            padding: 40px 30px; 
            position: relative; 
            overflow: hidden;
        }

        /* Barra de progreso elegante superior */
        .login-progress { 
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 4px;
            background: transparent;
            display: none;
        }
        .login-progress.active { display: block; }
        .login-progress .progress-bar {
            height: 100%;
            background-color: #212529;
            width: 0%;
            transition: width 1.8s cubic-bezier(0.4, 0, 0.2, 1);
        }

        /* Identidad Visual */
        .brand-icon {
            text-align: center;
            margin-bottom: 20px;
            color: #1a1a1a;
        }
        .brand-icon i { font-size: 32px; }
        
        h2 { font-size: 20px; font-weight: 600; text-align: center; margin-bottom: 6px; letter-spacing: -0.5px;}
        .subtitle { font-size: 14px; color: #777; text-align: center; margin-bottom: 30px; }
        
        /* Controles de Formulario */
        .form-floating { margin-bottom: 16px; }
        .form-floating .form-control { 
            border-radius: 4px; 
            border: 1px solid #ddd; 
            font-size: 14px; 
            box-shadow: none;
        }
        .form-floating .form-control:focus { 
            border-color: #212529; 
            box-shadow: 0 0 0 3px rgba(33, 37, 41, 0.1); 
        }
        .form-floating label { font-size: 14px; color: #888; }
        
        /* Enlaces y Checkbox */
        .remember-forgot { 
            display: flex; 
            justify-content: space-between; 
            align-items: center; 
            margin-bottom: 24px; 
            flex-wrap: wrap; 
            gap: 8px; 
        }
        .form-check-input:checked { background-color: #212529; border-color: #212529; }
        .form-check-input:focus { border-color: #212529; box-shadow: 0 0 0 3px rgba(33, 37, 41, 0.1); }
        .forgot-link { font-size: 13px; color: #555; text-decoration: none; transition: color 0.2s; }
        .forgot-link:hover { color: #000; text-decoration: underline; }
        
        /* Botones de Acción */
        .btn-login {
            width: 100%; 
            padding: 12px; 
            background-color: #212529;
            border: 1px solid #212529; 
            border-radius: 4px; 
            color: #fff; 
            font-size: 14px; 
            font-weight: 500;
            cursor: pointer; 
            transition: all 0.2s;
        }
        .btn-login:hover { background-color: #343a40; border-color: #343a40; }
        .btn-login:disabled { opacity: 0.7; cursor: not-allowed; }
        
        .btn-register {
            width: 100%; 
            padding: 12px; 
            margin-top: 12px; 
            background: transparent;
            border: 1px solid #ddd; 
            border-radius: 4px; 
            color: #555;
            font-size: 14px; 
            font-weight: 500; 
            cursor: pointer; 
            transition: all 0.2s;
        }
        .btn-register:hover { border-color: #212529; color: #212529; background-color: #fcfcfc;}

        /* Botones Sociales estilo Ghost */
        .social-section { text-align: center; margin-top: 30px; border-top: 1px solid #eaeaea; padding-top: 20px;}
        .social-section p { color: #888; font-size: 12px; margin-bottom: 16px; text-transform: uppercase; letter-spacing: 1px;}
        .social-buttons { display: flex; justify-content: center; gap: 10px; }
        .btn-social { 
            display: inline-flex; 
            align-items: center; 
            justify-content: center; 
            width: 40px;
            height: 40px;
            border: 1px solid #ddd; 
            border-radius: 50%; 
            color: #555; 
            cursor: pointer; 
            text-decoration: none; 
            transition: all 0.2s; 
        }
        .btn-social:hover { border-color: #212529; color: #212529; background-color: #f8f9fa; }

    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <asp:HiddenField ID="hfMsgType" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hfMsgText" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hfRedirectUrl" runat="server" ClientIDMode="Static" />

        <div class="login-wrapper">
            <div class="login-container">
                <div id="loginProgress" class="login-progress">
                    <div class="progress-bar" id="loginProgressBar"></div>
                </div>

                <div class="brand-icon"><i class="fas fa-glasses"></i></div>
                <h2>ÓPTICA 4BM</h2>
                <p class="subtitle">Acceso al sistema de gestión</p>

                <div class="form-floating">
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Usuario" ClientIDMode="Static" autocomplete="off" />
                    <label for="txtUsername">Usuario</label>
                </div>
                <div class="form-floating">
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Contraseña" ClientIDMode="Static" autocomplete="new-password" />
                    <label for="txtPassword">Contraseña</label>
                </div>

                <div class="remember-forgot">
                    <div class="form-check">
                        <asp:CheckBox ID="chkRemember" runat="server" CssClass="form-check-input" ClientIDMode="Static" />
                        <label class="form-check-label" for="chkRemember" style="font-size:13px;color:#555;">Recordarme</label>
                    </div>
                    <a href="OlvidasteContrasena.aspx" class="forgot-link">¿Olvidaste tu contraseña?</a>
                </div>

                <asp:Button ID="btnSignIn" runat="server" Text="Iniciar Sesión" CssClass="btn-login"
                    OnClientClick="return handleLogin();" OnClick="btnSignIn_Click" ClientIDMode="Static" />
                <asp:Button ID="btnShowRegister" runat="server" Text="Crear una cuenta" CssClass="btn-register"
                    OnClick="btnShowRegister_Click" CausesValidation="false" ClientIDMode="Static" />
            
                <div class="social-section">
                    <p>O ingresa con</p>
                    <div class="social-buttons">
                        <a href="#" class="btn-social" title="Facebook"><i class="fab fa-facebook-f"></i></a>
                        <a href="#" class="btn-social" title="Twitter"><i class="fab fa-twitter"></i></a>
                        <a href="#" class="btn-social" title="Google"><i class="fab fa-google"></i></a>
                    </div>
                </div>
            </div>
        </div>
    </form>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script>
        var loginReady = false;

        function handleLogin() {
            if (loginReady) return true;

            var u = document.getElementById('txtUsername');
            var p = document.getElementById('txtPassword');
            u.classList.remove('is-invalid'); p.classList.remove('is-invalid');
            var ok = true;

            if (!u.value.trim()) { u.classList.add('is-invalid'); ok = false; }
            if (!p.value.trim()) { p.classList.add('is-invalid'); ok = false; }

            if (!ok) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Datos requeridos',
                    text: 'Por favor, ingrese su usuario y contraseña.',
                    confirmButtonColor: '#212529',
                    customClass: { popup: 'border-radius-8' }
                });
                return false;
            }

            // Mostrar barra de progreso sutil
            document.getElementById('loginProgress').classList.add('active');
            document.getElementById('btnSignIn').disabled = true;

            setTimeout(function () {
                document.getElementById('loginProgressBar').style.width = '100%';
            }, 50);

            setTimeout(function () {
                loginReady = true;
                document.getElementById('btnSignIn').disabled = false;
                document.getElementById('btnSignIn').click();
            }, 1800);

            return false;
        }

        document.addEventListener('input', function (e) {
            if (e.target.classList.contains('form-control')) e.target.classList.remove('is-invalid');
        });

        window.addEventListener('load', function () {
            var t = document.getElementById('hfMsgType').value;
            var m = document.getElementById('hfMsgText').value;
            var r = document.getElementById('hfRedirectUrl').value;

            if (t && m) {
                Swal.fire({
                    icon: t,
                    title: t === 'success' ? 'Éxito' : 'Atención',
                    text: m,
                    confirmButtonColor: '#212529', /* Color alineado al diseño corporativo */
                    customClass: { popup: 'border-radius-8' }
                }).then(function () {
                    if (r) window.location.href = r;
                });
                document.getElementById('hfMsgType').value = '';
                document.getElementById('hfMsgText').value = '';
            }
        });
    </script>
</body>
</html>