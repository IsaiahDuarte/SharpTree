using SharpTree.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Terminal.Gui;
using Terminal.Gui.Trees;

namespace SharpTree.Core.Services
{
    public class NodeViewerUI
    {
        private readonly Window _window;
        private readonly Toplevel _top;
        private readonly TreeView<INode> _treeView;
        private readonly Button _btnOpen;
        private readonly Button _btnSystemDrive;
        private readonly Button _btnUserProfile;

        private INode _node { get; set; }

        public NodeViewerUI(INode node)
        {
            _top = Application.Top;
            _node = node;
            _window = new Window("Directory Explorer - Press ESC to Close")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            _treeView = new TreeView<INode>()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 2,
                CanFocus = true,
                TreeBuilder = new DelegateTreeBuilder<INode>(GetChildrenForNode),
                AspectGetter = GetAspectString
            };
            _btnOpen = new Button()
            {
                Text = "Open",
                X = 0,
                Y = Pos.Bottom(_treeView),
                Height = 1,
                Width = 10
            };
            _btnSystemDrive = new Button()
            {
                Text = "System Drive",
                X = Pos.Right(_btnOpen),
                Y = Pos.Bottom(_treeView),
                Height = 1,
                Width = 10
            };
            _btnUserProfile = new Button()
            {
                Text = "User Profile",
                X = Pos.Right(_btnSystemDrive) + 1,
                Y = Pos.Bottom(_treeView),
                Height = 1,
                Width = 10
            };
        }

        public void CreateWindow()
        {
            string systemDrive = Path.GetPathRoot(Environment.SystemDirectory);
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            _window.ColorScheme.Normal = Application.Driver.MakeAttribute(Color.BrightBlue, Color.Black);
            _top.Add(_window);
            _btnOpen.Clicked += BtnOpen_Clicked;
            _btnSystemDrive.Clicked += () => UpdateNode(systemDrive);
            _btnUserProfile.Clicked += () => UpdateNode(userProfile);
            _treeView.AddObject(_node);
            _window.Add(_treeView, _btnOpen, _btnSystemDrive, _btnUserProfile);
        }
        private Label GetWaitingLabel(string path)
        {
            return new Label($"Loading, please wait...\n{path}")
            {
                X = Pos.Center(),
                Y = Pos.Center()
            };
        }

        private async void UpdateNode(string path)
        {
            _btnUserProfile.Enabled = false;
            _btnSystemDrive.Enabled = false;
            _btnOpen.Enabled = false;
            if (string.IsNullOrWhiteSpace(path)) { return; }
            if (!Directory.Exists(path))
            {
                MessageBox.ErrorQuery("Error", $"Path {path} does not exist", "OK");
                return;
            }

            var waitingMessage = GetWaitingLabel(path);
            _window.Add(waitingMessage);
            _window.SetNeedsDisplay();

            await Task.Run(() =>
            {
                _treeView.Remove(_node);
                _node = FileSystemReader.Read(path);
                _window.Remove(waitingMessage);
            });

            _treeView.AddObject(_node);
            _btnUserProfile.Enabled = true;
            _btnSystemDrive.Enabled = true;
            _btnOpen.Enabled = true;
        }

        private void BtnOpen_Clicked()
        {

            var dialog = new OpenDialog
            {
                CanChooseDirectories = true,
                CanChooseFiles = false,
                Title = "Select a directory"
            };
            dialog.ColorScheme.Normal = Application.Driver.MakeAttribute(Color.BrightBlue, Color.Black);
            Application.Run(dialog);

            if (dialog.Canceled) { return; }
            string path = dialog.FilePath.ToString();
            if (string.IsNullOrEmpty(path)) { return; }
            UpdateNode(path);
            dialog.Dispose();
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