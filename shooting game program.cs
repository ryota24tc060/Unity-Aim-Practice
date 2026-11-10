using System.Collections.Generic;
using UnityEngine;
public class ShootingGameProgram : MonoBehaviour
{
    enum GameState
    {
        Title,
        Playing,
        Result
    }
    GameState gameState = GameState.Title;
    List<GameObject> targets = new List<GameObject>();
    Camera mainCamera;
    int score = 0;
    int shotCount = 0;

    float mouseSensitivity = 2f;
    float rotationX = 0f;
    float rotationY = 0f;
    GUIStyle titleStyle;
    GUIStyle buttonStyle;
    GUIStyle messageStyle;
    void Start()
    {
        mainCamera = Camera.main;
        SetBackgroundColor();
    }
    void Update()
    {
        if (gameState != GameState.Playing)
        {
            return;
        }

        MoveCamera();

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndGame();
        }
    }
    void SetBackgroundColor()
    {
        if (mainCamera != null)
        {
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = new Color(0.08f, 0.13f, 0.22f);
        }
    }
    void CreateStyles()
    {
        titleStyle = new GUIStyle();
        titleStyle.fontSize = 110;
        titleStyle.normal.textColor = Color.white;
        titleStyle.alignment = TextAnchor.MiddleCenter;

        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 70;
        buttonStyle.alignment = TextAnchor.MiddleCenter;

        messageStyle = new GUIStyle();
        messageStyle.fontSize = 65;
        messageStyle.normal.textColor = Color.white;
        messageStyle.alignment = TextAnchor.MiddleCenter;
    }
    void MoveCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotationY += mouseX;
        rotationX -= mouseY;

        rotationX = Mathf.Clamp(rotationX, -85f, 85f);

        mainCamera.transform.rotation =
            Quaternion.Euler(rotationX, rotationY, 0f);
    }
    void StartGame()
    {
        score = 0;
        shotCount = 0;

        rotationX = mainCamera.transform.eulerAngles.x;
        rotationY = mainCamera.transform.eulerAngles.y;

        gameState = GameState.Playing;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SpawnTargets();
    }
    void SpawnTargets()
    {
        ClearTargets();

        for (int i = 0; i < 3; i++)
        {
            GameObject target = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            target.transform.localScale = Vector3.one * 0.8f;

            float x = Random.Range(-3f, 3f);
            float y = Random.Range(-2f, 2f);

            Vector3 position =
                mainCamera.transform.position +
                mainCamera.transform.forward * 12f +
                mainCamera.transform.right * x +
                mainCamera.transform.up * y;

            target.transform.position = position;

            targets.Add(target);
        }
    }
    void ClearTargets()
    {
        foreach (GameObject target in targets)
        {
            if (target != null)
            {
                Destroy(target);
            }
        }

        targets.Clear();
    }
    void Shoot()
    {
        shotCount++;

        Ray ray = mainCamera.ScreenPointToRay(
            new Vector3(Screen.width / 2f, Screen.height / 2f, 0f)
        );

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            GameObject hitTarget = hit.collider.gameObject;

            if (targets.Contains(hitTarget))
            {
                targets.Remove(hitTarget);

                Destroy(hitTarget);

                score++;

                SpawnOneTarget();
            }
        }
    }
    void SpawnOneTarget()
    {
        GameObject target = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        target.transform.localScale = Vector3.one * 0.8f;

        float x = Random.Range(-3f, 3f);
        float y = Random.Range(-2f, 2f);

        Vector3 position =
            mainCamera.transform.position +
            mainCamera.transform.forward * 12f +
            mainCamera.transform.right * x +
            mainCamera.transform.up * y;

        target.transform.position = position;

        targets.Add(target);
    }
    void EndGame()
    {
        gameState = GameState.Result;

        ClearTargets();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ReturnToTitle()
    {
        gameState = GameState.Title;
    }
    void OnGUI()
    {
        if (titleStyle == null)
        {
            CreateStyles();
        }

        float centerX = Screen.width / 2f;
        float screenHeight = Screen.height;

        if (gameState == GameState.Title)
        {
            DrawBlackBackground();

            GUI.Label(
                new Rect(0, screenHeight * 0.12f, Screen.width, 180),
                "Aim練習",
                titleStyle
            );

            if (GUI.Button(
                new Rect(centerX - 350, screenHeight * 0.52f, 700, 180),
                "ゲームスタート",
                buttonStyle))
            {
                StartGame();
            }
        }
        else if (gameState == GameState.Playing)
        {
            GUI.Label(
                new Rect(20, 120, 550, 100),
                "命中数：" + score,
                messageStyle
            );

            GUI.Label(
                new Rect(20, 220, 550, 100),
                "射撃回数：" + shotCount,
                messageStyle
            );

            GUI.Label(
                new Rect(centerX - 50, screenHeight / 2f - 70, 100, 140),
                "+",
                titleStyle
            );
        }
        else if (gameState == GameState.Result)
        {
            DrawBlackBackground();

            float accuracy = 0f;

            if (shotCount > 0)
            {
                accuracy = (float)score / shotCount * 100f;
            }

            GUI.Label(
                new Rect(0, screenHeight * 0.08f, Screen.width, 180),
                "ゲーム終了",
                titleStyle
            );

            GUI.Label(
                new Rect(0, screenHeight * 0.28f, Screen.width, 100),
                "命中数：" + score,
                messageStyle
            );

            GUI.Label(
                new Rect(0, screenHeight * 0.38f, Screen.width, 100),
                "射撃回数：" + shotCount,
                messageStyle
            );

            GUI.Label(
                new Rect(0, screenHeight * 0.48f, Screen.width, 100),
                "命中率：" + accuracy.ToString("F1") + "%",
                messageStyle
            );

            if (GUI.Button(
                new Rect(centerX - 350, screenHeight * 0.63f, 700, 130),
                "もう一度遊ぶ",
                buttonStyle))
            {
                StartGame();
            }

            if (GUI.Button(
                new Rect(centerX - 350, screenHeight * 0.78f, 700, 130),
                "タイトルに戻る",
                buttonStyle))
            {
                ReturnToTitle();
            }
        }
    }
    void DrawBlackBackground()
    {
        GUI.backgroundColor = Color.black;

        GUI.Box(
            new Rect(0, 0, Screen.width, Screen.height),
            ""
        );
        GUI.backgroundColor = Color.white;
    }
}