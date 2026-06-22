using System;
using System.Collections.Generic;

namespace Monolito_4bm
{
    public class JuegoNaves
    {
        public int NaveX = 35;
        public int NaveY = 31;
        public int Vidas = 3;
        public int Puntos = 0;
        public int Calor = 0;
        public bool GameOver = false;
        public bool Victoria = false;
        public bool EscudoActivo = false;
        public bool DobleDisparo = false;
        public int TiempoEscudo = 0;
        public int TiempoDoble = 0;
        public string MensajeFlotante = "";
        public int TiempoMensaje = 0;

        public List<Disparo> Disparos = new List<Disparo>();
        public List<Disparo> DisparosEnemigos = new List<Disparo>();
        public List<Enemigo> Enemigos = new List<Enemigo>();
        public List<Explosion> Explosiones = new List<Explosion>();
        public Boss Jefe = null;
        public Cofre Cofre = null;

        Random rnd = new Random();
        int contador = 0;
        int vidaBoss = 20;
        int faseBoss = 1;
        int enemigosEliminados = 0;

        // Mini-enemigos invocados por el boss
        public List<MiniEnemigo> MiniEnemigos = new List<MiniEnemigo>();

        public void MoverIzquierda()
        {
            if (NaveX > 2) NaveX -= 2;
        }

        public void MoverDerecha(int ancho)
        {
            if (NaveX < ancho - 20) NaveX += 2;
        }

        public void Disparar()
        {
            if (GameOver || Victoria) return;

            if (Calor >= 85)
            {
                MensajeFlotante = "¡Muévete, internauta!";
                TiempoMensaje = 30;
                return;
            }

            if (DobleDisparo)
            {
                Disparos.Add(new Disparo(NaveX + 3, NaveY - 1));
                Disparos.Add(new Disparo(NaveX + 10, NaveY - 1));
            }
            else
            {
                Disparos.Add(new Disparo(NaveX + 6, NaveY - 1));
            }

            Calor += 10;
            if (Calor > 100) Calor = 100;
        }

        public void Actualizar()
        {
            if (GameOver || Victoria) return;

            contador++;

            // Enfriamiento
            if (Calor > 0) Calor -= 2;

            // Mensaje flotante
            if (TiempoMensaje > 0) TiempoMensaje--;

            // Poderes temporales
            if (EscudoActivo)
            {
                TiempoEscudo--;
                if (TiempoEscudo <= 0) EscudoActivo = false;
            }
            if (DobleDisparo)
            {
                TiempoDoble--;
                if (TiempoDoble <= 0) DobleDisparo = false;
            }

            // Mover disparos propios
            for (int i = Disparos.Count - 1; i >= 0; i--)
            {
                Disparos[i].Y--;
                if (Disparos[i].Y <= 0) Disparos.RemoveAt(i);
            }

            // Mover disparos enemigos
            for (int i = DisparosEnemigos.Count - 1; i >= 0; i--)
            {
                DisparosEnemigos[i].Y++;
                if (DisparosEnemigos[i].Y >= 40) DisparosEnemigos.RemoveAt(i);
            }

            // Colisión disparos enemigos con nave
            for (int i = DisparosEnemigos.Count - 1; i >= 0; i--)
            {
                if (DisparosEnemigos[i].X >= NaveX && DisparosEnemigos[i].X <= NaveX + 14 &&
                    DisparosEnemigos[i].Y >= NaveY && DisparosEnemigos[i].Y <= NaveY + 6)
                {
                    if (!EscudoActivo) Vidas--;
                    DisparosEnemigos.RemoveAt(i);
                    if (Vidas <= 0) GameOver = true;
                }
            }

            // Generar enemigos (2 tipos según puntuación)
            int dificultad = Math.Max(8, 25 - (Puntos / 100));
            if (contador % dificultad == 0 && Jefe == null)
            {
                bool tipoDisparador = Puntos >= 50 && rnd.Next(2) == 0;
                if (tipoDisparador)
                {
                    Enemigos.Add(new Enemigo(rnd.Next(2, 70), 3, true));
                }
                else
                {
                    Enemigos.Add(new Enemigo(rnd.Next(2, 70), 2, false));
                }
            }

            // Mover enemigos y que disparen
            for (int i = Enemigos.Count - 1; i >= 0; i--)
            {
                if (Enemigos[i].EsDisparador)
                {
                    // Se mueve horizontalmente en la parte superior
                    Enemigos[i].X += Enemigos[i].VelocidadX;
                    if (Enemigos[i].X <= 2 || Enemigos[i].X >= 70) Enemigos[i].VelocidadX *= -1;
                    // Dispara cada 20 ticks
                    if (contador % 20 == 0)
                    {
                        DisparosEnemigos.Add(new Disparo(Enemigos[i].X + 3, Enemigos[i].Y + 3));
                    }
                }
                else
                {
                    // Baja hacia la nave
                    Enemigos[i].Y++;
                    if (Enemigos[i].Y >= NaveY - 2)
                    {
                        if (!EscudoActivo) Vidas--;
                        Explosiones.Add(new Explosion(Enemigos[i].X, Enemigos[i].Y, 8));
                        Enemigos.RemoveAt(i);
                        enemigosEliminados++;
                        if (Vidas <= 0) GameOver = true;
                    }
                }
            }

            // Disparos propios vs enemigos
            for (int d = Disparos.Count - 1; d >= 0; d--)
            {
                bool eliminar = false;
                for (int e = Enemigos.Count - 1; e >= 0; e--)
                {
                    if (Colision(Disparos[d].X, Disparos[d].Y, Enemigos[e].X, Enemigos[e].Y, 10, 3))
                    {
                        Puntos += 10;
                        Explosiones.Add(new Explosion(Enemigos[e].X, Enemigos[e].Y, 6));
                        Enemigos.RemoveAt(e);
                        enemigosEliminados++;
                        eliminar = true;
                        break;
                    }
                }
                if (eliminar) { Disparos.RemoveAt(d); continue; }

                // vs mini enemigos
                for (int m = MiniEnemigos.Count - 1; m >= 0; m--)
                {
                    if (Colision(Disparos[d].X, Disparos[d].Y, MiniEnemigos[m].X, MiniEnemigos[m].Y, 6, 3))
                    {
                        Puntos += 5;
                        Explosiones.Add(new Explosion(MiniEnemigos[m].X, MiniEnemigos[m].Y, 4));
                        MiniEnemigos.RemoveAt(m);
                        eliminar = true;
                        break;
                    }
                }
                if (eliminar) { Disparos.RemoveAt(d); continue; }
            }

            // Cofre estratégico
            if (Cofre == null && Jefe == null)
            {
                bool debeAparecer = false;
                if (enemigosEliminados >= 10 && rnd.Next(0, 200) == 1) debeAparecer = true;
                if (Vidas == 1 && rnd.Next(0, 150) == 1) debeAparecer = true;
                if (Puntos == 0 && contador == 20) debeAparecer = true;

                if (debeAparecer)
                {
                    Cofre = new Cofre(rnd.Next(5, 70), rnd.Next(8, 20));
                }
            }

            // Disparos vs cofre
            if (Cofre != null)
            {
                for (int d = Disparos.Count - 1; d >= 0; d--)
                {
                    if (Colision(Disparos[d].X, Disparos[d].Y, Cofre.X, Cofre.Y, 5, 3))
                    {
                        AplicarPoder();
                        Cofre = null;
                        Disparos.RemoveAt(d);
                        break;
                    }
                }
            }

            // Boss
            if (Puntos >= 150 && Jefe == null)
            {
                Jefe = new Boss(25, 2);
                faseBoss = 1;
                vidaBoss = 20;
                MensajeFlotante = "¡BOSS APARECE!";
                TiempoMensaje = 60;
            }

            if (Jefe != null)
            {
                // Fase 1: Baja
                if (faseBoss == 1)
                {
                    Jefe.Y++;
                    if (Jefe.Y >= 10) faseBoss = 2;
                }
                // Fase 2: Se mantiene y dispara
                else if (faseBoss == 2)
                {
                    Jefe.X += Jefe.VelocidadX;
                    if (Jefe.X <= 2 || Jefe.X >= 60) Jefe.VelocidadX *= -1;
                    if (contador % 15 == 0)
                    {
                        DisparosEnemigos.Add(new Disparo(Jefe.X + 8, Jefe.Y + 3));
                    }
                    if (vidaBoss <= 5) faseBoss = 3;
                }
                // Fase 3: Invoca mini enemigos
                else if (faseBoss == 3 && contador % 30 == 0 && MiniEnemigos.Count < 4)
                {
                    MiniEnemigos.Add(new MiniEnemigo(rnd.Next(10, 70), rnd.Next(5, 15)));
                }

                // Disparos propios vs boss
                for (int d = Disparos.Count - 1; d >= 0; d--)
                {
                    if (Colision(Disparos[d].X, Disparos[d].Y, Jefe.X, Jefe.Y, 20, 3))
                    {
                        vidaBoss--;
                        Disparos.RemoveAt(d);
                        if (vidaBoss <= 0)
                        {
                            Explosiones.Add(new Explosion(Jefe.X - 3, Jefe.Y - 1, 20)); // Explosión grande
                            Jefe = null;
                            Victoria = true;
                        }
                        break;
                    }
                }

                // Colisión boss con nave
                if (Jefe != null && Colision(NaveX, NaveY, Jefe.X, Jefe.Y, 16, 3))
                {
                    if (!EscudoActivo) Vidas--;
                    Explosiones.Add(new Explosion(NaveX, NaveY, 6));
                    if (Vidas <= 0) GameOver = true;
                }
            }

            // Mini enemigos disparan
            if (contador % 25 == 0)
            {
                foreach (var m in MiniEnemigos)
                {
                    DisparosEnemigos.Add(new Disparo(m.X + 3, m.Y + 2));
                }
            }

            // Actualizar explosiones
            for (int i = Explosiones.Count - 1; i >= 0; i--)
            {
                Explosiones[i].Tiempo--;
                if (Explosiones[i].Tiempo <= 0) Explosiones.RemoveAt(i);
            }
        }

        private void AplicarPoder()
        {
            int p = rnd.Next(1, 4);
            switch (p)
            {
                case 1:
                    EscudoActivo = true;
                    TiempoEscudo = 100;
                    MensajeFlotante = "¡ESCUDO ACTIVADO!";
                    TiempoMensaje = 40;
                    break;
                case 2:
                    DobleDisparo = true;
                    TiempoDoble = 120;
                    MensajeFlotante = "¡DOBLE DISPARO!";
                    TiempoMensaje = 40;
                    break;
                case 3:
                    Vidas++;
                    MensajeFlotante = "¡VIDA EXTRA!";
                    TiempoMensaje = 40;
                    break;
            }
        }

        private bool Colision(int x1, int y1, int x2, int y2, int ancho, int alto)
        {
            return x1 >= x2 && x1 <= x2 + ancho && y1 >= y2 && y1 <= y2 + alto;
        }

        public void Reiniciar()
        {
            NaveX = 35; NaveY = 31; Vidas = 3; Puntos = 0; Calor = 0;
            GameOver = false; Victoria = false;
            EscudoActivo = false; DobleDisparo = false;
            TiempoEscudo = 0; TiempoDoble = 0;
            MensajeFlotante = ""; TiempoMensaje = 0;
            Disparos.Clear(); DisparosEnemigos.Clear();
            Enemigos.Clear(); Explosiones.Clear();
            MiniEnemigos.Clear();
            Cofre = null; Jefe = null;
            vidaBoss = 20; faseBoss = 1; enemigosEliminados = 0;
        }
    }

    public class Disparo { public int X; public int Y; public Disparo(int x, int y) { X = x; Y = y; } }
    public class Enemigo
    {
        public int X; public int Y; public bool EsDisparador; public int VelocidadX = 1;
        public Enemigo(int x, int y, bool disparador) { X = x; Y = y; EsDisparador = disparador; if (disparador) VelocidadX = (new Random().Next(2) == 0 ? 1 : -1); }
    }
    public class Boss { public int X; public int Y; public int VelocidadX = 1; public Boss(int x, int y) { X = x; Y = y; } }
    public class Cofre { public int X; public int Y; public Cofre(int x, int y) { X = x; Y = y; } }
    public class Explosion { public int X; public int Y; public int Tiempo; public Explosion(int x, int y, int t) { X = x; Y = y; Tiempo = t; } }
    public class MiniEnemigo { public int X; public int Y; public MiniEnemigo(int x, int y) { X = x; Y = y; } }
}