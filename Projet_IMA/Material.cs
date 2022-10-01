

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
    }
}