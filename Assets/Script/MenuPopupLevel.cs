using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPopupLevel : MonoBehaviour
{
    public void KembaliKeHomeScreen()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("Home");
        Debug.Log("Kembali ke HomeScreen dari pop-up menang.");
    }
}