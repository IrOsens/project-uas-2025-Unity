using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeScreenManager : MonoBehaviour
{

    public void StartGame()
    {
    Debug.Log("Fungsi StartGame() terpanggil!");
    SceneManager.LoadScene("Level 1");
    Debug.Log("Mencoba memuat Level 1...");
    }

}