using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPScanner : MonoBehaviour, ILocalGameStateListener
{
    private List<Ping> pings = new();
    private float timer = 0;
    private Coroutine checkPingCoroutine;

    [Header("Elements")]
    [SerializeField] IPButton ipButtonPrefab;
    [SerializeField] Transform ipButtonsParent;


    public void GameStateChangedCallback(LocalGameState localGameState)
    {
        if (localGameState != LocalGameState.SCANNING)
            return;

        Scan();
    }

    private void Scan()
    {
        if (checkPingCoroutine != null) StopCoroutine(checkPingCoroutine);
        timer = 0;

        pings.Clear();

        string localIP = NetworkUtilities.GetLocalIPV4();
        string subNetMask = NetworkUtilities.GetSubNetMask(localIP);

        List<string> ipAddresses = NetworkUtilities.GetIPRange(localIP, subNetMask);

        Debug.Log("Scanning....");

        foreach (string ipAddress in ipAddresses)
            pings.Add(new Ping(ipAddress));

        checkPingCoroutine = StartCoroutine(CheckPingsCoroutine());
    }

    IEnumerator CheckPingsCoroutine()
    {
        bool allDone = false;

        while (!allDone)
        {
            allDone = true;

            for (int i = pings.Count - 1; i >= 0; i--)
            {
                Ping ping = pings[i];

                if (ping.isDone)
                {
                    if (ping.time >= 0)
                    {
                        IPFound(ping.ip);
                        pings.RemoveAt(i);
                    }
                }
                else
                {
                    allDone = false;
                }
            }
            timer += Time.deltaTime;

            if(timer >= 5)
            {
                allDone = true;
                Debug.Log("TimeOut");
            }

            yield return null;
            Debug.Log("Scan Completed");
        }
    }

    private void IPFound(string ip)
    {
        if(ipButtonsParent.childCount > 0)
        {
            for(int i = 0; i < ipButtonsParent.childCount; i++)
            {
                IPButton childBtn = ipButtonsParent.GetChild(i).GetComponent<IPButton>();
                if (childBtn.IP == ip)
                    return;
            }
        }

        IPButton ipButton = Instantiate(ipButtonPrefab, ipButtonsParent);
        ipButton.Configure(ip);
    }
}
