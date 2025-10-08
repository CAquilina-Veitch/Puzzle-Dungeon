using System;
using Unity.Mathematics;
using UnityEngine;

namespace Runtime.Extensions
{
    public static class DoubleMathExtensions
    {
        public static double Distance(double2 a, double2 b)
        {
            double dx = a.x - b.x;
            double dy = a.y - b.y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
        
        public static int LayerFromLayerMask(LayerMask layerMask) {
            var layerNumber = 0;
            var layer = layerMask.value;

            // If no layer is set, return layer 0 (Default)
            if (layer == 0) return 0;

            while(layer > 0) {
                layer >>= 1;
                layerNumber++;
            }
            return layerNumber - 1;
        }
    }
}