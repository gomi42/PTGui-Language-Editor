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
