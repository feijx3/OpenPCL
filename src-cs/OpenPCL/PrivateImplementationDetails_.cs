internal static class PrivateImplementationDetails_
{
    internal static uint ComputeStringHash(string s)
    {
        uint hash = 0;
        if (s != null)
        {
            hash = 2166136261u;
            for (int i = 0; i < s.Length; i++) hash = (s[i] ^ hash) * 16777619u;
        }
        return hash;
    }
}
