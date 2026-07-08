using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    // Static fields persist across scene loads.
    public static int selectedLevel = 0;

    public void SelectLevel(int levelIndex)
    {
        selectedLevel = Mathf.Max(0, levelIndex);
        SceneManager.LoadScene("_Scene_0");
    }
}

