using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Projet_IMA
{
    class Sphere
    {
        V3 centre;
        float radius;
        Material material;

        public Sphere(V3 _centre, float _radius, Material _material)
        {
            centre = _centre;
            radius = _radius;
            material = _material;
        }


        public void DrawSphere(LampeDirectionnelle _keyLight, LampeDirectionnelle _fillLight, V3 _CameraPos)

        {  
            Couleur CSphere = material.couleur;
            int k = 50;


            float pas = 0.01f;

            for (float u = IMA.PI; u < 2 * IMA.PI; u += pas)  // echantillonage fnt paramétrique

                for (float v = -IMA.PI / 2; v < IMA.PI / 2; v += pas)
                {

                    // calcul des coordoonées dans la scène 3D
                    float x3D = radius * IMA.Cosf(v) * IMA.Cosf(u) + centre.x;
                    float y3D = radius * IMA.Cosf(v) * IMA.Sinf(u) + centre.y;
                    float z3D = radius * IMA.Sinf(v) + centre.z;

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
                    if (material.hasBump)
                    {
                        material.bump.Bump(uNormalise, vNormalise, out float dhdu, out float dhdv);

                        V3 dmdu = new V3(radius * -IMA.Cosf(v) * IMA.Sinf(u),
                                         radius * IMA.Cosf(v) * IMA.Cosf(u),
                                         0);

                        V3 dmdv = new V3(radius * -IMA.Sinf(v) * IMA.Cosf(u),
                                         radius * -IMA.Sinf(v) * IMA.Sinf(u),
                                         radius * IMA.Cosf(v));

                        V3 N = dmdu ^ dmdv;
                        N.Normalize();

                        if (N * normale < 0)
                        {
                            N = -N;
                        }


                        float coef_k = 0.004f;


                        V3 T2 = dhdu * (N ^ dmdv);
                        V3 T3 = dhdv * (dmdu ^ N);
                        normale = N + coef_k * (T2 + T3);
                    }
                    
                    //For specular and diffus
                    V3 D = P - _CameraPos;
                    D.Normalize();

                    V3 R = 2 * (_keyLight.direction * normale) * normale - _keyLight.direction;

                    float produitScalaireNL = normale * _keyLight.direction; //for the keyLight
                    float produitScalaireNL2 = normale * _fillLight.direction; // for the fillLight
                    float produiScalaireRD = R * D; //for the keyLight


                    //Texture mapping 
                    Couleur cT1;
                    if (material.hasTexture)
                    {
                        cT1 = material.texture.LireCouleur(uNormalise, vNormalise);
                    }
                    else
                    {
                        cT1 = material.couleur;
                    }
                    
                    //Specular and diffus with the directional light (KeyLight + FillLight)
                    Couleur CDiffus;
                    Couleur CDiffus2;
                    Couleur CSpecular;
                    Couleur CKeyLight;
                    Couleur CFillLight;
                    if (produitScalaireNL >= 0)
                    {
                        CDiffus = _keyLight.couleur * cT1 * produitScalaireNL;
                        if (produitScalaireNL2 >= 0)
                        {
                            CDiffus2 = _fillLight.couleur * cT1 * produitScalaireNL2;
                        }
                        else
                        {
                            CDiffus2 = cT1 * 0;
                        }

                        CSpecular = _keyLight.couleur * (float)Math.Pow(produiScalaireRD, k);
                        
                     
                        CKeyLight = CDiffus + CSpecular;
                        CFillLight = CDiffus2;
                        CSphere = CKeyLight + CFillLight;
                    }
                    else
                    {
                        if (produitScalaireNL2 >= 0)
                        {
                            CDiffus2 = _fillLight.couleur * cT1 * produitScalaireNL2;
                        }
                        else
                        {
                            CDiffus2 = cT1 * 0;
                        }

                        CKeyLight = cT1 * 0;
                        CFillLight = CDiffus2;

                        CSphere = CKeyLight + CFillLight;


                    }


                    BitmapEcran.DrawPixel(x_ecran, y_ecran, CSphere);
                }
        }

    }
}
