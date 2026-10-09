using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuManager : MonoBehaviour
{
    //メニューキャンバス
    [SerializeField] GameObject menuCanvas;

    

    //メニュー表示フラグ(メニューかキャンバスを必ず非表示に）
    public bool isMenuOpen = false;

    


    




    
    void Start()
    {
        menuCanvas.SetActive(false);
    }

    
    


    


    //メニュー関連---------------------------------------------------------
    void Update()
    {
        // Mキーが押されたら
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMenu();
        }


    }
    public void ToggleMenu()
    {
        //メニューを閉じる
        if (isMenuOpen)
        {
            CloseMenu();
        }
        //メニューを開ける
        else
        {
            OpenMenu();
        }
    }
    // メニューを表示
    public void OpenMenu()
    {
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(true);
            isMenuOpen = true;
            // ゲームを一時停止
            Time.timeScale = 0f;

        }
        else { Debug.LogError("menuCanvasがインスペクターで設定されていません！"); }
    }
    // メニューを非表示
    public void CloseMenu()
    {
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(false);
            isMenuOpen = false;
            // ゲームを再開
            Time.timeScale = 1f;

        }
    }


    




    public void TitleButton()
    {
        Time.timeScale = 1f;
        menuCanvas.SetActive(false);
        isMenuOpen = false;


        //タイトル画面に戻る
        SceneManager.LoadScene("title");

    }

    



}
