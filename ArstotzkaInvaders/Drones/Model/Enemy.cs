using System;
using System.Drawing;
using System.Collections.Generic;

namespace Drones
{
    public partial class Enemy
    {
        public float X, Y;             // Position de l'ennemi
        public int Width = 60, Height = 60;
        public Image Texture;

        private float yCible;          // Y aléatoire pour zigzag
        private Random rnd = new Random();

        private bool aTire = false;    // True si l'ennemi a déjà tiré

        public Enemy(int x, int y, Image tex)
        {
            X = x;
            Y = y;
            Texture = tex;

            // Y aléatoire initial pour zigzag
            yCible = rnd.Next(50, AirSpace.HEIGHT - 50);
        }

        // Affichage
        public void Render(BufferedGraphics bg)
        {
            if (Texture != null)
                bg.Graphics.DrawImage(Texture, (int)X, (int)Y, Width, Height);
        }

        public Rectangle GetBounds()
        {
            return new Rectangle((int)X, (int)Y, Width, Height);
        }

        public void Update(int interval, List<BazaAzova> bases)
        {
            // Déplacement horizontal vers la gauche
            float vitesseX = 0.05f * interval;
            X -= vitesseX;

            // Zigzag vertical
            float dy = yCible - Y;
            float vitesseY = 0.03f * interval;
            if (Math.Abs(dy) > 1)
                Y += Math.Sign(dy) * vitesseY;

            if (Math.Abs(dy) < 5)
            {
                yCible = rnd.Next(50, AirSpace.HEIGHT - 50);
            }
        }

        // Vérifie si l'ennemi peut tirer
        public bool PeutTirer()
        {
            return !aTire;
        }

        // Réinitialise l’état de tir (inutile si un seul tir)
        public void ReinitialiserTir()
        {
            aTire = true;
        }
    }
}
