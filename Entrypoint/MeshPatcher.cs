using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Entrypoint
{
    class MeshPatcher : MonoBehaviour
    {
        bool isRendered = false;

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoInlining)]
        void Update()
        {
            if (!isRendered)
            {
                isRendered = true;
                gameObject.GetComponent<MeshRenderer>().enabled = !gameObject.GetComponent<MeshRenderer>().enabled;
                gameObject.GetComponent<MeshRenderer>().enabled = !gameObject.GetComponent<MeshRenderer>().enabled;
            }
        }
    }
}
