using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Toastmappers
{
  public class RelayCommand : ICommand
  {
    private Action _action;
    private Func<bool> _canExecute;

    /// <summary>
    /// Creates instance of the command handler
    /// </summary>
    /// <param name="action">Action to be executed by the command</param>
    /// <param name="canExecute">A bolean property to containing current permissions to execute the command</param>
    public RelayCommand(Action action, Func<bool> canExecute)
    {
      _action = action;
      _canExecute = canExecute;
    }

    /// <summary>
    /// Wires CanExecuteChanged event 
    /// </summary>
    public event EventHandler CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }

    /// <summary>
    /// Forcess checking if execute is allowed
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public bool CanExecute(object parameter)
    {
      return _canExecute.Invoke();
    }

    public void Execute(object parameter)
    {
      _action();
    }
  }
  //public class RelayCommand : ICommand
  //{
  //  readonly Action<object> _execute;
  //  //readonly Action<object> _executeWithParam;
  //  readonly Predicate<object> canExecute;
  //  readonly Action _executeNoParam;
  //  //readonly Func<Task> _executeTask;
  //  private event EventHandler CanExecuteChangedInternal;

  //  public RelayCommand(Action<object> execute)
  //    : this(execute, DefaultCanExecute)
  //  {

  //  }

  //  public RelayCommand(Action execute)
  //  {
  //    if (execute == null)
  //      throw new ArgumentNullException("execute");

  //    _executeNoParam = execute;
  //  }
  //  public RelayCommand(Action<object> execute, Predicate<object> canExecute)
  //  {
  //    if (execute == null)
  //    {
  //      throw new ArgumentNullException("execute");
  //    }

  //    if (canExecute == null)
  //    {
  //      throw new ArgumentNullException("canExecute");
  //    }

  //    this._execute = execute;
  //    this.canExecute = canExecute;
  //  }



  //  public event EventHandler CanExecuteChanged
  //  {
  //    add
  //    {
  //      CommandManager.RequerySuggested += value;
  //      this.CanExecuteChangedInternal += value;
  //    }

  //    remove
  //    {
  //      CommandManager.RequerySuggested -= value;
  //      this.CanExecuteChangedInternal -= value;
  //    }
  //  }

  //  public bool CanExecute(object parameter)
  //  {
  //    return this.canExecute != null && this.canExecute(parameter);
  //  }

  //  public void Execute(object parameter)
  //  {
  //    if (_execute != null)
  //      this._execute(parameter);
  //  }

  //  public void OnCanExecuteChanged()
  //  {
  //    EventHandler handler = this.CanExecuteChangedInternal;
  //    if (handler != null)
  //    {
  //      handler.Invoke(this, EventArgs.Empty);
  //    }
  //  }

  //  //public void Destroy()
  //  //{
  //  //  this.canExecute = _ => false;
  //  //  this._execute = _ => { return; };
  //  //}

  //  private static bool DefaultCanExecute(object parameter)
  //  {
  //    return true;
  //  }

  //  public void Execute()
  //  { }
  //}
}
