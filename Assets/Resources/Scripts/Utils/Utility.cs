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
	public static byte LerpColorChannel(byte start, byte end, float percent)
	{
		int delta = (int)end - (int)start;
        percent = Mathf.Max(Mathf.Min(percent, 1f), 0f);
		return (byte)((int)start + (delta * percent));
	}
    public static Vector3 RotateVector(float angle)
    {
        Vector3 vector = new Vector3(Mathf.Sin(angle * Mathf.PI / 180f), Mathf.Cos(angle * Mathf.PI / 180f));
        return vector;
    }
}
