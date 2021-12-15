// Copyright 2005-2021 Daniel Bock. All rights reserved. See License.md in the project root for license information.
using System;

namespace Hilscher.netIdent
{
  public enum OpCodeEnum : UInt32
  {
    IDENTIFY_REQUEST = 0x01000000,
    IDENTIFY_REPLY = 0x02000000,
    SET_IP_ADDRESS_REQUEST = 0x03000000,
    SET_IP_ADDRESS_REPLY = 0x04000000,
  }
}


