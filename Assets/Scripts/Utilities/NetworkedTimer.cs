using System.Collections.Generic;
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
            timeDuration -= Time.deltaTime;

            UpdateTimerClientRPC(timeDuration);
            
            if(timeDuration <= 0)
            {
                timeDuration = 0f;
                UpdateTimerClientRPC(timeDuration);
                GameManager.Instance.GameCompletedRPC();
            }
        }
    }


    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateTimerClientRPC(float newTimeElapsed)
    {
        timeDuration = newTimeElapsed;
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
            int minutes = Mathf.FloorToInt(timeDuration / 60);
            int seconds = Mathf.FloorToInt(timeDuration % 60);

            timerTxt.text = $"{minutes:00}:{seconds:00}";
        }
    }
}
