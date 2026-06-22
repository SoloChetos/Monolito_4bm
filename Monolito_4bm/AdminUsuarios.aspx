<%@ Page Title="Gestión de Usuarios" Language="C#" MasterPageFile="~/PrincipalMaster.master" AutoEventWireup="true" CodeBehind="AdminUsuarios.aspx.cs" Inherits="Monolito_4bm.AdminUsuarios" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.min.css" />
    <style>
        .table-wrapper { background: #fff; border-radius: 12px; box-shadow: 0 4px 16px rgba(0,0,0,0.08); padding: 25px; margin-top: 20px; }
        .table thead th { background-color: #0f3460; color: #fff; font-weight: 600; border-bottom: none; }
        .badge-active { background-color: #d4edda; color: #155724; padding: 6px 12px; border-radius: 20px; font-weight: 600; }
        .badge-inactive { background-color: #f8d7da; color: #721c24; padding: 6px 12px; border-radius: 20px; font-weight: 600; }
        .badge-blocked { background-color: #fff3cd; color: #856404; padding: 6px 12px; border-radius: 20px; font-weight: 600; }
        .btn-action { margin: 0 4px; border-radius: 6px; padding: 6px 12px; font-size: 13px; }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hfMsgType" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfMsgText" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hfRedirectUrl" runat="server" ClientIDMode="Static" />

    <h2 class="mb-2" style="color: #0f3460; font-weight: 700;"><i class="fas fa-users-cog me-2"></i> Gesti&#243;n de Usuarios</h2>
    <p class="text-muted">Administra los accesos y el estado de los usuarios del sistema.</p>

    <div class="table-wrapper table-responsive">
        <asp:GridView ID="gvUsuarios" runat="server" AutoGenerateColumns="False" CssClass="table table-hover align-middle" 
            GridLines="None" OnRowCommand="gvUsuarios_RowCommand">
            <Columns>
                <asp:BoundField DataField="usu_nick" HeaderText="Usuario" />
                <asp:TemplateField HeaderText="Nombre Completo">
                    <ItemTemplate>
                        <%# Eval("usu_nombres") %> <%# Eval("usu_apellidos") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="usu_correo" HeaderText="Correo" />
                <asp:BoundField DataField="usu_intentos_dia" HeaderText="Intentos Fallidos" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                <asp:TemplateField HeaderText="Estado">
                    <ItemTemplate>
                        <asp:Label ID="lblEstado" runat="server" 
                            CssClass='<%# Eval("usu_estado").ToString() == "A" ? "badge-active" : (Eval("usu_estado").ToString() == "B" ? "badge-blocked" : "badge-inactive") %>'
                            Text='<%# Eval("usu_estado").ToString() == "A" ? "Activo" : (Eval("usu_estado").ToString() == "B" ? "Bloqueado" : "Inactivo") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Acciones" ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center">
                    <ItemTemplate>
                        <!-- Botón para alternar estado Activo/Inactivo -->
                        <asp:LinkButton ID="btnToggleEstado" runat="server" CommandName="ToggleEstado" CommandArgument='<%# Eval("usu_id") + "|" + Eval("usu_estado") %>' 
                            CssClass='<%# Eval("usu_estado").ToString() == "A" ? "btn btn-outline-danger btn-action" : "btn btn-outline-success btn-action" %>'
                            ToolTip='<%# Eval("usu_estado").ToString() == "A" ? "Desactivar usuario" : "Activar usuario" %>'>
                            <i class='<%# Eval("usu_estado").ToString() == "A" ? "fas fa-ban" : "fas fa-check" %>'></i>
                        </asp:LinkButton>

                        <!-- Botón para desbloquear (resetear intentos) -->
                        <asp:LinkButton ID="btnDesbloquear" runat="server" CommandName="Desbloquear" CommandArgument='<%# Eval("usu_id") %>' 
                            CssClass="btn btn-outline-warning btn-action" ToolTip="Resetear intentos / Desbloquear"
                            Visible='<%# Eval("usu_estado").ToString() == "B" || Convert.ToInt32(Eval("usu_intentos_dia")) > 0 %>'>
                            <i class="fas fa-unlock-alt"></i>
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <div class="text-center py-4">No hay usuarios registrados en el sistema.</div>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script>
        window.addEventListener('load', function() {
            var t = document.getElementById('hfMsgType').value;
            var m = document.getElementById('hfMsgText').value;
            var r = document.getElementById('hfRedirectUrl').value;
            if (t && m) {
                Swal.fire({ icon: t, title: t === 'success' ? '\u00a1Atenci\u00f3n!' : 'Error', text: m, confirmButtonColor: t === 'success' ? '#0f3460' : '#d33' })
                .then(function() { if (r) window.location.href = r; });
                document.getElementById('hfMsgType').value = ''; 
                document.getElementById('hfMsgText').value = '';
            }
        });
    </script>
</asp:Content>
