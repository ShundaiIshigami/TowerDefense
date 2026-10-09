using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    
 



    public void StartButton()
    {

        SceneManager.LoadScene("main");

        

    }
    
    public void EndButton()
    {
        //ゲームを終了
        Application.Quit();//アプリケーションの終了
        #if UNITY_EDITOR//unityエディターの再生停止
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    
}
