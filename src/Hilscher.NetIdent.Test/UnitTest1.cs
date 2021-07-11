using System;
using System.Net;
using System.Net.NetworkInformation;
using Hilscher.netIdent;
using Xunit;


namespace Hilscher.NetIdent.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Serialize_Deserialize_NetIdentProtocolMessageTest()
        {
            var expectedMessage = new NetIdentProtocolMessage
            {
                MagicCookie = "HINI",
                OpCode = OpCodeEnum.IDENTIFY_REQUEST,
                ErrorCode = 0,
                MasterIpAddress = IPAddress.Parse("10.0.0.0"),
                PortNumber = 234,
                IpAddress = IPAddress.Parse("1.2.3.4"),
                MacAddress = NetIdentProtocolMessage.MacAddressZero,
                SerialNumber = 123412,
                DeviceType = 587890,
                DeviceName = "adfasdf",
                AddrSwitch = 3,
            };

            var byteData = NetIdentProtocolMessage.Serialize(expectedMessage);
            var actualMessage = NetIdentProtocolMessage.Deserialize(byteData);

            Assert.Equal(expectedMessage.MagicCookie,actualMessage.MagicCookie);
            Assert.Equal(expectedMessage.OpCode,actualMessage.OpCode);
            Assert.Equal(expectedMessage.ErrorCode,actualMessage.ErrorCode);
            Assert.Equal(expectedMessage.MasterIpAddress,actualMessage.MasterIpAddress);
            Assert.Equal(expectedMessage.PortNumber,actualMessage.PortNumber);
            Assert.Equal(expectedMessage.IpAddress,actualMessage.IpAddress);
            Assert.Equal(expectedMessage.MacAddress,actualMessage.MacAddress);
            Assert.Equal(expectedMessage.SerialNumber,actualMessage.SerialNumber);
            Assert.Equal(expectedMessage.DeviceType,actualMessage.DeviceType);
            Assert.Equal(expectedMessage.DeviceName,actualMessage.DeviceName);
            Assert.Equal(expectedMessage.AddrSwitch,actualMessage.AddrSwitch);
            Assert.Equal(expectedMessage.HiniFlags,actualMessage.HiniFlags);
    }
    }
}
