<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="registrar.aspx.cs" Inherits="Monolito_4bm.registrar" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml" lang="es">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Registro de Usuario</title>
    <link href="https://fonts.googleapis.com/css2?family=Open+Sans:wght@400;600;700&amp;display=swap" rel="stylesheet" />
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
        .register-wrapper { width: 100%; max-width: 580px; }
        .register-container {
            background: #fff; border-radius: 12px;
            box-shadow: 0 8px 32px rgba(0,0,0,0.3);
            padding: 30px; position: relative; overflow: hidden;
        }
        .reg-progress { display: none; margin-bottom: 18px; }
        .reg-progress.active { display: block; }
        .reg-progress .progress { height: 28px; border-radius: 8px; background: #e9ecef; overflow: hidden; }
        .reg-progress .progress-bar {
            font-size: 12px; font-weight: 700; letter-spacing: 1px;
            background: repeating-linear-gradient(-45deg, #e8a87c, #e8a87c 10px, #d4956a 10px, #d4956a 20px);
            background-size: 28px 28px; width: 0%;
            animation: moveStripes 0.6s linear infinite;
            transition: width 1.8s ease-out;
            display: flex; align-items: center; justify-content: center;
        }
        @keyframes moveStripes { 0%{background-position:0 0;} 100%{background-position:28px 0;} }
        .avatar-wrapper { text-align: center; margin-bottom: 14px; }
        .avatar-circle {
            width: 90px; height: 90px; border-radius: 50%; background: #f0f0f0;
            margin: 0 auto; overflow: hidden; border: 3px solid #e8a87c;
            display: flex; align-items: center; justify-content: center;
        }
        .avatar-circle i { font-size: 36px; color: #ccc; }
        h2 { font-size: 22px; font-weight: 700; color: #333; text-align: center; margin-bottom: 6px; }
        .subtitle { font-size: 13px; color: #888; text-align: center; margin-bottom: 22px; }
        .form-floating { margin-bottom: 14px; }
        .form-floating .form-control { border-radius: 8px; border: 1px solid #ddd; font-size: 14px; }
        .form-floating .form-control:focus { border-color: #e8a87c; box-shadow: 0 0 0 3px rgba(232,168,124,0.15); }
        .form-floating label { font-size: 14px; color: #999; }
        .foto-section {
            margin-bottom: 16px; padding: 14px; border: 1px dashed #ddd; border-radius: 8px; background: #fafafa;
        }
        .foto-label { font-size: 13px; color: #666; margin-bottom: 8px; display: block; font-weight: 600; }
        .foto-info { font-size: 11px; color: #999; margin-top: 4px; }
        .preview-box {
            text-align: center; margin-top: 12px; padding: 10px;
            border: 1px solid #e0e0e0; border-radius: 8px; background: #fff;
        }
        .btn-registrar {
            width: 100%; padding: 12px; border: none; border-radius: 8px;
            background: linear-gradient(135deg, #e8a87c, #d4956a); color: #fff;
            font-size: 15px; font-weight: 600; cursor: pointer; transition: all 0.3s; margin-top: 6px;
        }
        .btn-registrar:hover { background: linear-gradient(135deg, #d4956a, #c4855a); transform: translateY(-1px); }
        .btn-registrar:disabled { opacity: 0.7; cursor: not-allowed; }
        .btn-preview {
            background: #6c757d; color: #fff; border: none; border-radius: 6px;
            padding: 6px 16px; font-size: 13px; cursor: pointer; margin-top: 8px;
        }
        .btn-preview:hover { background: #5a6268; }
        .btn-quitar { background: #dc3545; margin-left: 6px; }
        .btn-quitar:hover { background: #c82333; }
        .btn-volver {
            background: transparent; border: 2px solid #e0e0e0; color: #666;
            margin-top: 10px; display: block; text-align: center; text-decoration: none;
            padding: 12px; border-radius: 8px; font-size: 14px; font-weight: 600; transition: all 0.3s;
        }
        .btn-volver:hover { border-color: #e8a87c; color: #e8a87c; }
        .optional-badge { font-size: 10px; color: #aaa; font-weight: 400; }
        .lbl-error { color: #dc3545; font-size: 12px; display: block; margin-top: 2px; }
        @media (max-width: 576px) { .register-container { padding: 24px 18px; } }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <asp:HiddenField ID="hfMsgType" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hfMsgText" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hfRedirectUrl" runat="server" ClientIDMode="Static" />

        <div class="register-wrapper">
            <div class="register-container">
                <div id="regProgress" class="reg-progress">
                    <div class="progress">
                        <div class="progress-bar" id="regProgressBar" role="progressbar">REGISTRANDO...</div>
                    </div>
                </div>

                <!-- Avatar (server-side) -->
                <div class="avatar-wrapper">
                    <div class="avatar-circle">
                        <asp:Image ID="imgAvatar" runat="server" Width="90" Height="90"
                            style="object-fit:cover; display:none;" />
                        <asp:Label ID="lblAvatarIcon" runat="server">
                            <i class="fas fa-user" style="font-size:36px; color:#ccc;"></i>
                        </asp:Label>
                    </div>
                </div>

                <h2>Registro de Usuario</h2>
                <p class="subtitle">Complete los campos para registrarse</p>

                <div class="row">
                    <div class="col-sm-6">
                        <div class="form-floating">
                            <asp:TextBox ID="txtRegCedula" runat="server" CssClass="form-control" placeholder="C&#233;dula" ClientIDMode="Static" MaxLength="10" />
                            <label>C&#233;dula</label>
                        </div>
                    </div>
                    <div class="col-sm-6">
                        <div class="form-floating">
                            <asp:TextBox ID="txtRegCelular" runat="server" CssClass="form-control" placeholder="Celular" ClientIDMode="Static" MaxLength="10" />
                            <label>Celular</label>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-6">
                        <div class="form-floating">
                            <asp:TextBox ID="txtRegNombres" runat="server" CssClass="form-control" placeholder="Nombres" ClientIDMode="Static" />
                            <label>Nombres</label>
                        </div>
                    </div>
                    <div class="col-sm-6">
                        <div class="form-floating">
                            <asp:TextBox ID="txtRegApellidos" runat="server" CssClass="form-control" placeholder="Apellidos" ClientIDMode="Static" />
                            <label>Apellidos</label>
                        </div>
                    </div>
                </div>
                <div class="form-floating">
                    <asp:TextBox ID="txtRegDireccion" runat="server" CssClass="form-control" placeholder="Direcci&#243;n" ClientIDMode="Static" />
                    <label>Direcci&#243;n</label>
                </div>
                <div class="row">
                    <div class="col-sm-6">
                        <div class="form-floating">
                            <asp:TextBox ID="txtRegCorreo" runat="server" CssClass="form-control" placeholder="Correo" ClientIDMode="Static" TextMode="Email" />
                            <label>Correo electr&#243;nico</label>
                        </div>
                    </div>
                    <div class="col-sm-6">
                        <div class="form-floating">
                            <asp:TextBox ID="txtRegFechaCumple" runat="server" CssClass="form-control" TextMode="Date" placeholder="Fecha" ClientIDMode="Static" />
                            <label>Fecha de nacimiento</label>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-6">
                        <div class="form-floating">
                            <asp:TextBox ID="txtRegNick" runat="server" CssClass="form-control" placeholder="Nick" ClientIDMode="Static" />
                            <label>Nick <span class="optional-badge">(opcional)</span></label>
                        </div>
                    </div>
                    <div class="col-sm-6">
                        <div class="form-floating">
                            <asp:TextBox ID="txtRegPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Contrase&#241;a" ClientIDMode="Static" />
                            <label>Contrase&#241;a</label>
                        </div>
                    </div>
                </div>

                <!-- Foto de perfil - Preview desde SERVIDOR, sin JavaScript -->
                <div class="foto-section">
                    <span class="foto-label"><i class="fas fa-camera"></i> Foto de perfil <span class="optional-badge">(opcional)</span></span>
                    <asp:FileUpload ID="fileFoto" runat="server" CssClass="form-control" ClientIDMode="Static" />
                    <span class="foto-info">Formatos: JPG, PNG, GIF. M&#225;ximo 5 MB.</span>

                    <asp:Button ID="btnPrevisualizar" runat="server" Text="&#128247; Previsualizar" CssClass="btn-preview"
                        OnClick="btnPrevisualizar_Click" CausesValidation="false" />
                    <asp:Button ID="btnQuitarFoto" runat="server" Text="&#10060; Quitar" CssClass="btn-preview btn-quitar"
                        OnClick="btnQuitarFoto_Click" CausesValidation="false" Visible="false" />

                    <asp:Label ID="lblFotoError" runat="server" CssClass="lbl-error" Visible="false" />

                    <asp:Panel ID="pnlPreview" runat="server" Visible="false" CssClass="preview-box">
                        <asp:Image ID="imgPreview" runat="server" style="max-width:100%; max-height:180px; border-radius:6px;" />
                        <br />
                        <small style="color:#28a745;"><i class="fas fa-check-circle"></i> Imagen cargada correctamente</small>
                    </asp:Panel>
                </div>

                <asp:Button ID="btnRegistrar" runat="server" Text="Registrar" CssClass="btn-registrar"
                    OnClientClick="return handleRegister();" OnClick="btnRegistrar_Click" ClientIDMode="Static" />
                <a href="Login.aspx" class="btn-volver"><i class="fas fa-arrow-left"></i> Volver al Login</a>
            </div>
        </div>
    </form>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script>
        var regReady = false;
        function handleRegister() {
            if (regReady) return true;
            // Validar campos obligatorios
            var campos = ['txtRegCedula','txtRegCelular','txtRegNombres','txtRegApellidos','txtRegDireccion','txtRegCorreo','txtRegFechaCumple','txtRegPassword'];
            var ok = true;
            campos.forEach(function(id) {
                var el = document.getElementById(id);
                if (el) { el.classList.remove('is-invalid'); if (!el.value.trim()) { el.classList.add('is-invalid'); ok = false; } }
            });
            if (!ok) {
                Swal.fire({ icon: 'warning', title: 'Campos incompletos', text: 'Complete todos los campos obligatorios.', confirmButtonColor: '#e8a87c' });
                return false;
            }
            document.getElementById('regProgress').classList.add('active');
            document.getElementById('btnRegistrar').disabled = true;
            setTimeout(function() { document.getElementById('regProgressBar').style.width = '100%'; }, 50);
            setTimeout(function() { regReady = true; document.getElementById('btnRegistrar').disabled = false; document.getElementById('btnRegistrar').click(); }, 2000);
            return false;
        }
        document.addEventListener('input', function(e) { if (e.target.classList.contains('form-control')) e.target.classList.remove('is-invalid'); });
        window.addEventListener('load', function() {
            var t = document.getElementById('hfMsgType').value;
            var m = document.getElementById('hfMsgText').value;
            var r = document.getElementById('hfRedirectUrl').value;
            if (t && m) {
                Swal.fire({ icon: t, title: t === 'success' ? '\u00a1Registro Exitoso!' : 'Error', text: m, confirmButtonColor: t === 'success' ? '#28a745' : '#d33'
                }).then(function() { if (r) window.location.href = r; });
                document.getElementById('hfMsgType').value = ''; document.getElementById('hfMsgText').value = '';
            }
        });
    </script>
</body>
</html>
