using System;
using Unity.Mathematics;
using UnityEngine;

namespace Runtime.Extensions
{
    public static class UTMConverterExtensions
    {

        // GRS80 constants
        private const double a = 6378137.0;
        private const double e = 0.08181919084262149;
        private const double k0 = 0.9996;
        
        // Helper: Convert degrees to radians
        private static double ToRadians(double degrees) => degrees * (Math.PI / 180.0);

        // Helper: Convert radians to degrees
        private static double ToDegrees(double radians) => radians * (180.0 / Math.PI);

        /// <summary>
        /// Converts Latitude/Longitude (in decimal degrees) to UTM (easting, northing) coordinates.
        /// The zone parameter should be provided (typically calculated as: zone = floor((lon + 180) / 6) + 1).
        /// For locations in the southern hemisphere, northing is adjusted accordingly.
        /// </summary>
        public static void LatLongToUTM(double2 latLong, out int zone, out double2 eastingNorthing)
        {
            double latRad = ToRadians(latLong.x);
            double lonRad = ToRadians(latLong.y);

            zone = GetZone(latLong.y);
            // Calculate the central meridian for the zone (in radians)
            double lonOrigin = ToRadians(zone * 6 - 183);

            // Pre-calculate the square of the second eccentricity
            double ePrimeSq = Math.Pow(e, 2) / (1 - Math.Pow(e, 2));
            double N = a / Math.Sqrt(1 - Math.Pow(e * Math.Sin(latRad), 2));
            double T = Math.Pow(Math.Tan(latRad), 2);
            double C = ePrimeSq * Math.Pow(Math.Cos(latRad), 2);
            double A = Math.Cos(latRad) * (lonRad - lonOrigin);

            // Compute the meridional arc
            double M = a * ((1 - Math.Pow(e, 2) / 4 - 3 * Math.Pow(e, 4) / 64 - 5 * Math.Pow(e, 6) / 256) * latRad
                            - (3 * Math.Pow(e, 2) / 8 + 3 * Math.Pow(e, 4) / 32 + 45 * Math.Pow(e, 6) / 1024) * Math.Sin(2 * latRad)
                            + (15 * Math.Pow(e, 4) / 256 + 45 * Math.Pow(e, 6) / 1024) * Math.Sin(4 * latRad)
                            - (35 * Math.Pow(e, 6) / 3072) * Math.Sin(6 * latRad));

            // Calculate easting using the series expansion
            eastingNorthing.x = k0 * N * (A + (1 - T + C) * Math.Pow(A, 3) / 6
                                          + (5 - 18 * T + T * T + 72 * C - 58 * ePrimeSq) * Math.Pow(A, 5) / 120)
                              + 500000.0;

            // Calculate northing
            eastingNorthing.y = k0 * (M + N * Math.Tan(latRad) * (Math.Pow(A, 2) / 2
                                                                + (5 - T + 9 * C + 4 * C * C) * Math.Pow(A, 4) / 24
                                                                + (61 - 58 * T + T * T + 600 * C - 330 * ePrimeSq) * Math.Pow(A, 6) / 720));

            // Adjust for southern hemisphere
            if (latLong.x < 0)
            {
                eastingNorthing.y += 10000000.0;
            }
        }

        /// <summary>
        /// Converts UTM (easting, northing) coordinates to Latitude/Longitude (in decimal degrees).
        /// This implementation assumes a southern hemisphere adjustment.
        /// </summary>
        public static void UTMToLatLong(double2 eastingNorthing, int zone, out double2 latLong)
        {
            var easting = eastingNorthing.x;
            var northing = eastingNorthing.y;
            // Remove the UTM offsets.
            easting -= 500000.0;
            // This subtraction assumes the coordinate is in the southern hemisphere.
            northing -= 10000000.0;

            double m = northing / k0;
            double mu = m / (a * (1 - Math.Pow(e, 2) / 4 - 3 * Math.Pow(e, 4) / 64 - 5 * Math.Pow(e, 6) / 256));

            double e1 = (1 - Math.Sqrt(1 - Math.Pow(e, 2))) / (1 + Math.Sqrt(1 - Math.Pow(e, 2)));

            double j1 = (3 * e1 / 2) - (27 * Math.Pow(e1, 3) / 32);
            double j2 = (21 * Math.Pow(e1, 2) / 16) - (55 * Math.Pow(e1, 4) / 32);
            double j3 = (151 * Math.Pow(e1, 3) / 96);
            double j4 = (1097 * Math.Pow(e1, 4) / 512);

            double lat1 = mu + j1 * Math.Sin(2 * mu) + j2 * Math.Sin(4 * mu) + j3 * Math.Sin(6 * mu) + j4 * Math.Sin(8 * mu);

            double c1 = Math.Pow(e, 2) * Math.Pow(Math.Cos(lat1), 2) / (1 - Math.Pow(e, 2));
            double t1 = Math.Pow(Math.Tan(lat1), 2);
            double n1 = a / Math.Sqrt(1 - Math.Pow(e * Math.Sin(lat1), 2));
            double r1 = n1 * (1 - Math.Pow(e, 2)) / (1 - Math.Pow(e * Math.Sin(lat1), 2));

            double d = easting / (n1 * k0);

            double lat = lat1 - (n1 * Math.Tan(lat1) / r1) *
                (Math.Pow(d, 2) / 2 - Math.Pow(d, 4) / 24 * (5 + 3 * t1 + 10 * c1 - 4 * Math.Pow(c1, 2) - 9 * Math.Pow(e1, 2))
                                    - Math.Pow(d, 6) / 720 * (61 + 90 * t1 + 298 * c1 + 45 * Math.Pow(t1, 2) - 252 * Math.Pow(e1, 2) - 3 * Math.Pow(c1, 2)));
            lat = ToDegrees(lat);

            double lon = (d - Math.Pow(d, 3) / 6 * (1 + 2 * t1 + c1)
                          + Math.Pow(d, 5) / 120 * (5 - 2 * c1 + 28 * t1 - 3 * Math.Pow(c1, 2) + 8 * Math.Pow(e1, 2) + 24 * Math.Pow(t1, 2)))
                         / Math.Cos(lat1);
            lon = ToDegrees(lon) + (zone * 6 - 183);

            latLong = new (lat, lon);
        }

        private static int GetZone(double longitude) => (int)Math.Floor((longitude + 180) / 6) + 1;
    }
}