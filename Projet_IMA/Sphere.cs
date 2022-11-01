using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Projet_IMA
{
    class Sphere : Object
    {
        public float radius;

        public Sphere(V3 _centre, float _radius, Material _material)
        {
            position = _centre;
            radius = _radius;
            material = _material;
        }

        public override float calculSolution(V3 cameraPosition, V3 rayDir, out int indexInVoid)
        {
            V3 R0 = cameraPosition;
            V3 Rd = rayDir;
            V3 C = position;

            //Console.WriteLine(radius);
            //listRadius[index] = radius;

            float A = Rd*Rd;
            //float A = Rd.Norm();
            float B = 2 * Rd * (R0 - C);
            float D = (R0 - C) * (R0 - C) - (float)Math.Pow(radius, 2);
            //float D = R0.Norm() - 2 * R0 * C + C.Norm() - (float)Math.Pow(radius, 2);

            float delta = (float)Math.Pow(B, 2) - (4 * A * D);

            float t1 = (-B - (float)Math.Sqrt(delta)) / (2 * A);
            float t2 = (-B + (float)Math.Sqrt(delta)) / (2 * A);
            /*Console.WriteLine("t1 = " + t1.ToString());
            Console.WriteLine("t2 = " + t2.ToString());*/
            if (t1 > 0)
            {
                V3 R = R0 + t1 * Rd;
                indexInVoid = 0;
                return t1;
                //Console.WriteLine("hit");
                //IMA.Invert_Coord_Spherique(R, C, radius, out float u, out float v);

            }
            else if (t2 > 0)
            {
                V3 R = R0 + t2 * Rd;
                indexInVoid = 0;
                return t2;
                //Console.WriteLine("hit");
                //IMA.Invert_Coord_Spherique(R, C, radius, out float u, out float v);
            }
            else
            {
                indexInVoid = 1;
                return -1;
            }
        }

        public void DrawSphere(LampeDirectionnelle _keyLight, LampeDirectionnelle _fillLight, V3 _CameraPos)
        {  
            Couleur CSphere = material.couleur;
            int k = 50; //specular power


            float pas = 0.01f;
            for (float u = IMA.PI; u < 2 * IMA.PI; u += pas)  // echantillonage fnt paramétrique
                for (float v = -IMA.PI / 2; v < IMA.PI / 2; v += pas)
                {

                    // calcul des coordoonées dans la scène 3D
                    float x3D = radius * IMA.Cosf(v) * IMA.Cosf(u) + position.x;
                    float y3D = radius * IMA.Cosf(v) * IMA.Sinf(u) + position.y;
                    float z3D = radius * IMA.Sinf(v) + position.z;

                    // projection orthographique => repère écran

                    int x_ecran = (int)(x3D);
                    int y_ecran = (int)(z3D);

                    V3 P = new V3(x3D, y3D, z3D);

                    V3 normale = new V3(IMA.Cosf(v) * IMA.Cosf(u),
                                        IMA.Cosf(v) * IMA.Sinf(u),
                                        IMA.Sinf(v));

                    //u v coordinate in texture
                    float uNormalise = u / (2 * IMA.PI);
                    float vNormalise = (v + IMA.PI / 2) / IMA.PI;

                    //Bump Mapping
                    normale = BumpMapping(u, v, uNormalise, vNormalise, normale);
                    
                   
                    //Texture mapping 
                    Couleur cT1 = material.TextureMapping(uNormalise, vNormalise);
                    

                    //Specular and diffus with the directional light (KeyLight + FillLight)
                    CSphere = material.LightingEffect(P, normale, cT1, _CameraPos, _keyLight, _fillLight, k);


                    BitmapEcran.DrawPixel(x_ecran, y_ecran, CSphere);
                }
        }

        public Couleur recupCouleur(float u, float v, V3 _CameraPos, LampeDirectionnelle _keyLight, LampeDirectionnelle _fillLight, float k)
        {
            // calcul des coordoonées dans la scène 3D
            float x3D = radius * IMA.Cosf(v) * IMA.Cosf(u) + position.x;
            float y3D = radius * IMA.Cosf(v) * IMA.Sinf(u) + position.y;
            float z3D = radius * IMA.Sinf(v) + position.z;

            // projection orthographique => repère écran

            /*int x_ecran = (int)(x3D);
            int y_ecran = (int)(z3D);*/

            V3 P = new V3(x3D, y3D, z3D);

            V3 normale = new V3(IMA.Cosf(v) * IMA.Cosf(u),
                                IMA.Cosf(v) * IMA.Sinf(u),
                                IMA.Sinf(v));

            //u v coordinate in texture
            float uNormalise = u / (2 * IMA.PI);
            float vNormalise = (v + IMA.PI / 2) / IMA.PI;

            //Bump Mapping
            normale = BumpMapping(u, v, uNormalise, vNormalise, normale);


            //Texture mapping 
            Couleur cT1 = material.TextureMapping(uNormalise, vNormalise);


            //Specular and diffus with the directional light (KeyLight + FillLight)
            return material.LightingEffect(P, normale, cT1, _CameraPos, _keyLight, _fillLight, 50);
        }

        V3 BumpMapping(float _u, float _v, float _uNormalise, float _vNormalise, V3 _normale)
        {
            if (material.hasBump)
            {
                material.bump.Bump(_uNormalise, _vNormalise, out float dhdu, out float dhdv);

                V3 dmdu = new V3(radius * -IMA.Cosf(_v) * IMA.Sinf(_u),
                                 radius * IMA.Cosf(_v) * IMA.Cosf(_u),
                                 0);

                V3 dmdv = new V3(radius * -IMA.Sinf(_v) * IMA.Cosf(_u),
                                 radius * -IMA.Sinf(_v) * IMA.Sinf(_u),
                                 radius * IMA.Cosf(_v));

                V3 N = dmdu ^ dmdv;
                N.Normalize();

                if (N * _normale < 0)
                {
                    N = -N;
                }

                float coef_k = 0.004f;

                V3 T2 = dhdu * (N ^ dmdv);
                V3 T3 = dhdv * (dmdu ^ N);
                V3 new_normale = N + coef_k * (T2 + T3);

                return new_normale;
            }
            else
            {
                return _normale;
            }
        }
    }
}
