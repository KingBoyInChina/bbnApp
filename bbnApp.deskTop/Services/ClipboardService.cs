using Avalonia.Controls.ApplicationLifetimes;

namespace bbnApp.deskTop.Services
{
    public class ClipboardService(IClassicDesktopStyleApplicationLifetime liftime)
    {
        public void CopyToClipboard(string text) => liftime.MainWindow?.Clipboard?.SetTextAsync(text);
    }
}