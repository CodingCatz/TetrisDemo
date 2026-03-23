using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// [單例]音源播放管理
/// </summary>
public class AudioManager : MonoBehaviour
{
    #region 基礎元件
    private AudioSource _sfxSource;
    /// <summary>
    /// 音效撥放元件
    /// </summary>
    public AudioSource SfxSource
    {
        get
        {
            if (_sfxSource == null)
            {
                _sfxSource = gameObject.AddComponent<AudioSource>();
                _sfxSource.loop = false;//預設不循環撥放音效
            }
            return _sfxSource;
        }
    }
    private AudioSource _bgmSource;
    /// <summary>
    /// 背景音樂播放元件
    /// </summary>
    public AudioSource BgmSource
    {
        get
        {
            if (_bgmSource == null)
            {
                _bgmSource = gameObject.AddComponent<AudioSource>();
                _bgmSource.loop = true;//預設循環撥放音樂
            }
            return _bgmSource;
        }
    }
    #endregion 基礎元件

    #region 物件單例模式
    private static AudioManager _instance;
    /// <summary>
    /// 音樂音效撥放單例實體
    /// </summary>
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {//自我實體驗證1：搜尋場景
                _instance = FindAnyObjectByType<AudioManager>();
                if (_instance == null)
                {//自我實體驗證2：搜尋場景未找到直接新增
                    _instance = new GameObject("AudioManager").AddComponent<AudioManager>();
                }
            }
            return _instance;
        }
    }
    #endregion 物件單例模式

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {//不小心產生出來的分身
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 單次撥放音檔
    /// </summary>
    /// <param name="clip">音效音檔</param>
    /// /// <param name="vol">音量</param>
    public void PlaySFX(AudioClip clip, float vol = 1f)
    {
        SfxSource.volume = vol;
        SfxSource.PlayOneShot(clip);
    }

    /// <summary>
    /// 撥放音樂音檔
    /// </summary>
    /// <param name="clip">音樂音檔</param>
    /// <param name="vol">音量</param>
    public void PlayBGM(AudioClip clip, float vol = 1f)
    {
        BgmSource.volume = vol;
        BgmSource.clip = clip;
        BgmSource.Play();
    }
}
