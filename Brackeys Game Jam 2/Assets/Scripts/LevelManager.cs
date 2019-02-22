using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static int CurrentLevelIndex = 0;

    private static LevelDef[] Levels;

    [SerializeField]
    private GameObject WallPrefab = null;

    [SerializeField]
    private Lovable LovablePrefab = null;

    [SerializeField]
    private TMPro.TextMeshProUGUI LevelText = null;

    [SerializeField]
    private TMPro.TextMeshProUGUI DescriptionText = null;

    public static int Lovables = 0;

    private void Start()
    {
        if (Levels == null)
            Levels = Resources.LoadAll<LevelDef>("Levels").OrderBy(x => x.Number).ToArray();

        LoadLevel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Retry();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Skip();
        }
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Skip()
    {
        Win();
    }

    private void LoadLevel()
    {
        LevelDef level = Levels[CurrentLevelIndex];

        LevelText.text = $"{level.Number}/{Levels.Length}";
        DescriptionText.text = level.DescriptionText;

        Lovables = 0;

        string layout = level.LevelLayout;

        int row = 0;

        foreach (var line in layout.Split(new string[] { System.Environment.NewLine }, System.StringSplitOptions.None))
        {
            int col = 0;

            foreach (var letter in line)
            {
                switch (letter)
                {
                    case 'W':
                        Instantiate(WallPrefab, new Vector3(col, -row, 0), Quaternion.identity, transform);
                        break;

                    case 'U':
                        Lovable upLovable = Instantiate(LovablePrefab, new Vector3(col, -row, 0), Quaternion.identity, transform);
                        upLovable.SetRotation(Lovable.UpRotation);
                        Lovables++;
                        break;

                    case 'R':
                        Lovable rightLovable = Instantiate(LovablePrefab, new Vector3(col, -row, 0), Quaternion.identity, transform);
                        rightLovable.SetRotation(Lovable.RightRotation);
                        Lovables++;
                        break;

                    case 'D':
                        Lovable downLovable = Instantiate(LovablePrefab, new Vector3(col, -row, 0), Quaternion.identity, transform);
                        downLovable.SetRotation(Lovable.DownRotation);
                        Lovables++;
                        break;

                    case 'L':
                        Lovable leftLovable = Instantiate(LovablePrefab, new Vector3(col, -row, 0), Quaternion.identity, transform);
                        leftLovable.SetRotation(Lovable.LeftRotation);
                        Lovables++;
                        break;

                    default:
                        break;
                }

                col++;
            }

            row++;
        }
    }

    public static void Win()
    {
        if (CurrentLevelIndex == Levels.Length - 1)
        {
            CurrentLevelIndex = 0;
            SceneManager.LoadScene("Win");
            return;
        }

        CurrentLevelIndex++;
        SceneManager.LoadScene("Main");
    }
}
