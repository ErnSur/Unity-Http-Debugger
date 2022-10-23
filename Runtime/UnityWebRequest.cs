using System.Collections.Generic;

public class UnityWebRequest : UnityEngine.Networking.UnityWebRequest
{
    private readonly Dictionary<string, string> _requestHeaders = new Dictionary<string, string>();

    public Dictionary<string, string> GetRequestHeaders()
    {
        return _requestHeaders;
    }

    public new void SetRequestHeader(string name, string value)
    {
        _requestHeaders[name] = value;
        base.SetRequestHeader(name, value);
    }
}