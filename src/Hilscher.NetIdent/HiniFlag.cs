// Copyright 2005-2021 Daniel Bock. All rights reserved. See License.md in the project root for license information.

using System;

namespace Hilscher.netIdent
{
  [Flags]
  public enum HiniFlags : UInt32
  {
    INVALID = 0x00000000,
    ERROR_CODE_VALID = 0x80000000,
    MASTER_IP_ADDRESS_VALID = 0x40000000,
    MASTER_PORT_NUMBER_VALID = 0x20000000,
    DEVICE_TYPE_VALID = 0x02000000,
    DEVICE_NAME_VALID = 0x01000000,
    SERIAL_NUMBER_VALID = 0x04000000,
    ETHERNET_ADDRESS_VALID = 0x08000000,
    IP_ADDRESS_VALID = 0x10000000,
    ADDRESS_SWITCH_VALID = 0x00800000,
    ADDRESS_SWITCH_HEX_FORMAT = 0x00400000,
  }
}


