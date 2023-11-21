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
    public float timeout = 5f;
    public float startTime = Time.time;
    public bool isTimeout => Time.time - startTime > timeout;
    public virtual void Response(object value, NetworkConnectionToClient conn = null)
    {
        m_IsDone = true;
    }

    public virtual void Timeout()
    {
        m_IsDone = true;
    }
}

public class RequestAsyncOption<T> : RequestBaseAsyncOption
{
    protected T m_Result;
    public T result => m_Result;

    public event Action<T> completed;

    public override void Response(object value, NetworkConnectionToClient conn = null)
    {
        m_Result = (T)value;
        m_IsDone = true;
        completed?.Invoke(m_Result);
    }

    public override void Timeout()
    {
        completed?.Invoke(m_Result);
        base.Timeout();
    }
}

public class MultipleRequestAsyncOption<T> : RequestAsyncOption<List<T>>
{
    private HashSet<int> m_ReadyClients = new HashSet<int>();
    public HashSet<int> readyClients => m_ReadyClients;
    public void Response(T value, NetworkConnectionToClient conn = null)
    {
        if(m_Result == null) {
            m_Result = new List<T>();
        }
        m_Result.Add(value);
        m_ReadyClients.Add(conn.connectionId);
        foreach (var connection in NetworkServer.connections)
        {
            if (!m_ReadyClients.Contains(connection.Key)) return;
        }
        base.Response(m_Result);
    }
}

public struct RequestMessage : NetworkMessage
{
    public int connectionId;
    public int messageId;
    public string responsibleKey;
    public byte[] message;
}

public struct ResponseMessage : NetworkMessage
{
    public bool isError;
    public int messageId;
    public byte[] message;
}

public interface IMirrorResponsible
{
    public RequestAsyncOption<RespType> RequestServer<RespType>(
        string responsibleKey,
        object message
        );
    public MultipleRequestAsyncOption<RespType> RequestAllClients<RespType>(
        string responsibleKey,
        object message
        );
    public RequestAsyncOption<RespType> RequestClient<RespType>(
        string responsibleKey,
        object message,
        NetworkConnectionToClient conn
        );

}
