using UnityEngine;
using System.Collections;
using System.Text;

public class Utility  {
    private static StringBuilder builder = new StringBuilder();
    public static string Combine(int a, string b, int c)
    {
        builder.Remove(0, builder.Length);
        builder.Append(a);
        builder.Append(b);
        builder.Append(c);
        return builder.ToString();
    }
}
