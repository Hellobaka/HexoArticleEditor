﻿namespace HexoArticleEditor
{
    public class Debouncer
    {
        private CancellationTokenSource cancellationTokenSource = new();

        /// <summary>
        /// 防抖
        /// </summary>
        /// <param name="callback">要执行的方法</param>
        /// <param name="waitTime">等待时间（毫秒）</param>
        public void Debounce(Action callback, int waitTime)
        {
            // 取消之前的延时调用
            cancellationTokenSource.Cancel();
            cancellationTokenSource = new CancellationTokenSource();

            Task.Delay(waitTime, cancellationTokenSource.Token)
                .ContinueWith(task =>
                {
                    if (!task.IsCanceled)
                    {
                        callback();
                    }
                });
        }
    }
}
