using System;
using System.Text;

namespace Monolito_4bm
{
    public partial class Juego : System.Web.UI.Page
    {
        private JuegoNaves juego;
        const int ANCHO = 90;
        const int ALTO = 40;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Juego"] == null) Session["Juego"] = new JuegoNaves();
            juego = (JuegoNaves)Session["Juego"];
            if (!IsPostBack) RenderizarJuego();
        }

        protected void timerJuego_Tick(object sender, EventArgs e) { juego.Actualizar(); RenderizarJuego(); }
        protected void btnIzquierda_Click(object sender, EventArgs e) { juego.MoverIzquierda(); RenderizarJuego(); }
        protected void btnDerecha_Click(object sender, EventArgs e) { juego.MoverDerecha(ANCHO); RenderizarJuego(); }
        protected void btnDisparar_Click(object sender, EventArgs e) { juego.Disparar(); RenderizarJuego(); }
        protected void btnReiniciar_Click(object sender, EventArgs e) { juego.Reiniciar(); timerJuego.Enabled = true; RenderizarJuego(); }

        private void DibujarTexto(char[,] pantalla, int x, int y, string texto)
        {
            for (int i = 0; i < texto.Length; i++)
                if (x + i >= 0 && x + i < ANCHO && y >= 0 && y < ALTO)
                    pantalla[y, x + i] = texto[i];
        }

        private void RenderizarJuego()
        {
            char[,] p = new char[ALTO, ANCHO];
            for (int y = 0; y < ALTO; y++) for (int x = 0; x < ANCHO; x++) p[y, x] = ' ';

            // Bordes
            for (int x = 0; x < ANCHO; x++) { p[0, x] = '═'; p[ALTO - 1, x] = '═'; }
            for (int y = 0; y < ALTO; y++) { p[y, 0] = '║'; p[y, ANCHO - 1] = '║'; }
            p[0, 0] = '╔'; p[0, ANCHO - 1] = '╗'; p[ALTO - 1, 0] = '╚'; p[ALTO - 1, ANCHO - 1] = '╝';

            // Nave
            int nx = juego.NaveX, ny = juego.NaveY;
            string[] nave = { "     .      ", "   . ' .    ", "   | o |     ", " . ' o ' .   ", "  | .-. |    ", "   '   '     " };
            for (int i = 0; i < nave.Length; i++) DibujarTexto(p, nx, ny + i, nave[i]);

            // Escudo visual ( )
            if (juego.EscudoActivo)
            {
                for (int i = 0; i < 6; i++)
                {
                    DibujarTexto(p, nx - 3, ny + i, "(");
                    DibujarTexto(p, nx + 16, ny + i, ")");
                }
                DibujarTexto(p, nx - 2, ny - 1, "~~~~~~~~~~~~~~");
                DibujarTexto(p, nx - 2, ny + 6, "~~~~~~~~~~~~~~");
            }

            // Doble disparo visual ( ||  || )
            if (juego.DobleDisparo)
            {
                DibujarTexto(p, nx + 3, ny + 6, "||");
                DibujarTexto(p, nx + 10, ny + 6, "||");
            }

            // Disparos propios
            foreach (var d in juego.Disparos)
                if (d.Y > 0 && d.Y < ALTO) DibujarTexto(p, d.X, d.Y, "|");

            // Disparos enemigos
            foreach (var d in juego.DisparosEnemigos)
                if (d.Y > 0 && d.Y < ALTO) DibujarTexto(p, d.X, d.Y, "*");

            // Enemigos
            foreach (var e in juego.Enemigos)
            {
                if (e.EsDisparador)
                {
                    string[] enemigo = { "[******]", "[  *  *  ]", "\\/[##]\\/" };
                    for (int i = 0; i < enemigo.Length; i++) DibujarTexto(p, e.X, e.Y + i, enemigo[i]);
                }
                else
                {
                    string[] enemigo = { "[------]", "[  * *  ]", "\\/\\[##]/\\/" };
                    for (int i = 0; i < enemigo.Length; i++) DibujarTexto(p, e.X, e.Y + i, enemigo[i]);
                }
            }

            // Mini enemigos
            foreach (var m in juego.MiniEnemigos)
            {
                string[] mini = { "/[**]\\", "[ ** ]", "\\[--]/" };
                for (int i = 0; i < mini.Length; i++) DibujarTexto(p, m.X, m.Y + i, mini[i]);
            }

            // Boss con muerte gradual
            if (juego.Jefe != null)
            {
                string[] boss = { "      /\\[#]/\\\\      ", "    [    * *    ]   ", " \\/\\[#######]/\\\\/" };
                for (int i = 0; i < boss.Length; i++) DibujarTexto(p, juego.Jefe.X, juego.Jefe.Y + i, boss[i]);
            }

            // Cofre
            if (juego.Cofre != null)
            {
                string[] cofre = { " ___ ", "|+ +|", "|___|" };
                for (int i = 0; i < cofre.Length; i++) DibujarTexto(p, juego.Cofre.X, juego.Cofre.Y + i, cofre[i]);
            }

            // Explosiones
            foreach (var exp in juego.Explosiones)
            {
                string marco = new string('*', exp.Tiempo / 2);
                DibujarTexto(p, exp.X, exp.Y, marco);
            }

            // Mensaje flotante
            if (juego.TiempoMensaje > 0 && !string.IsNullOrEmpty(juego.MensajeFlotante))
                DibujarTexto(p, 20, 15, juego.MensajeFlotante);

            // Game Over / Victoria
            if (juego.GameOver) { DibujarTexto(p, 25, 20, "===== GAME OVER ====="); timerJuego.Enabled = false; }
            if (juego.Victoria) { DibujarTexto(p, 25, 20, "***** VICTORIA *****"); timerJuego.Enabled = false; }

            // Construir string final
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < ALTO; y++)
            {
                for (int x = 0; x < ANCHO; x++) sb.Append(p[y, x]);
                sb.AppendLine();
            }
            areaJuego.InnerText = sb.ToString();

            lblVidas.Text = "VIDAS: " + juego.Vidas;
            lblPuntos.Text = "PUNTOS: " + juego.Puntos;
            lblCalor.Text = "CALOR: " + juego.Calor + "%";
            lblEstado.Text = juego.EscudoActivo ? "ESCUDO" : (juego.DobleDisparo ? "DOBLE DISPARO" : "");
        }
    }
}