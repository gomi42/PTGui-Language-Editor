namespace PTGui_Language_Editor
{
    using System;
    using System.Windows.Input;

    public class DelegateCommand : ICommand
    {
        //////////////////////////////////////////////

        private readonly Action execute;

        //////////////////////////////////////////////

        private readonly Func<bool>? canExecute;

        //////////////////////////////////////////////

        public event EventHandler? CanExecuteChanged;

        //////////////////////////////////////////////

        public DelegateCommand (Action execute)
            : this (execute, null)
        {
        }

        //////////////////////////////////////////////

        public DelegateCommand (Action execute, Func<bool>? canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException ("execute");
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        //////////////////////////////////////////////

        public void RaiseCanExecuteChanged ()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged (this, new EventArgs());
            }
        }

        //////////////////////////////////////////////

        public bool CanExecute (object? parameter)
        {
            return canExecute == null ? true : canExecute ();
        }

        //////////////////////////////////////////////

        public void Execute (object? parameter)
        {
            execute ();
        }
    }
}
