using SharpTree.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui.Trees;
using Terminal.Gui;

namespace SharpTree.Core.Services
{
    public class NodeViewer
    {
        public static void Show(INode node)
        {
            Application.Init();
            Application.QuitKey = Key.Esc;
            Colors.Base.Normal = Application.Driver.MakeAttribute(Color.BrightBlue, Color.Black);

            var top = Application.Top;
            var win = new Window("Directory Explorer - Press ESC to Close")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            win.ColorScheme.Normal = Application.Driver.MakeAttribute(Color.BrightBlue, Color.Black);

            top.Add(win);
            var treeView = new TreeView<INode>()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 2,
                CanFocus = true,
                TreeBuilder = new DelegateTreeBuilder<INode>(GetChildrenForNode),
                AspectGetter = GetAspectString
            };

            var btnOpen = new Button()
            {
                Text = "Open",
                X = 0,
                Y = Pos.Bottom(treeView),
                Height = 1,
                Width = 10
            };
            btnOpen.Clicked += () =>
            {
                MessageBox.Query("Open", "You clicked the button", "Ok");
            };

            treeView.AddObject(node);
            win.Add(treeView, btnOpen);
            Application.Run();
            top.Dispose();
            Application.Shutdown();
        }

        private static IEnumerable<INode> GetChildrenForNode(INode node)
        {
            return node.IsDirectory ? node.Children : null;
        }

        private static string GetAspectString(INode node)
        {
            return string.Format("{0} - {1}", node.Name, BytesToString(node.Size));
        }

        private static string BytesToString(long byteCount)
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