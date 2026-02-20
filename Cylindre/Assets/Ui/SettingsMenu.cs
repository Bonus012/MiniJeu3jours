using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        // Charger les valeurs sauvegardées
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

        // Appliquer au démarrage
        SetMusicVolume(musicSlider.value);
        SetSFXVolume(sfxSlider.value);

        // Lier les événements
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMusicVolume(float volume)
    {
        AudioListener.volume = volume; // Change ici si tu as un AudioMixer
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        // Si tu as des effets séparés, ajuste-les ici avec AudioMixer
        PlayerPrefs.SetFloat("SFXVolume", volume);
        // Tu peux aussi stocker ça pour que tes scripts SFX les utilisent plus tard
    }
}
