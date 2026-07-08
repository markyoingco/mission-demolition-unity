using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameMode
{
    idle,
    playing,
    levelEnd
}

public class MissionDemolition : MonoBehaviour
{
    static private MissionDemolition S;     // a private Singleton

    [Header("Inscribed")] 
    public Text uitLevel;                   // The UIText_Level Text
    public Text uitShots;                   // The UIText_Shots Text
    public Vector3 castlePos;               // The place to put castles
    public GameObject[] castles;            // An array of the castles

    [Header("Dynamic")] 
    public int level;                       // The current level
    public int levelMax;                    // The number of levels
    public int shotsTaken;
    public GameObject castle;               // The current castle
    public GameMode mode = GameMode.idle;
    public string showing = "Show SlingShot";   // FollowCam mode

    void Start()
    {
        S = this;

        level = LevelSelectManager.selectedLevel;
        shotsTaken = 0;
        levelMax = castles.Length;
        StartLevel();
    }

    void StartLevel()
    {
        // Get rid of the old castle if one exists
        if (castle != null)
        {
            Destroy(castle);
        }
        
        // Destroy old projectiles if they exist
        Projectile.DESTROY_PROJECTILES();
        
        // Instantiate the new castle
        castle = Instantiate<GameObject>(castles[level]);
        castle.transform.position = castlePos;
        
        // Reset the goal
        Goal.goalMet = false;

        UpdateGUI();

        mode = GameMode.playing;

        // Reset the per-level slingshot shot counter
        Slingshot slingshot = FindFirstObjectByType<Slingshot>();
        if (slingshot != null)
        {
            slingshot.ResetShotCounter();
        }
        
        // Zoom out to show both
        FollowCam.SWITCH_VIEW(FollowCam.eView.both);
    }

    void UpdateGUI()
    {
        // show the data in the GUITexts
        uitLevel.text = "Level: " + (level + 1) + " of " + levelMax;
        uitShots.text = "Shots Taken: " + shotsTaken;
    }

    void Update()
    {
        UpdateGUI();
        
        // Check for level end
        if ((mode == GameMode.playing) && Goal.goalMet)
        {
            // Change mode to stop checking for level end
            mode = GameMode.levelEnd;
            
            // Zoom out to show both
            FollowCam.SWITCH_VIEW(FollowCam.eView.both);
            
            // Start the next level in 2 seconds
            Invoke("NextLevel", 2f);
        }
    }

    void NextLevel()
    {
        level++;
        if (level >= levelMax)
        {
            SceneManager.LoadScene("_Scene_LevelSelect");
            return;
        }
        StartLevel();
    }

    static public void SHOT_FIRED()
    {
        S.shotsTaken++;
    }

    static public GameObject GET_CASTLE()
    {
        return S.castle;
    }

    public void ResetCurrentLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
