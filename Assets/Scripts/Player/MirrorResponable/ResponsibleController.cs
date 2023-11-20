using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Mirror;

public partial class NetworkController : IMirrorResponsible
{
    public int responsibleId => -1;

    Dictionary<int, RequestBaseAsyncOption> m_ReqOptions = new Dictionary<int, RequestBaseAsyncOption>();

    Dictionary<string, MethodInfo> m_RespMethods = new Dictionary<string, MethodInfo>();


    void InitClientResponsible()
    {
        var methodInfo = GetType().GetMethods();
        foreach (var method in methodInfo)
        {
            var attr = method.GetCustomAttribute<ResponsibleAttribute>();
            if (attr != null)
            {
                m_RespMethods.Add(attr.responsibleKey, method);
            }
        }

        NetworkClient.RegisterHandler<RequestMessage>(OnClientRequestMessage);
        NetworkClient.RegisterHandler<ResponseMessage>(OnClientResponseMessage);
    }

    void InitServerResponsible()
    {
        var methodInfo = GetType().GetMethods();
        foreach (var method in methodInfo)
        {
            var attr = method.GetCustomAttribute<ResponsibleAttribute>();
            if (attr != null)
            {
                m_RespMethods.Add(attr.responsibleKey, method);
            }
        }

        NetworkServer.RegisterHandler<RequestMessage>(OnServerRequestMessage);
        NetworkServer.RegisterHandler<ResponseMessage>(OnServerResponseMessage);
    }

    void OnClientRequestMessage(RequestMessage message)
    {
        
    }

    void OnClientResponseMessage(ResponseMessage message)
    {

    }

    void OnServerRequestMessage(NetworkConnectionToClient conn, RequestMessage message)
    {

    }

    void OnServerResponseMessage(NetworkConnectionToClient conn, ResponseMessage message)
    {

    }

    [Server]
    public MultipleRequestAsyncOption<RespType> RequestAllClients<RespType, ReqType>(string responsibleKey, ReqType message)
        where ReqType : struct, NetworkMessage where RespType : struct, NetworkMessage
    {
        throw new System.NotImplementedException();
    }

    [Server]
    public RequestAsyncOption<RespType> RequestClient<RespType, ReqType>(string responsibleKey, ReqType message, NetworkConnection conn)
        where ReqType : struct, NetworkMessage where RespType : struct, NetworkMessage
    {
        throw new System.NotImplementedException();
    }

    [Server]
    public RequestAsyncOption<RespType> RequestServer<RespType, ReqType>(string responsibleKey, ReqType message)
        where ReqType : struct, NetworkMessage where RespType : struct, NetworkMessage
    {
        throw new System.NotImplementedException();
    }

}
