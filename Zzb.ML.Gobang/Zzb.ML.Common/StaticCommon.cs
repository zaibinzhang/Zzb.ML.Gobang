using System;
using System.Text;

namespace Zzb.ML.Common
{
    public static class StaticCommon
    {
        public static string ToMapString(this int[,] values)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    sb.Append(values[i, j]);
                }
            }
            return sb.ToString();
        }
    }
}
