using TMPro;
using UnityEngine;

public class LocalUIManager : MonoBehaviour,ILocalGameStateListener
{
    [Header("Elements")]
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject waitingPanel;
    [SerializeField] GameObject networkScanPanel;
    [SerializeField] GameObject joiningPanel;
    [SerializeField] TMP_Text ipTxt;


    [Space()]
    [Header("NetworkScan Elements")]
    [SerializeField] GameObject joiningBtn;
    [SerializeField] TMP_Text selectedIPTxt;

    private GameObject[] panels;

    private void Awake()
    {
        panels = new[] { menuPanel, waitingPanel, networkScanPanel, joiningPanel };
        IPButton.onClicked += IPButtonClickedCallBack;
        joiningBtn.SetActive(false);
    }


    private void IPButtonClickedCallBack(string ip)
    {
        joiningBtn.SetActive(true);
        selectedIPTxt.text = ip;
    }


    private void Start()
    {
        ipTxt.text = $"LOCAL IP : {NetworkUtilities.GetLocalIPV4()}";
    }


    private void ShowPanel(GameObject panel)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(panels[i] == panel);
        }
    }

    public void GameStateChangedCallback(LocalGameState state)
    {
        switch (state)
        {
            case LocalGameState.MENU:
                ShowPanel(menuPanel);
                break;

            case LocalGameState.JOINING:
                ShowPanel(joiningPanel);
                break;

            case LocalGameState.SCANNING:
                joiningBtn.SetActive(false);
                selectedIPTxt.text = string.Empty;
                ShowPanel(networkScanPanel);
                break;

            case LocalGameState.WAITING:
                ShowPanel(waitingPanel);
                break;
        }
    }
}
