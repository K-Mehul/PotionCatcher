using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[RequireComponent(typeof(Button))]
public class IPButton : MonoBehaviour
{
    [SerializeField] TMP_Text ipTxt;
    public string IP { get; private set; }

    public static Action<string> onClicked;

    public void Configure(string ip)
    {
        ipTxt.text = ip;
        IP = ip;

        GetComponent<Button>().onClick.AddListener(() => { onClicked?.Invoke(IP); });
    }
}
