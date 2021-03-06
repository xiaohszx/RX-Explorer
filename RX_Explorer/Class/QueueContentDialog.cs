﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace RX_Explorer.Class
{
    /// <summary>
    /// 提供ContentDialog队列(按序)弹出的实现
    /// </summary>
    public class QueueContentDialog : ContentDialog
    {
        private static readonly AutoResetEvent Locker = new AutoResetEvent(true);

        private static int WaitCount = 0;

        /// <summary>
        /// 指示当前是否存在正处于弹出状态的ContentDialog
        /// </summary>
        public static bool IsRunningOrWaiting
        {
            get
            {
                return WaitCount != 0;
            }
        }

        /// <summary>
        /// 显示对话框
        /// </summary>
        /// <returns></returns>
        public new async Task<ContentDialogResult> ShowAsync()
        {
            _ = Interlocked.Increment(ref WaitCount);

            await Task.Run(() =>
            {
                Locker.WaitOne();
            }).ConfigureAwait(true);

            ContentDialogResult Result = await base.ShowAsync();

            _ = Interlocked.Decrement(ref WaitCount);

            Locker.Set();

            return Result;
        }

        /// <summary>
        /// 初始化QueueContentDialog
        /// </summary>
        public QueueContentDialog()
        {
            if (AppThemeController.Current.Theme == ElementTheme.Dark)
            {
                Background = Application.Current.Resources["DialogAcrylicBrush"] as Brush;
                RequestedTheme = ElementTheme.Dark;
            }
            else
            {
                RequestedTheme = ElementTheme.Light;
            }
        }
    }
}
