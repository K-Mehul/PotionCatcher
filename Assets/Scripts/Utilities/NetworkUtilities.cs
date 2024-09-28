using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System;
using System.Collections.Generic;

public class NetworkUtilities
{
    public static string GetLocalIPV4(string preferredSubset = "192.168.")
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach(var ip in host.AddressList)
        {
            if(ip.AddressFamily  == AddressFamily.InterNetwork)
            {
                if (ip.ToString().StartsWith(preferredSubset))
                {
                    return ip.ToString();
                }
            }
        }
        return host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetworkV6)?.ToString();
    }

    public static string GetSubNetMask(string ipAddress)
    {
        foreach(var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            foreach(var uniCastIPAddressInformation in networkInterface.GetIPProperties().UnicastAddresses)
            {
                if(uniCastIPAddressInformation.Address.ToString() == ipAddress)
                {
                    return uniCastIPAddressInformation.IPv4Mask.ToString();
                }
            }
        }

        throw new Exception("Subnet mask not found");
    }

    public static List<string> GetIPRange(string ipAddress, string subnetMask)
    {
        List<string> ipRange = new List<string>();

        byte[] ipBytes = IPAddress.Parse(ipAddress).GetAddressBytes();
        byte[] maskBytes = IPAddress.Parse(subnetMask).GetAddressBytes();

        byte[] startIP = new byte[4];
        byte[] endIP = new byte[4];

        for(int i = 0; i < 4; i++)
        {
            startIP[i] = (byte)(ipBytes[i] & maskBytes[i]);
            endIP[i] = (byte)(ipBytes[i] | ~maskBytes[i]);
        }

        for(uint i = BitConverter.ToUInt32(startIP.Reverse().ToArray(),0); i <= BitConverter.ToUInt32(endIP.Reverse().ToArray()); i++)
        {
            ipRange.Add(new IPAddress(BitConverter.GetBytes(i).Reverse().ToArray()).ToString());
        }

        return ipRange;
    }
}
