using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Reflection;


struct MirrorRequestGetMethodInfo
{
    public string path;
    public MethodInfo methodInfo;
    public object target;
}

[AttributeUsage(AttributeTargets.Method)]
public class MirrorGetAttribute : System.Attribute
{
    public string path;
    public MirrorGetAttribute(string path)
    {
        this.path = path;
    }
}

public class MirrorReseponseAsyncOperation<T> : CustomYieldInstruction
{
    public int id;
    public event Action<T> complete;

    public T result;

    private bool m_isDone = false;
    public bool isDone => m_isDone;

    public override bool keepWaiting => !m_isDone;

    public void Complete(byte[] data)
    {
        using (var reader = NetworkReaderPool.Get(data))
        {
            result = reader.Read<T>();
            complete?.Invoke(result);
            m_isDone = true;
        }
    }
}

public class MirrorRequest : MonoSingleton<MirrorRequest>
{
    struct MirrorRequestMessage : NetworkMessage
    {
        public int id;
        public string path;
        public byte[] body;
    }

    struct MirrorReseponseMessage : NetworkMessage
    {
        public int id;
        public byte[] body;
    }

    Dictionary<int, Action<byte[]>> requestMap = new Dictionary<int, Action<byte[]>>();

    #region scan
    Dictionary<string, MirrorRequestGetMethodInfo> methodMap = new Dictionary<string, MirrorRequestGetMethodInfo>();

    public void Register<T>(T handler)
    {
        var type = typeof(T);
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var method in methods)
        {
            var attribute = method.GetCustomAttribute<MirrorGetAttribute>();
            if (attribute != null)
            {
                var path = attribute.path;
                var methodInfo = method;
                var target = handler;
                methodMap.Add(path, new MirrorRequestGetMethodInfo()
                {
                    path = path,
                    methodInfo = methodInfo,
                    target = target,
                });
            }
        }
    }

    public void UnRegister<T>(T handler)
    {
        var type = typeof(T);
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var method in methods)
        {
            var attribute = method.GetCustomAttribute<MirrorGetAttribute>();
            if (attribute != null)
            {
                var path = attribute.path;
                methodMap.Remove(path);
            }
        }
    }
    #endregion

    #region client
    public void clientInit()
    {
        NetworkClient.RegisterHandler<MirrorRequestMessage>(OnClientRequest);
        NetworkClient.RegisterHandler<MirrorReseponseMessage>(OnClientResponse);
    }

    private void OnClientRequest(MirrorRequestMessage message)
    {
        if (methodMap.TryGetValue(message.path, out var methodInfo))
        {
            var result = methodInfo.methodInfo.Invoke(methodInfo.target, new object[] { });
            using (var writer = NetworkWriterPool.Get())
            {
                writer.Write(result);
                NetworkClient.Send(new MirrorReseponseMessage()
                {
                    id = message.id,
                    body = writer.ToArraySegment().Array,
                });
            }
        }
    }

    private void OnClientResponse(MirrorReseponseMessage message)
    {
    }
    #endregion

    #region server

    public void serverInit()
    {
        NetworkServer.RegisterHandler<MirrorRequestMessage>(OnServerRequest);
        NetworkServer.RegisterHandler<MirrorReseponseMessage>(OnServerResponse);
    }

    private void OnServerResponse(NetworkConnectionToClient client, MirrorReseponseMessage message)
    {
    }

    private void OnServerRequest(NetworkConnectionToClient client, MirrorRequestMessage message)
    {
    }

    #endregion

    public MirrorReseponseAsyncOperation<ResType> Send<ResType, ReqType>(Action<NetworkMessage> target, string path, ReqType message)
    {
        using (var writer = NetworkWriterPool.Get())
        {
            writer.Write(message);
            target(new MirrorRequestMessage()
            {
                id = Guid.NewGuid().GetHashCode(),
                path = path,
                body = writer.ToArraySegment().Array,
            });
        }

        var response = new MirrorReseponseAsyncOperation<ResType>()
        {
            id = Guid.NewGuid().GetHashCode(),
        };
        requestMap.Add(response.id, response.Complete);
        return response;
    }
}
