using TMPro;
using Unity.Netcode;
using UnityEngine;

public class NetworkedTimer : NetworkBehaviour
{
    private float timeElapsed = 0;
    [SerializeField] private float timeDuration = 60f;

    [SerializeField] TMP_Text timerTxt;
     
    private void Update()
    {
        if (IsServer && GameManager.Instance.IsGameState())
        {
            timeElapsed += Time.deltaTime;

            UpdateTimerClientRPC(timeElapsed);
            
            if(timeElapsed >= timeDuration)
            {
                timeElapsed = 60f;
                GameManager.Instance.GameCompletedRPC();
            }
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateTimerClientRPC(float newTimeElapsed)
    {
        timeElapsed = newTimeElapsed;
        UpdateUI();
    }

    public float GetTimeElapsed()
    {
        return timeElapsed;
    }

    private void UpdateUI()
    {
        if(timerTxt != null)
        {
            timerTxt.text = $"Time Elapsed : {timeElapsed:F2} seconds remaining";
        }
    }
}
