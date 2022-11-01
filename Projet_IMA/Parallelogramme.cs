

namespace Projet_IMA
{
    class Parallelogramme : Object
    {
        public V3 cote1;
        public V3 cote2;

        public Parallelogramme(V3 origine, Material _material, V3 _cote1, V3 _cote2)
        {
            position = origine;
            material = _material;
            cote1 = _cote1;
            cote2 = _cote2;
        }

        public void Draw(LampeDirectionnelle _keyLight, LampeDirectionnelle _fillLight, V3 _CameraPos)
        {
            Couleur C = material.couleur;
            int k = 50; //specular power
            V3 normale = (cote1 ^ cote2);
            normale = normale / normale.Norm();

            float pas = 0.002f;
            for (float u = 0; u < 1; u += pas)  // echantillonage fnt paramétrique
                for (float v = 0; v < 1; v += pas)
                {
                    V3 P3D = position + u * cote1 + v * cote2;

                    // projection orthographique => repère écran

                    int x_ecran = (int)(P3D.x);
                    int y_ecran = (int)(P3D.y);
                    

                    //Texture mapping 
                    Couleur cRect = material.TextureMapping(u, v);
                    //Couleur cRect = material.LightingEffect(P3D, normale, cT1, _CameraPos, _keyLight, _fillLight, k);

                    BitmapEcran.DrawPixel(x_ecran, y_ecran, cRect);
                }
        }

        public override float calculSolution(V3 cameraPosition, V3 rayDir, out int indexInVoid)
        {
            V3 R0 = cameraPosition;
            V3 Rd = rayDir;
            V3 C = position;
            V3 normale = (cote1 ^ cote2);
            normale = normale / normale.Norm();
            //Console.WriteLine(radius);
            //listRadius[index] = radius;
            //indexInVoid = 0;


            float t = ((position - R0) * normale) / (Rd * normale);
            V3 R = R0 + t * Rd;
            /*Console.WriteLine("t1 = " + t1.ToString());
            Console.WriteLine("t2 = " + t2.ToString());*/
            recupUV(R, normale, out float u, out float v);

            if (u > 0 & u < 1 & v > 0 & v < 1)
            {
                
                indexInVoid = 0;
                return t;
                //Console.WriteLine("hit");
                //IMA.Invert_Coord_Spherique(R, C, radius, out float u, out float v);

            }
            else
            {
                indexInVoid = 1;
                return -1;
            }
        }

        public Couleur recupCouleur(float u, float v, V3 _CameraPos, LampeDirectionnelle _keyLight, LampeDirectionnelle _fillLight, float k)
        {
            V3 normale = (cote1 ^ cote2);
            normale = normale / normale.Norm();


            V3 P3D = position + u * cote1 + v * cote2;



            //Texture mapping 
            Couleur cT1 = material.TextureMapping(u, v);
            Couleur cRect = material.LightingEffect(P3D, normale, cT1, _CameraPos, _keyLight, _fillLight, k);

            return cRect;
        }

        //R : V3 de l'intersection 
        public void recupUV(V3 R, V3 normale, out float u, out float v) 
        {
            V3 vectoriel = cote1 ^ cote2;
            V3 AI = R - position;
            V3 uk = (cote2 ^ normale) / vectoriel.Norm();
            u = uk * AI;

            vectoriel = -vectoriel;
            V3 vk = (cote1 ^ normale) / vectoriel.Norm();

            v = vk * AI;
        }
    }
}