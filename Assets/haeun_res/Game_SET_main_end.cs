using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Game_SET_main_end : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject MainMenu;
    public GameObject Ending;

    [Header("Ending Text")]
    public TextMeshProUGUI Ending_txt;

    [Header("Team Script Reference")]
    public Basket_Controller basketController;

    [Header("Player Scripts to Disable")]
    public FirstPersonLook firstPersonLookScript;
    public MonoBehaviour playerMovementScript;  
    public MonoBehaviour attackScript;         

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 1. 처음엔 시작 화면만 켜고 엔딩은 끔
        MainMenu.SetActive(true);
        Ending.SetActive(false);

        // 2. 게임 속 모든 물리, 애니메이션 정지
        Time.timeScale = 0f;

        // 3. 마우스 보이기&잠금 해제
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 4. 플레이어 조작 스크립트 끄기
        SetPlayerScriptsEnabled(false);
    }

    public void StartGame()
    {
        MainMenu.SetActive(false); // 시작 화면 끄기
        Time.timeScale = 1f;

        // 1. 마우스 숨기기&화면 중앙에 잠금
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 2. 플레이어 조작 스크립트 다시 켜기
        SetPlayerScriptsEnabled(true);
    }

    //시간 끝났을 때 호출할 함수 게임오버 or 게임클리어 판단
    public void CheckGameResult()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SetPlayerScriptsEnabled(false);

        if (basketController != null && Ending_txt != null)
        {
            if (basketController.score >= basketController.goal)
            {
                Ending_txt.text = "GAME CLEAR!";
            }
            else
            {
                Ending_txt.text = "GAME OVER\n시간 초과!";
            }
        }

        Ending.SetActive(true);
    }

    // [Restart_Btn]에 연결할 함수
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // [Quit_Btn]에 연결할 함수 (게임 종료)
    public void QuitGame()
    {
        // 유니티 에디터에서 실행 중일 경우 플레이 모드를 종료
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // 플레이어 조작 스크립트들의 활성화/비활성화를 한 번에 관리
    private void SetPlayerScriptsEnabled(bool isEnabled)
    {
        if (firstPersonLookScript != null) firstPersonLookScript.enabled = isEnabled;
        if (playerMovementScript != null) playerMovementScript.enabled = isEnabled;
        if (attackScript != null) attackScript.enabled = isEnabled;
    }
}