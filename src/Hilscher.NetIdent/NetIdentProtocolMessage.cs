// Copyright 2005-2021 Daniel Bock. All rights reserved. See License.md in the project root for license information.

using System;
using System.Net;
using System.Text;
using System.IO;
using System.Net.NetworkInformation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

namespace Hilscher.netIdent
{
  class IPAddressConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return (objectType == typeof(IPAddress));
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      writer.WriteValue(value.ToString());
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      return IPAddress.Parse((string)reader.Value);
    }
  }

  class PhysicalAddressConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return (objectType == typeof(PhysicalAddress));
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      writer.WriteValue(value.ToString());
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      return IPAddress.Parse((string)reader.Value);
    }
  }

  public class NetIdentProtocolMessage
  {
    public static PhysicalAddress MacAddressZero = new PhysicalAddress(new byte[] { 0, 0, 0, 0, 0, 0 });
    private static Random random = new Random();
    public NetIdentProtocolMessage()
    {
      int number = random.Next(Int32.MinValue, Int32.MaxValue);
      this.TransactionID = (uint)(number + (uint)Int32.MaxValue);
      this.MasterIpAddress = IPAddress.Any;
      this.MacAddress = MacAddressZero;
      this.IpAddress = IPAddress.Any;
      this.PortNumber = NetIdentPorts.MasterPort;
    }

    public string MagicCookie = "HINI";
    public UInt32 Version { get; private set; } = 1000;
    [JsonConverter(typeof(StringEnumConverter))]
    public OpCodeEnum OpCode { get; set; }
    public UInt32 TransactionID { get; private set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public HiniFlags HiniFlags { get => GetHilFlag(); }
    public UInt32 ErrorCode;
    public IPAddress MasterIpAddress;
    public Int32 PortNumber;
    [JsonConverter(typeof(IPAddressConverter))]
    public IPAddress IpAddress;
    [JsonConverter(typeof(PhysicalAddressConverter))]
    public PhysicalAddress MacAddress;
    public uint DeviceType;
    public uint SerialNumber;
    public string DeviceName = string.Empty;
    public UInt32 AddrSwitch;

    public static NetIdentProtocolMessage Deserialize(byte[] packetData)
    {
      using (MemoryStream memoryStream = new MemoryStream(packetData))
      {
        return NetIdentProtocolMessage.Deserialize(memoryStream);
      }
    }

    private HiniFlags GetHilFlag()
    {
      HiniFlags flags = HiniFlags.INVALID;
      if (this.PortNumber != 0)
      {
        flags |= HiniFlags.MASTER_PORT_NUMBER_VALID;
      }
      if (this.MasterIpAddress != IPAddress.Any)
      {
        flags |= HiniFlags.MASTER_IP_ADDRESS_VALID;
      }
      if (!this.MacAddress.Equals(MacAddressZero))
      {
        flags |= HiniFlags.ETHERNET_ADDRESS_VALID;
      }
      if (this.IpAddress != IPAddress.Any)
      {
        flags |= HiniFlags.IP_ADDRESS_VALID;
      }
      if (this.DeviceType != 0)
      {
        flags |= HiniFlags.DEVICE_TYPE_VALID;
      }
      if (this.SerialNumber != 0)
      {
        flags |= HiniFlags.SERIAL_NUMBER_VALID;
      }
      return flags;
    }

    public static NetIdentProtocolMessage Deserialize(Stream stream)
    {
      using var binData = new BinaryReader(stream, new System.Text.UTF8Encoding(), true);
      var data = new NetIdentProtocolMessage();

      data.MagicCookie = Encoding.ASCII.GetString(BitConverter.GetBytes(binData.ReadUInt32()));
      data.Version = SwapUInt32(binData.ReadUInt32());
      data.OpCode = (OpCodeEnum)binData.ReadUInt32();
      data.TransactionID = binData.ReadUInt32();
      var hiniFlags = (HiniFlags)binData.ReadUInt32();
      data.ErrorCode = binData.ReadUInt32();
      data.MasterIpAddress = new IPAddress(binData.ReadBytes(4));
      data.PortNumber = IPAddress.NetworkToHostOrder(binData.ReadInt32());
      data.IpAddress = new IPAddress(binData.ReadBytes(4));
      data.MacAddress = new PhysicalAddress(binData.ReadBytes(6));
      data.SerialNumber = SwapUInt32(binData.ReadUInt32());
      data.DeviceType = SwapUInt32(binData.ReadUInt32());
      data.DeviceName = Encoding.ASCII.GetString(binData.ReadBytes(64)).TrimEnd('\0');
      data.AddrSwitch = binData.ReadUInt32();

      return data;
    }

    public static byte[] Serialize(NetIdentProtocolMessage data)
    {
      using MemoryStream memStream = new MemoryStream();
      using var binData = new BinaryWriter(memStream);
      binData.Write(BitConverter.ToUInt32(Encoding.ASCII.GetBytes(data.MagicCookie), 0));
      binData.Write(SwapUInt32(data.Version));
      binData.Write((UInt32)data.OpCode);
      binData.Write(data.TransactionID);
      binData.Write((UInt32)data.HiniFlags);
      binData.Write(data.ErrorCode);
      binData.Write(data.MasterIpAddress.GetAddressBytes());
      binData.Write(IPAddress.HostToNetworkOrder(data.PortNumber));
      binData.Write(data.IpAddress.GetAddressBytes());
      binData.Write(data.MacAddress.GetAddressBytes());
      binData.Write(SwapUInt32(data.SerialNumber));
      binData.Write(SwapUInt32(data.DeviceType));
      var deviceNameBin = new byte[64];
      var src = Encoding.ASCII.GetBytes(data.DeviceName);
      Buffer.BlockCopy(src, 0, deviceNameBin, 0, src.Length);
      binData.Write(deviceNameBin);
      binData.Write(data.AddrSwitch);


      return memStream.ToArray();
    }
    static UInt32 SwapUInt32(UInt32 toSwap)
    {
      UInt32 tmp = 0;
      tmp = toSwap >> 24;
      tmp = tmp | ((toSwap & 0xff0000) >> 8);
      tmp = tmp | ((toSwap & 0xff00) << 8);
      tmp = tmp | ((toSwap & 0xff) << 24);
      return tmp;
    }
  }
}


