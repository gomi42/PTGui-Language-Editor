using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTGui_Language_Editor
{
    internal class NotSavedDialogViewModel
    {
        private Dialog dialog = null!;

        //////////////////////////////////////////////

        public NotSavedDialogViewModel()
        {
            OkCommand = new DelegateCommand(OnOk);
            NoCommand = new DelegateCommand(OnNo);
            CancelCommand = new DelegateCommand(OnCancel);
            Title = "Not Saved";
        }

        //////////////////////////////////////////////

        public DelegateCommand OkCommand { get; set; }
        public DelegateCommand NoCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        //////////////////////////////////////////////

        public string Title { get; set; }

        //////////////////////////////////////////////

        public string ErrorMessage { get; set; } = string.Empty;

        //////////////////////////////////////////////

        public Task<DialogOkNoCancel> ShowDialog()
        {
            if (string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = "Ein unbekannter Fehler ist aufgetreten.";
            }

            dialog = Dialog.Create<NotSavedDialog>(this);
            return dialog.ShowDialog();
        }

        //////////////////////////////////////////////

        private void OnOk()
        {
            dialog.DialogResult = DialogOkNoCancel.Yes;
            dialog.Close();
        }

        //////////////////////////////////////////////

        private void OnNo()
        {
            dialog.DialogResult = DialogOkNoCancel.No;
            dialog.Close();
        }

        //////////////////////////////////////////////

        private void OnCancel()
        {
            dialog.DialogResult = DialogOkNoCancel.Cancel;
            dialog.Close();
        }
    }
}
