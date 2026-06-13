using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game_SET_main_end : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject MainMenu;
    public GameObject Ending;

    [Header("Ending Text")]
    public Text Ending_txt;

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

    //시간 끝났을 때 호출할 함수 게임오버 or 게임클리어
    public void GameOver()
    {
        Time.timeScale = 0f;
        if (Ending_txt != null) Ending_txt.text = "GAME OVER\n시간 초과!";
        Ending.SetActive(true);

        // 1. 마우스 다시 보이기 (재시작 버튼 등을 누르기 위해)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 2. 플레이어 조작 스크립트 끄기
        SetPlayerScriptsEnabled(false);
    }

    public void GameClear()
    {
        Time.timeScale = 0f; // 게임 정지
        if (Ending_txt != null) Ending_txt.text = "GAME CLEAR!\n축하합니다!";
        Ending.SetActive(true);

        // 1. 마우스 다시 보이기
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 2. 플레이어 조작 스크립트 끄기
        SetPlayerScriptsEnabled(false);
    }

    // [Restart_Btn]에 연결할 함수
    public void RestartGame()
    {
        // 현재 씬을 처음부터 다시 로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 플레이어 조작 스크립트들의 활성화/비활성화를 한 번에 관리
    private void SetPlayerScriptsEnabled(bool isEnabled)
    {
        if (firstPersonLookScript != null) firstPersonLookScript.enabled = isEnabled;
        if (playerMovementScript != null) playerMovementScript.enabled = isEnabled;
        if (attackScript != null) attackScript.enabled = isEnabled;
    }
}