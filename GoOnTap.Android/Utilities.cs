using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public static class Utilities
    {
        public static int Diff(string a, string b)
        {
            if (a == null || b == null) //string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
                throw new ArgumentNullException();

            int lengthA = a.Length;
            int lengthB = b.Length;
            var distances = new int[lengthA + 1, lengthB + 1];
            for (int i = 0; i <= lengthA; distances[i, 0] = i++) ;
            for (int j = 0; j <= lengthB; distances[0, j] = j++) ;

            for (int i = 1; i <= lengthA; i++)
                for (int j = 1; j <= lengthB; j++)
                {
                    int cost = b[j - 1] == a[i - 1] ? 0 : 1;
                    distances[i, j] = Math.Min
                        (
                        Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                        distances[i - 1, j - 1] + cost
                        );
                }

            return distances[lengthA, lengthB];
        }
    }
}
