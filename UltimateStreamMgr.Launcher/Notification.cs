using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace UltimateStreamMgr.Launcher
{
    public class Notification : IDisposable
    {
        private readonly NotifyIcon _notifyIcon;

        public Notification(string title, string message, NotificationType type)
        {
            _notifyIcon = new NotifyIcon()
            {
                Text = title,
                BalloonTipTitle = title,
                BalloonTipText = message,
                Visible = true,
            };

            try
            {
                _notifyIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location);
            }
            catch
            {
                _notifyIcon.Icon = SystemIcons.Application;
            }

            switch (type)
            {
                case NotificationType.Error:
                    _notifyIcon.BalloonTipIcon = ToolTipIcon.Error;
                    break;
                case NotificationType.Warning:
                    _notifyIcon.BalloonTipIcon = ToolTipIcon.Warning;
                    break;
                case NotificationType.Info:
                    _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            _notifyIcon.ShowBalloonTip(5);
        }

        public void Dispose()
        {
            _notifyIcon.Visible = false;
        }
    }

    public enum NotificationType
    {
        Error,
        Warning, 
        Info
    }
}
