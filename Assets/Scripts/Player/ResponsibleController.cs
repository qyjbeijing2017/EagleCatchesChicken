using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Mirror;
using UnityEngine;

public partial class NetworkController : IMirrorResponsible
{

    Dictionary<int, RequestBaseAsyncOption> m_ReqOptions = new Dictionary<int, RequestBaseAsyncOption>();

    Dictionary<string, MethodInfo> m_RespMethods = new Dictionary<string, MethodInfo>();

    byte[] Serialization(object value)
    {
        using (var writer = NetworkWriterPool.Get())
        {
            var type = value.GetType();
            var write = typeof(NetworkWriter).GetMethod("Write");
            var bound = write.MakeGenericMethod(type);
            bound.Invoke(writer, new object[] { value });
            return writer.ToArray();
        }
    }

    object Deserialization(byte[] data, Type type)
    {
        using (var reader = NetworkReaderPool.Get(data))
        {
            var read = typeof(NetworkReader).GetMethod("Read");
            var bound = read.MakeGenericMethod(type);
            return bound.Invoke(reader, null);
        }
    }

    void ScanResponsibleMethod()
    {
        var methodInfo = GetType().GetMethods();
        foreach (var method in methodInfo)
        {
            var attr = method.GetCustomAttribute<ResponsibleAttribute>();
            if (attr != null)
            {
                m_RespMethods[attr.responsibleKey] = method;
            }
        }
    }

    IEnumerator ResponsibleTimeoutMonitor() {
        yield return new WaitForSeconds(0.5f);
        foreach (var option in m_ReqOptions.Values)
        {
            if (option.isTimeout)
            {
                option.Timeout();
            }
        }
    }

    void InitClientResponsible()
    {
        NetworkClient.RegisterHandler<RequestMessage>(OnClientRequestMessage);
        NetworkClient.RegisterHandler<ResponseMessage>(OnClientResponseMessage);
    }

    void InitServerResponsible()
    {
        NetworkServer.RegisterHandler<RequestMessage>(OnServerRequestMessage);
        NetworkServer.RegisterHandler<ResponseMessage>(OnServerResponseMessage);
    }

    void OnClientRequestMessage(RequestMessage message)
    {
        CallResponseMessage(message, (ResponseMessage response) =>
        {
            NetworkClient.Send(response);
        });
    }

    void OnClientResponseMessage(ResponseMessage message)
    {
        m_ReqOptions.TryGetValue(message.messageId, out var option);
        if (option == null) return;
        if (message.isError)
        {
            throw new Exception(message.message.ToString());
        }
        else
        {
            var returnType = option.GetType().GetGenericArguments()[0];
            option.Response(Deserialization(message.message, returnType));
        }
        m_ReqOptions.Remove(message.messageId);
    }

    void OnServerRequestMessage(NetworkConnectionToClient conn, RequestMessage message)
    {
        CallResponseMessage(message, (ResponseMessage response) =>
        {
            conn.Send(response);
        });

    }

    void OnServerResponseMessage(NetworkConnectionToClient conn, ResponseMessage message)
    {
        m_ReqOptions.TryGetValue(message.messageId, out var option);
        if (option == null) return;
        if (message.isError)
        {
            throw new Exception(message.message.ToString());
        }
        else
        {
            var returnType = option.GetType().GetGenericArguments()[0];
            option.Response(Deserialization(message.message, returnType));
        }
        m_ReqOptions.Remove(message.messageId);
    }

    void CallResponseMessage(RequestMessage message, Action<ResponseMessage> callback)
    {
        m_RespMethods.TryGetValue(message.responsibleKey, out var method);
        if (method == null)
        {
            callback(new ResponseMessage()
            {
                messageId = message.messageId,
                isError = true,
                message = Serialization($"Not found responsibleKey: {message.responsibleKey}"),
            });
            return;
        }

        var parameterInfos = method.GetParameters();
        var param = new object[]{
            Deserialization(message.message, parameterInfos[0].ParameterType),
            new Action<object>((object value) =>
            {
                    callback(new ResponseMessage(){
                        messageId = message.messageId,
                        isError = false,
                        message = Serialization(value),
                    });
            })
        };
        var result = method.Invoke(this, param);

        if (result is IEnumerator)
        {
            StartCoroutine((IEnumerator)result);
        }
    }

    [Server]
    public MultipleRequestAsyncOption<RespType> RequestAllClients<RespType>(string responsibleKey, object message)
    {
        var id = Guid.NewGuid().GetHashCode();
        NetworkServer.SendToAll(new RequestMessage()
        {
            responsibleKey = responsibleKey,
            messageId = id,
            message = Serialization(message),
        });
        var option = new MultipleRequestAsyncOption<RespType>();
        m_ReqOptions.Add(id, option);
        return option;
    }

    [Server]
    public RequestAsyncOption<RespType> RequestClient<RespType>(string responsibleKey, object message, NetworkConnectionToClient conn)
    {
        var id = Guid.NewGuid().GetHashCode();
        conn.Send(new RequestMessage()
        {
            responsibleKey = responsibleKey,
            messageId = id,
            message = Serialization(message),
        });
        var option = new RequestAsyncOption<RespType>();
        m_ReqOptions.Add(id, option);
        return option;
    }

    [Client]
    public RequestAsyncOption<RespType> RequestServer<RespType>(string responsibleKey, object message)
    {
        var id = Guid.NewGuid().GetHashCode();
        NetworkClient.Send(new RequestMessage()
        {
            responsibleKey = responsibleKey,
            messageId = id,
            message = Serialization(message),
        });
        var option = new RequestAsyncOption<RespType>();
        m_ReqOptions.Add(id, option);
        return option;
    }

}
