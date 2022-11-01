using System;

namespace Projet_IMA
{
    class Material
    {
        public Texture texture;
        public Texture bump;
        public Couleur couleur;
        public bool hasTexture;
        public bool hasBump;

        public Material(string _textureName)
        {
            texture = new Texture(_textureName);
            //couleur = new Couleur(0, 0, 0);
            hasTexture = true;
            hasBump = false;
        }

        public Material(string _textureName, string _bumpName)
        {
            texture = new Texture(_textureName);
            bump = new Texture(_bumpName);
            couleur = new Couleur(0, 0, 0);
            hasTexture = true;
            hasBump = true;
        }

        public Material(Couleur _couleur)
        {
            couleur = _couleur;
            hasTexture = false;
            hasBump = false;
        }

        public Material(Couleur _couleur, string _bumpName)
        {
            bump = new Texture(_bumpName);
            couleur = _couleur;
            hasTexture = false;
            hasBump = true;
        }

        public Couleur SpecularEffect(V3 _P, V3 _normale, V3 cameraPosition, LampeDirectionnelle lampe, float specularPower)
        {
            V3 D = _P - cameraPosition;
            D.Normalize();
            V3 R = 2 * (lampe.direction * _normale) * _normale - lampe.direction;
            float produiScalaireRD = R * D; //for the keyLight

            Couleur specular = lampe.couleur * (float)Math.Pow(produiScalaireRD, specularPower);

            return specular;
        }

        public Couleur LightingEffect(V3 _P, V3 _normale, Couleur textureColor, V3 cameraPosition,
            LampeDirectionnelle _keyLight, LampeDirectionnelle _fillLight, float specularPower) //Lighting Effect Diffus + Specular + Ambiant color and keyLight + FillLight
        {
            //For diffus
            float produitScalaireNL = _normale * _keyLight.direction; //for the keyLight
            float produitScalaireNL2 = _normale * _fillLight.direction; // for the fillLight

            Couleur CDiffus;
            Couleur CDiffus2;
            Couleur CSpecular;
            Couleur CKeyLight;
            Couleur CFillLight;
            if (produitScalaireNL >= 0)
            {
                CDiffus = _keyLight.couleur * textureColor * produitScalaireNL;
                if (produitScalaireNL2 >= 0)
                {
                    CDiffus2 = _fillLight.couleur * textureColor * produitScalaireNL2;
                }
                else
                {
                    CDiffus2 = textureColor * 0;
                }

                CSpecular = SpecularEffect(_P, _normale, cameraPosition, _keyLight, specularPower);


                CKeyLight = CDiffus + CSpecular;
                CFillLight = CDiffus2;
                Couleur objectColor = CKeyLight + CFillLight;

                return objectColor;
            }
            else
            {
                if (produitScalaireNL2 >= 0)
                {
                    CDiffus2 = _fillLight.couleur * textureColor * produitScalaireNL2;
                }
                else
                {
                    CDiffus2 = textureColor * 0;
                }

                CKeyLight = textureColor * 0;
                CFillLight = CDiffus2;

                Couleur objectColor = CKeyLight + CFillLight;

                return objectColor;
            }
        }

        public Couleur TextureMapping(float _uNormalise, float _vNormalise)
        {
            if (hasTexture)
            {
                Couleur textureColor = texture.LireCouleur(_uNormalise, _vNormalise);
                return textureColor;
            }
            else
            {
                Couleur color = couleur;
                return color;
            }
        }
    }
}