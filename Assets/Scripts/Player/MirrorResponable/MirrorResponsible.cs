using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

[AttributeUsage(AttributeTargets.Method)]
public class ResponsibleAttribute : PropertyAttribute
{
    public string responsibleKey;
    public ResponsibleAttribute(string responsibleKey)
    {
        this.responsibleKey = responsibleKey;
    }
}

public class RequestBaseAsyncOption : CustomYieldInstruction
{
    protected bool m_IsDone = false;
    public bool isDone => m_IsDone;
    public override bool keepWaiting => !m_IsDone;
}

public class RequestAsyncOption<T> : RequestBaseAsyncOption
{
    protected T m_Result;
    public T result => m_Result;

    public void Response(T value)
    {
        m_Result = value;
        m_IsDone = true;
    }
}

public class MultipleRequestAsyncOption<T> : RequestAsyncOption<List<T>>
{
    private HashSet<int> m_ReadyClients = new HashSet<int>();
    public HashSet<int> readyClients => m_ReadyClients;
    public void ResponseMultiple(T value, int connectionId)
    {
        m_Result.Add(value);
        m_ReadyClients.Add(connectionId);
        foreach (var conn in NetworkServer.connections)
        {
            if (!m_ReadyClients.Contains(conn.Key)) return;
        }
        Response(m_Result);
    }
}

public struct RequestMessage : NetworkMessage
{
    public int responsibleId;
    public int messageId;
    public string responsibleKey;
    public byte[] message;
}

public struct ResponseMessage : NetworkMessage
{
    public int messageId;
    public byte[] message;
}

public interface IMirrorResponsible
{
    public int responsibleId { get; }
    public RequestAsyncOption<RespType> RequestServer<RespType, ReqType>(
        string responsibleKey,
        ReqType message
        ) where ReqType : struct, NetworkMessage where RespType : struct, NetworkMessage;
    public MultipleRequestAsyncOption<RespType> RequestAllClients<RespType, ReqType>(
        string responsibleKey,
        ReqType message
        ) where ReqType : struct, NetworkMessage where RespType : struct, NetworkMessage;
    public RequestAsyncOption<RespType> RequestClient<RespType, ReqType>(
        string responsibleKey,
        ReqType message,
        NetworkConnection conn
        ) where ReqType : struct, NetworkMessage where RespType : struct, NetworkMessage;

}
