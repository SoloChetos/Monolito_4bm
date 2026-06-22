<%@ Page Title="Mi Perfil" Language="C#" MasterPageFile="~/PrincipalMaster.master" AutoEventWireup="true" CodeBehind="Perfil.aspx.cs" Inherits="Monolito_4bm.Perfil" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.min.css" />
    <style>
        .profile-wrapper { max-width: 700px; margin: 0 auto; }
        .profile-card { background: #fff; border-radius: 16px; box-shadow: 0 8px 24px rgba(0,0,0,0.1); padding: 35px 30px; }
        .profile-img-preview { width: 150px; height: 150px; object-fit: cover; border-radius: 50%; border: 4px solid #0f3460; margin-bottom: 20px; }
        .form-control:focus { border-color: #0f3460; box-shadow: 0 0 0 0.2rem rgba(15, 52, 96, 0.15); }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <asp:HiddenField ID="hfMsgType" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfMsgText" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfRedirectUrl" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfBase64Preview" runat="server" ClientIDMode="Static" />

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="profile-wrapper">
                <div class="profile-card text-center">
                    <h2 class="mb-4" style="color:#0f3460; font-weight:700;">Editar Perfil</h2>

                    <asp:Image ID="imgPreview" runat="server" CssClass="profile-img-preview" ClientIDMode="Static" AlternateText="Foto de perfil" />

                    <div class="mb-3">
                        <div class="btn btn-outline-secondary btn-sm" onclick="document.getElementById('fuFotoPerfil').click();">
                            <i class="fas fa-camera"></i> Seleccionar nueva foto (Max 5MB)
                        </div>
                        <asp:FileUpload ID="fuFotoPerfil" runat="server" ClientIDMode="Static" accept="image/png, image/jpeg, image/gif, image/webp" Style="display:none;" onchange="previewImage(this);" />
                    </div>

                    <div class="row g-3 text-start">
                        <div class="col-md-6">
                            <label class="form-label">Cédula</label>
                            <asp:TextBox ID="txtCedula" runat="server" CssClass="form-control" ReadOnly="true" />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Nick (no editable)</label>
                            <asp:TextBox ID="txtNick" runat="server" CssClass="form-control" ReadOnly="true" />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Nombres *</label>
                            <asp:TextBox ID="txtNombres" runat="server" CssClass="form-control" required="required" />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Apellidos *</label>
                            <asp:TextBox ID="txtApellidos" runat="server" CssClass="form-control" required="required" />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Dirección</label>
                            <asp:TextBox ID="txtDireccion" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Celular</label>
                            <asp:TextBox ID="txtCelular" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Correo electrónico</label>
                            <asp:TextBox ID="txtCorreo" runat="server" CssClass="form-control" TextMode="Email" />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Fecha de nacimiento</label>
                            <asp:TextBox ID="txtFechaCumple" runat="server" CssClass="form-control" TextMode="Date" />
                            <small class="text-muted" id="infoCumple" runat="server"></small>
                        </div>
                    </div>

                    <div class="mt-4">
                        <asp:Button ID="btnGuardar" runat="server" Text="Guardar Cambios" CssClass="btn btn-primary w-100" OnClick="btnGuardar_Click" OnClientClick="return validarPerfil();" />
                    </div>
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnGuardar" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script>
        // SweetAlert al cargar
        window.addEventListener('load', function () {
            var t = document.getElementById('hfMsgType').value;
            var m = document.getElementById('hfMsgText').value;
            var r = document.getElementById('hfRedirectUrl').value;
            if (t && m) {
                Swal.fire({ icon: t, title: t === 'success' ? '¡Éxito!' : 'Error', text: m, confirmButtonColor: '#0f3460' })
                    .then(function () { if (r) window.location.href = r; });
                document.getElementById('hfMsgType').value = '';
                document.getElementById('hfMsgText').value = '';
            }
        });

        // Previsualización de imagen
        function previewImage(input) {
            if (input.files && input.files[0]) {
                var file = input.files[0];
                if (file.size > 5 * 1024 * 1024) {
                    Swal.fire({ icon: 'error', title: 'Archivo muy grande', text: 'Máximo 5 MB.' });
                    input.value = '';
                    return;
                }
                var ext = file.name.split('.').pop().toLowerCase();
                if (['jpg', 'jpeg', 'png', 'gif', 'webp'].indexOf(ext) === -1) {
                    Swal.fire({ icon: 'error', title: 'Formato inválido', text: 'Solo JPG, PNG, GIF, WebP.' });
                    input.value = '';
                    return;
                }
                var reader = new FileReader();
                reader.onload = function (e) {
                    document.getElementById('imgPreview').src = e.target.result;
                };
                reader.readAsDataURL(file);
            }
        }

        // Validación antes de enviar
        function validarPerfil() {
            var nombres = document.getElementById('<%= txtNombres.ClientID %>').value.trim();
            var apellidos = document.getElementById('<%= txtApellidos.ClientID %>').value.trim();
            if (!nombres || !apellidos) {
                Swal.fire({ icon: 'warning', title: 'Campos obligatorios', text: 'Nombres y apellidos son requeridos.' });
                return false;
            }
            var fecha = document.getElementById('<%= txtFechaCumple.ClientID %>').value;
            if (fecha) {
                var hoy = new Date();
                var nac = new Date(fecha);
                var edad = hoy.getFullYear() - nac.getFullYear();
                var m = hoy.getMonth() - nac.getMonth();
                if (m < 0 || (m === 0 && hoy.getDate() < nac.getDate())) edad--;
                if (edad < 18) {
                    Swal.fire({ icon: 'warning', title: 'Edad insuficiente', text: 'Debes ser mayor de edad.' });
                    return false;
                }
            }
            return true;
        }
    </script>
</asp:Content>