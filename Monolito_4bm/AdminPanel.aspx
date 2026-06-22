<%@ Page Title="Panel Administrador" Language="C#" MasterPageFile="~/Site.Admin.Master" AutoEventWireup="true" CodeBehind="AdminPanel.aspx.cs" Inherits="Monolito_4bm.AdminPanel" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hfMsg" runat="server" ClientIDMode="Static" />

    <div class="row mb-4">
        <div class="col-12">
            <div class="card shadow-sm">
                <div class="card-header bg-dark text-white d-flex justify-content-between align-items-center">
                    <h5 class="mb-0"><i class="fas fa-users-cog"></i> Usuarios Bloqueados</h5>
                    <asp:Button ID="btnRefrescar" runat="server" Text="&#128260; Refrescar" CssClass="btn btn-sm btn-outline-light"
                        OnClick="btnRefrescar_Click" CausesValidation="false" />
                </div>
                <div class="card-body p-0">
                    <asp:GridView ID="gvUsuarios" runat="server" CssClass="table table-hover table-striped mb-0"
                        AutoGenerateColumns="false" EmptyDataText="No hay usuarios bloqueados."
                        OnRowCommand="gvUsuarios_RowCommand" DataKeyNames="usu_id"
                        HeaderStyle-CssClass="table-dark">
                        <Columns>
                            <asp:BoundField DataField="usu_id" HeaderText="ID" />
                            <asp:BoundField DataField="usu_cedula" HeaderText="C&#233;dula" />
                            <asp:BoundField DataField="usu_nombres" HeaderText="Nombres" />
                            <asp:BoundField DataField="usu_apellidos" HeaderText="Apellidos" />
                            <asp:BoundField DataField="usu_nick" HeaderText="Nick" />
                            <asp:BoundField DataField="usu_correo" HeaderText="Correo" />
                            <asp:BoundField DataField="usu_intentos" HeaderText="Intentos" />
                            <asp:TemplateField HeaderText="Acci&#243;n">
                                <ItemTemplate>
                                    <asp:Button ID="btnDesbloquear" runat="server" Text="&#128275; Desbloquear"
                                        CommandName="Desbloquear" CommandArgument='<%# Eval("usu_id") %>'
                                        CssClass="btn btn-sm btn-success" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptContent" runat="server">
    <script>
        window.addEventListener('load', function() {
            var m = document.getElementById('hfMsg').value;
            if (m) {
                var isSuccess = m.toLowerCase().indexOf('exitoso') >= 0 || m.toLowerCase().indexOf('desbloqueado') >= 0;
                Swal.fire({ icon: isSuccess ? 'success' : 'error', title: isSuccess ? '\u00a1Listo!' : 'Error', text: m, confirmButtonColor: isSuccess ? '#28a745' : '#d33' });
                document.getElementById('hfMsg').value = '';
            }
        });
    </script>
</asp:Content>
