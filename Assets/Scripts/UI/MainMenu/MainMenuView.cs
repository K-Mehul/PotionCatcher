using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
 
public class MainMenuView : MonoBehaviour
{


    [Header("Screens")]
    [SerializeField] GameObject MainScreen;
    [SerializeField] GameObject SettingScreen;
    [SerializeField] GameObject MatchScreen;
    private GameObject[] screens;

    [Header("Main"), Space()]
    [SerializeField] Button playBtn;
    [SerializeField] Button settingBtn;
    [SerializeField] Button quitBtn;


    [Header("Settings"),Space()]
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Button settings_backBtn;


    [Header("MatchScreen"), Space()]
    [SerializeField] Button match_Backbtn;

    private void Awake()
    {
        screens = new[] { MainScreen, SettingScreen,MatchScreen };
        EnableScreen(MainScreen);

        //MainScreen
        playBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlaySound2D("Click");
            EnableScreen(MatchScreen);
        });

        settingBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlaySound2D("Click");
            EnableScreen(SettingScreen);
        });

        quitBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlaySound2D("Click");
            Application.Quit();
        });


        //Setting Screen
        musicSlider.onValueChanged.AddListener(UpdateMusicVolume);
        sfxSlider.onValueChanged.AddListener(UpdateSoundVolume);
        settings_backBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlaySound2D("Click");
            EnableScreen(MainScreen);
        });

        //Match Screen
        match_Backbtn.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound2D("Click");
            EnableScreen(MainScreen);
        });
    }

    public void EnableScreen(GameObject screen)
    {
        screens.ToList().ForEach(x => x.SetActive(false));
        var screenEnable = screens.FirstOrDefault(x => x.name == screen.name);
        screenEnable.SetActive(true);
    }


    private void Start()
    {
        LoadVolume();
        MusicManager.Instance.PlayMusic("MainMenu");
    }

    public void Play()
    {
        //MusicManager.Instance.PlayMusic("Game");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void UpdateMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
        SaveVolume();
    }

    public void UpdateSoundVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
        SaveVolume();
    }

    public void SaveVolume()
    {
        audioMixer.GetFloat("MusicVolume", out float musicVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);

        audioMixer.GetFloat("SFXVolume", out float sfxVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }

    public void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume",0);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume",0);
    }
}