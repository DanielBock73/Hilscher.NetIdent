// Copyright 2005-2021 Daniel Bock. All rights reserved. See License.md in the project root for license information.

namespace Hilscher.netIdent
{
  using System;
  using System.Net;
  using System.Net.Sockets;
  using System.Threading;
  using System.Threading.Tasks;

  public class NetIdentSocket
  {
    public static Task SendAsync(NetIdentProtocolMessage data, Action<NetIdentProtocolMessage> deviceFoundFunc, int timeout = 6000)
    {
      UdpClient udpClient = new UdpClient();
      udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, NetIdentPorts.MasterPort));
      udpClient.Client.ReceiveTimeout = timeout;

      var from = new IPEndPoint(0, 0);
      var task = Task.Run(() =>
      {
        try
        {
          while (true)
          {
            var recvBuffer = udpClient.Receive(ref from);
            deviceFoundFunc(NetIdentProtocolMessage.Deserialize(recvBuffer));
          }
        }
        catch (SocketException)
        {
        }
      });

      var dataBin = NetIdentProtocolMessage.Serialize(data);
      udpClient.Send(dataBin, dataBin.Length, new IPEndPoint(IPAddress.Broadcast, NetIdentPorts.SlavePort));

      task.ContinueWith(p =>
      {
        udpClient.Close();
      });

      return task;
    }
  }
}