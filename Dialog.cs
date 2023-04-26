//
// Author:
//   Michael Göricke
//
// Copyright (c) 2023
//
// This file is part of PTGui Language Editor.
//
// The PTGui Language Editor is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program. If not, see<http://www.gnu.org/licenses/>.

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PTGui_Language_Editor
{

    internal enum DialogOkNoCancel
    {
        Yes,
        No,
        Cancel
    }

    class Dialog
    {
        private TaskCompletionSource<DialogOkNoCancel>? taskCompletionSource;
        private FrameworkElement dialogBox;
        private static DialogLayer? dialogLayer;

        //////////////////////////////////////////////

        public DialogOkNoCancel DialogResult { get; set; }

        //////////////////////////////////////////////

        public static Dialog Create<T>(object viewModel)
        {
            var view = (FrameworkElement)Activator.CreateInstance(typeof(T))!;
            view.DataContext = viewModel;

            return new Dialog(view);
        }

        //////////////////////////////////////////////

        private Dialog(FrameworkElement dialogBox)
        {
            this.dialogBox = dialogBox;
        }

        //////////////////////////////////////////////

        public Task<DialogOkNoCancel> ShowDialog()
        {
            ShowFrame();

            taskCompletionSource = new TaskCompletionSource<DialogOkNoCancel>();
            return taskCompletionSource.Task;
        }

        //////////////////////////////////////////////

        public void Show()
        {
            ShowFrame();
        }

        //////////////////////////////////////////////

        public void Close()
        {
            CloseFrame();

            taskCompletionSource?.SetResult(DialogResult);
            taskCompletionSource = null;
        }

        //////////////////////////////////////////////

        private void ShowFrame()
        {
            if (dialogLayer == null)
            {
                dialogLayer = new DialogLayer();
            }

            dialogLayer.ShowDialog(dialogBox);
        }

        //////////////////////////////////////////////

        private void CloseFrame()
        {
            dialogLayer?.CloseDialog(dialogBox);
        }
    }
}
