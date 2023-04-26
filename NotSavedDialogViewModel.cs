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
