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
            System.Diagnostics.Debug.WriteLine("wsh");
            V3 CameraPosition = new V3(500, -3000, 500);

            V3[] temp = new V3[3];
            System.Diagnostics.Debug.WriteLine(temp);
            LampeDirectionnelle keyLight = new LampeDirectionnelle(new V3(1, -1, 1), new Couleur(0.8f, 0.8f, 0.8f));
            LampeDirectionnelle fillLight = new LampeDirectionnelle(new V3(-1, -1, -0.5f), new Couleur(0.2f, 0.2f, 0.2f));

            Material materialSphere1 = new Material(Couleur.Red, "bump38.jpg");
            Material materialSphere2 = new Material("gold.jpg", "gold_Bump.jpg");
            Material materialSphere3 = new Material("lead.jpg", "lead_bump.jpg");
            Material materialRect = new Material("brick01.jpg", "lead_bump.jpg");
            Material materialRect1 = new Material("rock.jpg", "lead_bump.jpg");

            Object[] listOfObjectInScene = new Object[6];

            

            Sphere sphere = new Sphere(new V3(250, 200, 300), 100, materialSphere1);
            listOfObjectInScene[0] = sphere;
            //sphere.DrawSphere(keyLight, fillLight, CameraPosition);

            Sphere sphere2 = new Sphere(new V3(500, 2000, 300), 100, materialSphere2);
            listOfObjectInScene[1] = sphere2;
            //sphere2.DrawSphere(keyLight, fillLight, CameraPosition);

            Sphere sphere3 = new Sphere(new V3(750, 200, 300), 100, materialSphere3);
            listOfObjectInScene[2] = sphere3;
            //sphere3.DrawSphere(keyLight, fillLight, CameraPosition);

            Parallelogramme rect = new Parallelogramme(new V3(-50, 200, 600), materialRect, new V3(100, 3000, 0), new V3(0, 0, 700));
            //rect.Draw(keyLight, fillLight, CameraPosition);
            listOfObjectInScene[3] = rect;

            Parallelogramme rect1 = new Parallelogramme(new V3(900, -500, 600), materialRect, new V3(-100, 3000, 0), new V3(0, 0, 600));
            //rect1.Draw(keyLight, fillLight, CameraPosition);
            listOfObjectInScene[4] = rect1;

            Parallelogramme rect2 = new Parallelogramme(new V3(100, 2500, 590), materialRect1, new V3(700, 0, 0), new V3(0, 0, 600));
            //rect2.Draw(keyLight, fillLight, CameraPosition);
            listOfObjectInScene[5] = rect2;

            //Raycasting
            for (int x_ecran = 0; x_ecran <= BitmapEcran.GetWidth(); x_ecran++)
            {
                for (int y_ecran = 0; y_ecran <= BitmapEcran.GetHeight() ; y_ecran++)
                {
                    V3 PosPixScene = new V3(x_ecran, 0, y_ecran);
                    V3 DirRayon = PosPixScene - CameraPosition;
                    DirRayon.Normalize();

                    Couleur C = RayCast(CameraPosition, DirRayon, listOfObjectInScene, keyLight, fillLight);
                    BitmapEcran.DrawPixel(x_ecran, y_ecran, C);
                }
            }
            
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

        // listOfT : liste des solutions pour le calcul d'intersectoin
        //listPos : liste des positions des objets de la scene
        //listRadius : liste des rayons des spheres de la scene
        //_indexInVoid : le nombre de fois qu'on a pas d'intersection avec un objet de la scene
        public static V3[] calculIntersections(Object[] listOfObjectInScene, V3 cameraPosition, V3 RayDir, 
            out float[] listOfT, out V3[] listPos, out float[] listRadius, out int _indexInVoid)
        {
            int index = 0;
            int size = listOfObjectInScene.Length;
            int indexInVoid;
            _indexInVoid = 0;

            V3[] listIntersection = new V3[size];
            listOfT = new float[size];
            listPos = new V3[size];

            //for sphere
            listRadius = new float[size];

            foreach (Object objet in listOfObjectInScene)
            {
                V3 C = objet.position;
                listPos[index] = C;
                if (objet is Sphere)
                {
                    Sphere tempShepre = (Sphere)objet;
                    listRadius[index] = tempShepre.radius;

                }
                float t = objet.calculSolution(cameraPosition, RayDir, out indexInVoid);
                listOfT[index] = t;
                V3 R = cameraPosition + t * RayDir;
                listIntersection[index] = R;
                _indexInVoid += indexInVoid;
                index++;
            }

            return listIntersection;
        }

        //listOfT : la liste des solutions calculer les intersections
        //listIntersection : la liste des intersections
        //sizeOfListDist : la taille du tableau des distances, qui est égale au nombre d'objet dans la scene
        public static float[] calculDistances(V3 cameraPosition, float[] listOfT, V3[] listIntersection, int sizeOfListDist)
        {
            //calcul des distances
            float[] listDist = new float[sizeOfListDist];

            for (int i = 0; i < sizeOfListDist; i++)
            {
                listDist[i] = 100000;
            }
            for (int i = 0; i < sizeOfListDist; i++)
            {
                if (listOfT[i] == -1)
                {
                    Console.WriteLine("No intersection");
                }
                else
                {
                    V3 dir = listIntersection[i] - cameraPosition;
                    float dist = dir.Norm();
                    listDist[i] = dist;
                }
                //Console.WriteLine("index = " + index.ToString());
            }

            return listDist;
        }

        //listDist : la liste des distances
        public static int minIndex(float[] listDist)
        {
            float min = listDist[0];
            int index = 0;
            int indexMin = 0;
            foreach (float value in listDist)
            {
                if (value < min)
                {
                    min = value;
                    indexMin = index;
                }

                index++;
            }

            return indexMin;
        }

        public static Couleur RayCast(V3 cameraPosition, V3 RayDir, Object[] listOfObjectInScene, 
            LampeDirectionnelle _keyLight, LampeDirectionnelle _fillLight)
        {
            V3[] listIntersection = new V3[listOfObjectInScene.Length];
            Couleur backgroundColor = new Couleur(0, 0, 0);

            //calcul des intersections
            listIntersection = calculIntersections(listOfObjectInScene, cameraPosition, RayDir,
                out float[] listOfT, out V3[] listPos, out float[] listRadius, out int _indexInVoid);



            if(_indexInVoid == listOfObjectInScene.Length)
            {
                return backgroundColor;
            }
            else
            {
                //calcul des distances
                float[] listDist = calculDistances(cameraPosition, listOfT, listIntersection, listOfObjectInScene.Length);

                //Indice de la distance la plus petite
                int indexMin = minIndex(listDist);
                
                if(listOfObjectInScene[indexMin] is Sphere)
                {
                    IMA.Invert_Coord_Spherique(listIntersection[indexMin], listPos[indexMin], listRadius[indexMin], out float u, out float v);
                    Sphere a = (Sphere)listOfObjectInScene[indexMin];

                    return a.recupCouleur(u, v, cameraPosition, _keyLight, _fillLight, 50);
                }
                else
                {
                    Parallelogramme a = (Parallelogramme)listOfObjectInScene[indexMin];

                    V3 normale = a.cote1 ^ a.cote2;
                    normale = normale / normale.Norm();

                    a.recupUV(listIntersection[indexMin], normale, out float u, out float v);

                    if(u>0 & u<1 & v>0 & v < 1)
                    {
                        return a.recupCouleur(u, v, cameraPosition, _keyLight, _fillLight, 50);
                    }
                    else
                    {
                        return backgroundColor;
                    }
                    
                }                
            }
            
        }
    }
}
