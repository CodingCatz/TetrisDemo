using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 遊戲倒數計時器
/// </summary>
public class CountdownTimer : MonoBehaviour
{
    #region 基本參數
    private Text _timerText;
    private Text timerText => _timerText ??= GetComponent<Text>();

    private CancellationTokenSource _cts;
    #endregion 基本參數

    private void OnDestroy()
    {//安全銷毀機制
        CancelTimer();
    }

    /// <summary>
    /// 非同步倒計時
    /// </summary>
    /// <param name="duration">執行秒數</param>
    /// <param name="onComplete">完成時回調操作</param>
    public async void StartTimer(float duration, Action onComplete)
    {
        CancelTimer();
        _cts = new CancellationTokenSource();
        //啟動計時
        await ProcessTimer(duration, _cts.Token);
    }

    /// <summary>
    /// 計時運算核心邏輯
    /// </summary>
    /// <param name="duration">時長</param>
    /// <param name="token">Task任務標記</param>
    /// <returns>Task任務</returns>
    private async Task ProcessTimer(float duration, CancellationToken token)
    {
        float time = duration;
        while (time > 0)
        {
            token.ThrowIfCancellationRequested();
            //刷新UI
            if (timerText) timerText.text = Mathf.CeilToInt(time).ToString();
            //時間以FPS速率減少：1/FPS(1秒/幀)
            time -= Time.deltaTime;
            //等一幀
            await Task.Yield();
        }
    }

    /// <summary>
    /// 取消計時任務
    /// </summary>
    private void CancelTimer()
    {
        if (_cts != null && !_cts.IsCancellationRequested)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
    }
}
