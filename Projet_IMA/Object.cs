using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_IMA
{
    abstract class Object
    {
        public V3 position;
        public Material material;

        public virtual float calculSolution(V3 cameraPosition, V3 rayDir, out int indexInVoid)
        {
            indexInVoid = 1;
            return -1; 
        }
    }
}
