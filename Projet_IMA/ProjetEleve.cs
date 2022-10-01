using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Projet_IMA
{
    static class ProjetEleve
    {
        public static void Go()
        {
            //////////////////////////////////////////////////////////////////////////
            ///
            ///     Sphère en 3D
            /// 
            //////////////////////////////////////////////////////////////////////////
            
            V3 CameraPosition = new V3(1000, -3000, 500);

            LampeDirectionnelle keyLight = new LampeDirectionnelle(new V3(1, -1, 1), new Couleur(0.8f, 0.8f, 0.8f));
            LampeDirectionnelle fillLight = new LampeDirectionnelle(new V3(-1, -1, -0.5f), new Couleur(0.2f, 0.2f, 0.2f));

            Material materialSphere1 = new Material(Couleur.Red);
            Material materialSphere2 = new Material("gold.jpg", "gold_Bump.jpg");
            Material materialSphere3 = new Material("lead.jpg", "lead_bump.jpg");

            Sphere sphere = new Sphere(new V3(250, 200, 400), 100, materialSphere1);
            sphere.DrawSphere(keyLight, fillLight, CameraPosition);

            Sphere sphere2 = new Sphere(new V3(500, 200, 400), 100, materialSphere2);
            sphere2.DrawSphere(keyLight, fillLight, CameraPosition);

            Sphere sphere3 = new Sphere(new V3(750, 200, 400), 100, materialSphere3);
            sphere3.DrawSphere(keyLight, fillLight, CameraPosition);
            //////////////////////////////////////////////////////////////////////////
            ///
            ///     Rectangle 3D  + exemple texture
            /// 
            //////////////////////////////////////////////////////////////////////////


            Couleur LampAmbiente = new Couleur(1, 1, 1);
            

            Couleur BlueRect = Couleur.Blue;
            Couleur RedRect = Couleur.Red;
            Couleur GreenRect = Couleur.Green;
            Couleur WhiteRect = new Couleur(1, 1, 1);
            Couleur YellowRect = new Couleur(1, 1, 0);
            Couleur MagentaRect = new Couleur(1, 0, 1);
            Couleur CyanRect = new Couleur(0, 1, 1);
            Couleur BlackRect = new Couleur(0, 0, 0);

            // Gestion des textures
            // Texture T1 = new Texture("brick01.jpg");
            // Couleur c = T1.LireCouleur(u, v);

            /*DrawRect(50, 200, 300, 100, 100, WhiteRect * LampAmbiente);
            DrawRect(160, 200, 300, 100, 100, RedRect * LampAmbiente);
            DrawRect(270, 200, 300, 100, 100, YellowRect * LampAmbiente);
            DrawRect(380, 200, 300, 100, 100, GreenRect * LampAmbiente);
            DrawRect(490, 200, 300, 100, 100, CyanRect * LampAmbiente);
            DrawRect(600, 200, 300, 100, 100, BlueRect * LampAmbiente);
            DrawRect(710, 200, 300, 100, 100, MagentaRect * LampAmbiente);
            DrawRect(820, 200, 300, 100, 100, BlackRect * LampAmbiente);*/

        }

        public static void DrawRect(float x, float y, float z, float _coté1, float _coté2, Couleur C)
        {
            V3 Origine = new V3(x, y, z);
            V3 Coté1 = new V3(_coté1, 000, 000);
            V3 Coté2 = new V3(000, _coté2, 000);

            float pas = 0.002f;
            for (float u = 0; u < 1; u += pas)  // echantillonage fnt paramétrique
                for (float v = 0; v < 1; v += pas)
                {
                    V3 P3D = Origine + u * Coté1 + v * Coté2;

                    // projection orthographique => repère écran

                    int x_ecran = (int)(P3D.x);
                    int y_ecran = (int)(P3D.y);
                    BitmapEcran.DrawPixel(x_ecran, y_ecran, C);
                }
        }
    }
}
