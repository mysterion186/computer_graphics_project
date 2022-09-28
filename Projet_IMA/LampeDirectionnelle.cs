

namespace Projet_IMA
{
    class LampeDirectionnelle
    {
        public V3 direction;
        public Couleur couleur;

        public LampeDirectionnelle(V3 _direction, Couleur _couleur)
        {
            direction = _direction;
            direction.Normalize();
            couleur = _couleur;
        }
    }
}