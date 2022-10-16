using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

// ReSharper disable InconsistentNaming

#if false
//TODO: Write a tool that will automatically create this class
public class UnityWebRequest : IDisposable
{
    public static void ClearCookieCache() => UnityEngine.Networking.UnityWebRequest.ClearCookieCache();
    public static void Delete(Uri uri) => UnityEngine.Networking.UnityWebRequest.Delete(uri);
    public static void Delete(string uri) => UnityEngine.Networking.UnityWebRequest.Delete(uri);
    public static void EscapeURL(string s) => UnityEngine.Networking.UnityWebRequest.EscapeURL(s);
    public static void EscapeURL(string s, Encoding e) => UnityEngine.Networking.UnityWebRequest.EscapeURL(s, e);

    public static void GenerateBoundary() => UnityEngine.Networking.UnityWebRequest.GenerateBoundary();

    // make them return This version of UWR
    public static void Get(Uri uri) => UnityEngine.Networking.UnityWebRequest.Get(uri);
    public static void Get(string uri) => UnityEngine.Networking.UnityWebRequest.Get(uri);
    public static void Head(Uri uri) => UnityEngine.Networking.UnityWebRequest.Head(uri);
    public static void Head(string uri) => UnityEngine.Networking.UnityWebRequest.Head(uri);

    public static void Post(Uri uri, string postData, string contentType) =>
        UnityEngine.Networking.UnityWebRequest.Post(uri, postData, contentType);

    // TODO: Add All overloads
    // public static void Post(string uri) => UnityEngine.Networking.UnityWebRequest.Post(uri);
    // public static void Put(Uri uri) => UnityEngine.Networking.UnityWebRequest.Put(uri);
    // public static void Put(string uri) => UnityEngine.Networking.UnityWebRequest.Put(uri);

    public void SerializeFormSections(List<IMultipartFormSection> multipartFormSections, byte[] boundary) =>
        UnityEngine.Networking.UnityWebRequest.SerializeFormSections(multipartFormSections, boundary);

    public void SerializeSimpleForm(Dictionary<string, string> formFields) =>
        UnityEngine.Networking.UnityWebRequest.SerializeSimpleForm(formFields);

    public static void UnEscapeURL(string s) => UnityEngine.Networking.UnityWebRequest.UnEscapeURL(s);
    public static void UnEscapeURL(string s, Encoding e) => UnityEngine.Networking.UnityWebRequest.UnEscapeURL(s, e);

    public static string kHttpVerbCREATE => UnityEngine.Networking.UnityWebRequest.kHttpVerbCREATE;
    public static string kHttpVerbDELETE => UnityEngine.Networking.UnityWebRequest.kHttpVerbCREATE;
    public static string kHttpVerbGET => UnityEngine.Networking.UnityWebRequest.kHttpVerbCREATE;
    public static string kHttpVerbHEAD => UnityEngine.Networking.UnityWebRequest.kHttpVerbCREATE;
    public static string kHttpVerbPOST => UnityEngine.Networking.UnityWebRequest.kHttpVerbCREATE;
    public static string kHttpVerbPUT => UnityEngine.Networking.UnityWebRequest.kHttpVerbCREATE;

    private readonly UnityEngine.Networking.UnityWebRequest _r;

    public CertificateHandler certificateHandler => _r.certificateHandler;
    public bool disposeCertificateHandlerOnDispose => _r.disposeCertificateHandlerOnDispose;
    public bool disposeDownloadHandlerOnDispose => _r.disposeDownloadHandlerOnDispose;
    public bool disposeUploadHandlerOnDispose => _r.disposeUploadHandlerOnDispose;
    public ulong downloadedBytes => _r.downloadedBytes;
    public DownloadHandler downloadHandler => _r.downloadHandler;
    public float downloadProgress => _r.downloadProgress;
    public string error => _r.error;
    public bool isDone => _r.isDone;
    public bool isModifiable => _r.isModifiable;
    public string method => _r.method;
    public int redirectLimit => _r.redirectLimit;
    public long responseCode => _r.responseCode;
    public UnityEngine.Networking.UnityWebRequest.Result result => _r.result;
    public int timeout => _r.timeout;
    public ulong uploadedBytes => _r.uploadedBytes;
    public UploadHandler uploadHandler => _r.uploadHandler;
    public float uploadProgress => _r.uploadProgress;
    public Uri uri => _r.uri;
    public string url => _r.url;
    public bool useHttpContinue => _r.useHttpContinue;

    public UnityWebRequest()
    {
        _r = new UnityEngine.Networking.UnityWebRequest();
    }

    public UnityWebRequest(string url)
    {
        _r = new UnityEngine.Networking.UnityWebRequest(url);
    }

    public UnityWebRequest(Uri uri)
    {
        _r = new UnityEngine.Networking.UnityWebRequest(uri);
    }

    public UnityWebRequest(string url, string method)
    {
        _r = new UnityEngine.Networking.UnityWebRequest(url, method);
    }

    public UnityWebRequest(Uri uri, string method)
    {
        _r = new UnityEngine.Networking.UnityWebRequest(uri, method);
    }

    public UnityWebRequest(string url, string method, DownloadHandler downloadHandler, UploadHandler uploadHandler)
    {
        _r = new UnityEngine.Networking.UnityWebRequest(url, method, downloadHandler, uploadHandler);
    }

    public UnityWebRequest(Uri uri, string method, DownloadHandler downloadHandler, UploadHandler uploadHandler)
    {
        _r = new UnityEngine.Networking.UnityWebRequest(uri, method, downloadHandler, uploadHandler);
    }

    public void Abort()
    {
        _r.Abort();
    }

    public string GetRequestHeader(string name)
    {
        return _r.GetRequestHeader(name);
    }

    public string GetResponseHeader(string name)
    {
        return _r.GetResponseHeader(name);
    }

    public Dictionary<string, string> GetResponseHeaders()
    {
        return _r.GetResponseHeaders();
    }

    public UnityWebRequestAsyncOperation SendWebRequest()
    {
        return _r.SendWebRequest();
    }

    public void SetRequestHeader(string name, string value)
    {
        _r.SetRequestHeader(name, value);
        requestHeaders[name] = value;
    }

    public void Dispose()
    {
        _r.Dispose();
    }

    #region NEW INTERFACE

    private readonly Dictionary<string, string> requestHeaders = new Dictionary<string, string>();

    public Dictionary<string, string> GetRequestHeaders()
    {
        return requestHeaders;
    }

    #endregion
}
#endif