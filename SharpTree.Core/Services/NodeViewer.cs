﻿using SharpTree.Core.Models;
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

            var top = Application.Top;
            var win = new Window("Directory Explorer")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            top.Add(win);
            var treeView = new TreeView<INode>()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                CanFocus = true,
                TreeBuilder = new DelegateTreeBuilder<INode>(
                    node => node.IsDirectory ? node.Children : null
                ),
                AspectGetter = node => $"{node.Name} - {BytesToString(node.Size)}"
            };

            treeView.AddObject(node);
            win.Add(treeView);
            Application.Run();
        }

        static String BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 2);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }
    }
}