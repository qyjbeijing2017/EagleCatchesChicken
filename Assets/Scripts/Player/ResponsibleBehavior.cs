using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using System.Reflection;
using System;

public class ResponsibleBehavior : NetworkBehaviour, IMirrorResponsible
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

    void ScanResponsible()
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

    public override void OnStartServer()
    {
        ScanResponsible();
        StartCoroutine(ResponsibleTimeoutMonitor());
    }

    public override void OnStartClient()
    {
        if (isClientOnly){
            ScanResponsible();
            StartCoroutine(ResponsibleTimeoutMonitor());
        }
    }

    [TargetRpc]
    void TargetClientRequest(NetworkConnectionToClient target, RequestMessage message)
    {
        CallResponse(message, (ResponseMessage response) =>
        {
            CmdServerResponse(response);
        });
    }

    [ClientRpc]
    void RpcClientRequest(RequestMessage message)
    {
        CallResponse(message, (ResponseMessage response) =>
        {
            CmdServerResponse(response);
        });
    }

    [TargetRpc]
    void TargetClientResponse(NetworkConnectionToClient target, ResponseMessage message)
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

    [Command]
    void CmdServerRequest(RequestMessage message)
    {
        CallResponse(message, (ResponseMessage response) =>
        {
            TargetClientResponse(NetworkServer.connections[message.connectionId], response);
        });

    }

    [Command]
    void CmdServerResponse(ResponseMessage message)
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

    void CallResponse(RequestMessage message, Action<ResponseMessage> callback)
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
        RpcClientRequest(new RequestMessage()
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
        TargetClientRequest(conn, new RequestMessage()
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
        CmdServerRequest(new RequestMessage()
        {
            connectionId = NetworkClient.connection.connectionId,
            responsibleKey = responsibleKey,
            messageId = id,
            message = Serialization(message),
        });
        var option = new RequestAsyncOption<RespType>();
        m_ReqOptions.Add(id, option);
        return option;
    }


    IEnumerator ResponsibleTimeoutMonitor()
    {
        yield return new WaitForSeconds(0.5f);
        foreach (var option in m_ReqOptions.Values)
        {
            if (option.isTimeout)
            {
                option.Timeout();
            }
        }
    }
}
