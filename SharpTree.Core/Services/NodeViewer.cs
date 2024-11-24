using SharpTree.Core.Models;
using System;
using Terminal.Gui;

namespace SharpTree.Core.Services
{
    public class NodeViewer
    {
        public static void Show(INode node)
        {
            try
            {
                Application.Init();
                Application.QuitKey = Key.Esc;
                Colors.Base.Normal = Application.Driver.MakeAttribute(Color.BrightBlue, Color.Black);

                var nodeViewerUI = new NodeViewerUI(node);
                nodeViewerUI.CreateWindow();

                Application.Run();
                Application.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Query("Error", ex.Message, "OK");
                Application.Shutdown();
            }
        }

        public static string BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 2);
            return (Math.Sign(byteCount) * num).ToString() + ' ' + suf[place];
        }
    }
}