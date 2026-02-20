using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    // Pour afficher/masquer le panneau d'options (si utilisé)
    public GameObject optionsPanel;

    // Lancer la scène du jeu
    public void PlayGame()
    {
        Debug.Log("Play button clicked!");
        SceneManager.LoadScene("Game"); // Change "Game" selon le nom exact de ta scène
    }

    // Afficher les options (UI panel)
    public void ShowOptions()
    {
        Debug.Log("Options button clicked!");
        if (optionsPanel != null)
            optionsPanel.SetActive(true);
    }

    // Fermer le jeu
    public void QuitGame()
    {
        Debug.Log("Quit button clicked!");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Pour arrêter dans l'éditeur Unity
#endif
    }
}
