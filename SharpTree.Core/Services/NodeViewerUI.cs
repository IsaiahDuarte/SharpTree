using SharpTree.Core.Models;
using System;
using System.IO;
using Terminal.Gui;
using Terminal.Gui.Trees;

namespace SharpTree.Core.Services
{
    public class NodeViewerUI
    {
        private readonly Window _window;
        private readonly Toplevel _top;
        private readonly TreeView<INode> _treeView;
        private Button _btnOpen;
        private Button _btnSystemDrive;
        //private Button _btnUserProfile;
        //private Button _btnRemoteComputer;


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
                TreeBuilder = new DelegateTreeBuilder<INode>(
                    node => node.IsDirectory ? node.Children : null
                ),
                AspectGetter = node => $"{node.Name} - {NodeViewer.BytesToString(node.Size)}"
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
                X = 11,
                Y = Pos.Bottom(_treeView),
                Height = 1,
                Width = 10
            };
        }

        public void CreateWindow()
        {
            _window.ColorScheme.Normal = Application.Driver.MakeAttribute(Color.BrightBlue, Color.Black);
            _top.Add(_window);
            _btnOpen.Clicked += BtnOpen_Clicked;
            _btnSystemDrive.Clicked += BtnSysemDrive_Clicked;
            _treeView.AddObject(_node);
            _window.Add(_treeView, _btnOpen, _btnSystemDrive);
        }

        private async void BtnSysemDrive_Clicked()
        {
            string? systemDrive = Path.GetPathRoot(Environment.SystemDirectory);
            if (systemDrive == null) { return; }

            var waitingMessage = GetWaitingLabel();
            _window.Add(waitingMessage);
            _window.SetNeedsDisplay();

            await Task.Run(() =>
            {
                _treeView.Remove(_node);
                _node = FileSystemReader.Read(systemDrive);
                _window.Remove(waitingMessage);
            });

            _treeView.AddObject(_node);
        }

        private Label GetWaitingLabel()
        {
            return new Label("Loading, please wait...")
            {
                X = Pos.Center(),
                Y = Pos.Center()
            };
        }

        private async void BtnOpen_Clicked()
        {
            var dialog = new OpenDialog
            {
                CanChooseDirectories = true,
                CanChooseFiles = false,
                Title = "Select a directory"
            };
            Application.Run(dialog);
            if(dialog.Canceled) { return; }
            string? path = dialog.FilePath.ToString();
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var waitingMessage = GetWaitingLabel();
            _window.Add(waitingMessage);
            _window.SetNeedsDisplay();

            await Task.Run(() =>
            {
                _treeView.Remove(_node);
                _node = FileSystemReader.Read(path);
            });

            _treeView.AddObject(_node);
            _window.Remove(waitingMessage);
            dialog.Dispose();
        }
    }
}