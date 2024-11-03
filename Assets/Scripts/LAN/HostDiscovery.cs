using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class HostDiscovery : MonoBehaviour
{
    private UdpClient udpClient;
    private const int discoveryPort = 8888; // Port used for discovery

    private void Start()
    {
        // Start the UDP client on the designated port
        udpClient = new UdpClient(discoveryPort);
        StartListeningForRequests();
    }

    private async void StartListeningForRequests()
    {
        // Keep listening for incoming discovery requests
        while (true)
        {
            var receivedResults = await udpClient.ReceiveAsync();
            string message = Encoding.ASCII.GetString(receivedResults.Buffer);
            Debug.Log($"Received message: {message}");

            if (message == "DISCOVER") // Check if the message is a discovery request
            {
                // Respond to the discover request with a confirmation
                byte[] responseData = Encoding.ASCII.GetBytes("HOST_FOUND");
                await udpClient.SendAsync(responseData, responseData.Length, receivedResults.RemoteEndPoint);
                Debug.Log($"Responded to {receivedResults.RemoteEndPoint} with HOST_FOUND");
            }
        }
    }

    private void OnDestroy()
    {
        // Clean up the UDP client when the object is destroyed
        udpClient.Close();
    }
}
