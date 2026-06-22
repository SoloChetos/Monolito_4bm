<%@ Page Title="Juego ASCII" Language="C#" MasterPageFile="~/PrincipalMaster.master"
    AutoEventWireup="true" CodeBehind="Juego.aspx.cs" Inherits="Monolito_4bm.Juego" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        body { background: black; }
        .contenedor { width: fit-content; margin: auto; text-align: center; color: white; font-family: Consolas, monospace; }
        .titulo { color: cyan; margin-top: 10px; }
        .panel { margin-bottom: 10px; font-size: 18px; }
        .ascii {
            background: black; color: white; font-family: Consolas, monospace; font-size: 14px;
            line-height: 14px; white-space: pre; border: 2px solid #777; padding: 10px;
            display: inline-block; text-align: left;
        }
        .botones { margin-top: 10px; }
        .botones input { margin: 4px; padding: 8px 15px; font-weight: bold; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <div class="contenedor">
        <h2 class="titulo">NAVE INTERCEPTOR ASCII EXTREMO</h2>
        <div class="panel">
            <asp:Label ID="lblVidas" runat="server" /> &nbsp;&nbsp;&nbsp;
            <asp:Label ID="lblPuntos" runat="server" /> &nbsp;&nbsp;&nbsp;
            <asp:Label ID="lblCalor" runat="server" /> &nbsp;&nbsp;&nbsp;
            <asp:Label ID="lblEstado" runat="server" />
        </div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:Timer ID="timerJuego" runat="server" Interval="120" OnTick="timerJuego_Tick" />
                <pre id="areaJuego" runat="server" class="ascii"></pre>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div class="botones">
            <asp:Button ID="btnIzquierda" runat="server" Text="← IZQUIERDA" OnClick="btnIzquierda_Click" />
            <asp:Button ID="btnDisparar" runat="server" Text="DISPARAR" OnClick="btnDisparar_Click" />
            <asp:Button ID="btnDerecha" runat="server" Text="DERECHA →" OnClick="btnDerecha_Click" />
            <asp:Button ID="btnReiniciar" runat="server" Text="REINICIAR" OnClick="btnReiniciar_Click" />
        </div>
    </div>
    <script>
        document.addEventListener('keydown', function (e) {
            if (e.key === "ArrowLeft") { __doPostBack('<%= btnIzquierda.UniqueID %>', ''); e.preventDefault(); }
            else if (e.key === "ArrowRight") { __doPostBack('<%= btnDerecha.UniqueID %>', ''); e.preventDefault(); }
            else if (e.key === " ") { __doPostBack('<%= btnDisparar.UniqueID %>', ''); e.preventDefault(); }
            else if (e.key === "r" || e.key === "R") { __doPostBack('<%= btnReiniciar.UniqueID %>', ''); e.preventDefault(); }
        });
    </script>
</asp:Content>