using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class ClientDiscovery : MonoBehaviour
{
    private UdpClient udpClient;
    private const int discoveryPort = 8888;

    private void Start()
    {
        udpClient = new UdpClient();
        udpClient.EnableBroadcast = true; // Enable broadcasting for discovery
    }

    public async Task<bool> CheckIfRoomExists(string ipAddress)
    {
        try
        {
            udpClient.Connect(ipAddress, discoveryPort);

            // Send a discovery message
            byte[] data = Encoding.ASCII.GetBytes("DISCOVER");
            await udpClient.SendAsync(data, data.Length);

            // Wait for a response
            var result = await udpClient.ReceiveAsync();
            string response = Encoding.ASCII.GetString(result.Buffer);
            Debug.Log($"Response from host: {response}");

            // Check if the response indicates a host
            return response == "HOST_FOUND";
        }
        catch (SocketException ex)
        {
            Debug.LogError($"SocketException: {ex.Message}");
            return false; // Indicates no room hosted
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Exception: {ex.Message}");
            return false; // Indicates no room hosted
        }
    }

    private void OnDestroy()
    {
        udpClient.Close(); // Clean up the UDP client
    }
}
