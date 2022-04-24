using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
/// <summary> 
/// A base class that implements the infrastructure for property change notification and automatically performs UI thread marshalling. 
/// </summary> 
public abstract class PropertyChangedBase : INotifyPropertyChanged
{
    /// <summary> 
    /// Occurs when a property value changes. 
    /// </summary> 
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Sets a property value and calls NotifyPropertyChanged when the new value differs from the current value.
    /// </summary>
    /// <returns>    
    /// Returns true if the property changes.
    ///</returns>
    protected bool SetProperty<T>(ref T backingField, T value, Expression<Func<T>> property)
    {
        var changed = !EqualityComparer<T>.Default.Equals(backingField, value);
        if (changed)
        {
            backingField = value;
            NotifyPropertyChanged(getName(property));
        }
        return changed;
    }

    /// <summary>
    /// Sets a property value and calls NotifyPropertyChanged when the new value differs from the current value.
    /// </summary>
    /// <returns>    
    /// Returns true if the property changes.
    ///</returns>
    protected bool SetProperty<T>(ref T backingField, T value, [System.Runtime.CompilerServices.CallerMemberName] string name = "")
    {
        var changed = !EqualityComparer<T>.Default.Equals(backingField, value);
        if (changed)
        {
            backingField = value;
            NotifyPropertyChanged(name);
        }
        return changed;
    }

    /// <summary>
    /// Raises the PropertyChanged event for the specified property.
    /// </summary>
    protected virtual void NotifyPropertyChanged<T>(Expression<Func<T>> property)
    {
        NotifyPropertyChanged(getName(property));
    }

    /// <summary>
    /// Raises the PropertyChanged event for the specified property.
    /// </summary>
    protected virtual void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = "")
    {
        NotifyPropertyChanged(new PropertyChangedEventArgs(name));
    }

    /// <summary>
    /// Raises the PropertyChanged event for the specified property.
    /// </summary>
    protected virtual void NotifyPropertyChanged(PropertyChangedEventArgs args)
    {
        PropertyChanged?.Invoke(this, args);
    }

    private static string getName<T>(Expression<Func<T>> property)
    {
        var lambda = (LambdaExpression)property;

        MemberExpression memberExpression;
        if (lambda.Body is UnaryExpression)
        {
            var unaryExpression = (UnaryExpression)lambda.Body;
            memberExpression = (MemberExpression)unaryExpression.Operand;
        }
        else memberExpression = (MemberExpression)lambda.Body;

        return memberExpression.Member.Name;
    }
}

internal interface IViewModel
{
    object Model { get; set; }
}

internal interface IView
{
    ContentControl View { get; set; }
}

/// <summary>
/// Base class for ViewModels. Implements INotifyPropertyChange.
/// </summary>
public abstract class ViewModelBase : PropertyChangedBase, IViewModel, IView
{
    /// <summary>
    /// Gets or sets the view-model's model object.
    /// </summary>
    public object Model { get; set; }

    ContentControl IView.View
    {
        get;
        set;
    }
}

public class RelayCommand : PropertyChangedBase, ICommand
{
    private static List<WeakReference> _relayCommands = new List<WeakReference>();
    private static Object _collectionLock = new Object();
    private bool _isConnected;

    #region Fields
    readonly bool _disableWhenBusy = true;
    readonly Action _execute;
    readonly Action<object> _executeWithParam;
    readonly Func<bool> _canExecute;
    readonly Func<object, bool> _canExecuteWithParamer;
    readonly Func<Task> _executeTask;

    readonly PropertyInfo _canExecuteProperty;
    readonly MethodInfo _executeMethod;
    readonly object _targetObject;

    #endregion // Fields

    #region Events

    /// <summary>
    /// Occurs when the CanExecute property changes.
    /// </summary>
    public event EventHandler CanExecuteChanged;

    #endregion

    #region Constructors

    internal RelayCommand(bool supportsOnUpdate = true)
    {
        if (supportsOnUpdate)
            AddToCache();
    }

    internal RelayCommand(Action execute, bool supportsOnUpdate, bool disableWhenBusy)
    {
        if (execute == null)
            throw new ArgumentNullException("execute");

        _execute = execute;
        _disableWhenBusy = disableWhenBusy;

        if (supportsOnUpdate)
            AddToCache();
    }

    /// <summary>
    /// Instantiates a new RelayCommand instance.
    /// </summary>
    /// <param name="execute">The method to call.</param>
    /// <param name="supportsOnUpdate">Is the command automatically called for a state update. Default is true.</param>
    /// <remarks>
    /// If supportsOnUpdate is true, the default, the command will automatically disable whenever the main worker thread is busy
    /// and when the application begins shutting down.
    /// </remarks>
    public RelayCommand(Action execute, bool supportsOnUpdate = true)
    {
        if (execute == null)
            throw new ArgumentNullException("execute");

        _execute = execute;

        if (supportsOnUpdate)
            AddToCache();
    }

    /// <summary>
    /// Instantiates a new RelayCommand instance.
    /// </summary>
    /// <param name="execute">The method to call.</param>
    /// <param name="supportsOnUpdate">Is the command automatically called for a state update. Default is true.</param>
    /// <remarks>
    /// If supportsOnUpdate is true, the default, the command will automatically disable whenever the main worker thread is busy
    /// and when the application begins shutting down.
    /// </remarks>
    public RelayCommand(Func<Task> execute, bool supportsOnUpdate = true)
    {
        if (execute == null)
            throw new ArgumentNullException("execute");

        _executeTask = execute;

        if (supportsOnUpdate)
            AddToCache();
    }

    /// <summary>
    /// Instantiates a new RelayCommand instance.
    /// </summary>
    /// <param name="canExecute">The method to call to update the state.</param>
    /// <param name="execute">The method to call.</param>
    /// <param name="supportsOnUpdate">Is the command automatically called for a state update. Default is true.</param>
    /// <param name="disableWhenBusy">Is the command automatically disabled when the main worker thread is busy. Default is true.</param>
    /// <remarks>
    /// Use this constructor to specify a specific enabling logic (canExecute) function for the command.
    /// </remarks>
    public RelayCommand(Action execute, Func<bool> canExecute, bool supportsOnUpdate = true, bool disableWhenBusy = true)
    {
        if (execute == null)
            throw new ArgumentNullException("execute");

        _execute = execute;
        _canExecute = canExecute;
        _disableWhenBusy = disableWhenBusy;

        if (supportsOnUpdate)
            AddToCache();
    }

    /// <summary>
    /// Instantiates a new RelayCommand instance.
    /// </summary>
    /// <param name="canExecute">The method to call to update the state.</param>
    /// <param name="execute">The method to call.</param>
    /// <param name="supportsOnUpdate">Is the command automatically called for a state update. Default is true.</param>
    /// <param name="disableWhenBusy">Is the command automatically disabled when the main worker thread is busy. Default is true.</param>
    public RelayCommand(Action<object> execute, Func<bool> canExecute, bool supportsOnUpdate = true, bool disableWhenBusy = true)
    {
        if (execute == null)
            throw new ArgumentNullException("execute");

        _executeWithParam = execute;
        _canExecute = canExecute;
        _disableWhenBusy = disableWhenBusy;

        if (supportsOnUpdate)
            AddToCache();
    }

    /// <summary>
    /// Instantiates a new RelayCommand instance.
    /// </summary>
    /// <param name="canExecute">The method to call to update the state.</param>
    /// <param name="execute">The method to call.</param>
    /// <param name="supportsOnUpdate">Is the command automatically called for a state update. Default is true.</param>
    /// <param name="disableWhenBusy">Is the command automatically disabled when the main worker thread is busy. Default is true.</param>
    public RelayCommand(Action<object> execute, Func<object, bool> canExecute, bool supportsOnUpdate = true, bool disableWhenBusy = true)
    {
        if (execute == null)
            throw new ArgumentNullException("execute");

        _executeWithParam = execute;
        _canExecuteWithParamer = canExecute;
        _disableWhenBusy = disableWhenBusy;

        if (supportsOnUpdate)
            AddToCache();
    }

    /// <summary>
    /// Instantiates a new RelayCommand instance.
    /// </summary>
    /// <param name="canExecute">The method to call to update the state.</param>
    /// <param name="execute">The method to call.</param>
    /// <param name="supportsOnUpdate">Is the command automatically called for a state update. Default is true.</param>
    /// <param name="disableWhenBusy">Is the command automatically disabled when the main worker thread is busy. Default is true.</param>
    public RelayCommand(Func<Task> execute, Func<bool> canExecute, bool supportsOnUpdate = true, bool disableWhenBusy = true)
    {
        if (execute == null)
            throw new ArgumentNullException("execute");

        _executeTask = execute;
        _canExecute = canExecute;
        _disableWhenBusy = disableWhenBusy;

        if (supportsOnUpdate)
            AddToCache();
    }

    /// <summary>
    /// Instantiates a new RelayCommand instance.
    /// </summary>
    /// <param name="targetObject">The object to call.</param>
    /// <param name="execute">The method on the target object to call.</param>
    /// <param name="canExecute">The method to call to update the state.</param>
    /// <param name="supportsOnUpdate">Is the command automatically called for a state update. Default is true.</param>
    public RelayCommand(object targetObject, MethodInfo execute, PropertyInfo canExecute, bool supportsOnUpdate = true)
    {
        _targetObject = targetObject;
        _executeMethod = execute;
        _canExecuteProperty = canExecute;

        if (supportsOnUpdate)
            AddToCache();
    }

    internal void AddToCache()
    {
        lock (_collectionLock)
        {
            if (!_isConnected)
            {
                this._isConnected = true;
                _relayCommands.Add(new WeakReference(this));
            }
        }
    }
    #endregion

    #region ICommand Members

    /// <summary>
    /// Gets whether the command can be executed.
    /// </summary>
    /// <param name="parameter">Parameter</param>
    /// <returns></returns>
    public virtual bool CanExecute(object parameter)
    {
        if (_disableWhenBusy)
            return false;

        // this is called on the GUI thread
        if (_canExecute != null)
            return _canExecute();
        if (_canExecuteWithParamer != null)
            return _canExecuteWithParamer(parameter);

        return true;
    }

    /// <summary>
    /// Raises the CanExecuteChanged event.
    /// </summary>
    //[SuppressMessage("Microsoft.Design", "CA1030")]
    //public void RaiseCanExecuteChanged()
    //{
    //  if (CanExecuteChanged != null)
    //    CanExecuteChanged(this, EventArgs.Empty);
    //  else if (this is LabelWrapper)
    //  {
    //    (this as LabelWrapper).Enabled = CanExecute(null);
    //  }
    //  else if (this is CommandWrapper)
    //    CanExecute(null);
    //  //else
    //  //  this.Disconnect();
    //}

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="parameter"></param>
    public virtual void Execute(object parameter)
    {
        if (_execute != null)
            _execute();
        else if (_executeTask != null)
            _executeTask();
        else if (_executeWithParam != null)
            _executeWithParam(parameter);
        else if (_executeMethod != null)
            _executeMethod.Invoke(_targetObject, new object[] { parameter });
    }

    #endregion

    /// <summary>
    /// Removes the command from the the OnUpdate message queue.
    /// </summary>
    /// <returns></returns>
    public void Disconnect()
    {
        WeakReference? targetItem = null;

        lock (_collectionLock)
        {
            foreach (var weakRef in _relayCommands)
            {
                if (this == weakRef.Target)
                {
                    targetItem = weakRef;
                    break;
                }
            }

            if (targetItem != null)
            {
                _relayCommands.Remove(targetItem);
                _isConnected = false;
            }
        }
    }

    /// <summary>
    /// Adds the command back to the the OnUpdate message queue.
    /// </summary>
    /// <returns></returns>
    public void Reconnect()
    {
        AddToCache();
    }

    internal static void RaiseAllCanExecuteChanged()
    {
        // Don't let any threads modify the relay commands collection simultaneously
        // Don't call any blind functions from within a lock - make a copy and call out on it

        List<RelayCommand> localCmdsCopy = new List<RelayCommand>();

        lock (_collectionLock)
        {
            WeakReference? targetItem = null;
            int count = _relayCommands.Count;

            for (int i = _relayCommands.Count - 1; i >= 0; i--)
            {
                targetItem = _relayCommands[i];
                if (!targetItem.IsAlive)
                {
                    _relayCommands.Remove(targetItem);
                    continue;
                }

                RelayCommand? relayCommand = targetItem.Target as RelayCommand;

                if (null != relayCommand)
                    localCmdsCopy.Add(relayCommand);
            }
        }

        foreach (RelayCommand cmd in localCmdsCopy)
        {
            //cmd.RaiseCanExecuteChanged();
        }

        localCmdsCopy = null;
    }

    internal static RelayCommand? GenerateCommand(object target, string methodName, bool supportOnUpdate = true)
    {
        if ((target == null) || string.IsNullOrEmpty(methodName))
            return null;

        // See if we have a simple Action (no parameters; no return type)
        Action action = (Action)Delegate.CreateDelegate(typeof(Action), target, methodName, true, false);

        Func<Task> taskFunction = null;

        // If Action failed, check if we have a Task function
        if (action == null)
            taskFunction = (Func<Task>)Delegate.CreateDelegate(typeof(Func<Task>), target, methodName, true, false);

        // Still nothing? Check via reflection.
        if (action == null && taskFunction == null)
            return GetCommand(target, methodName, supportOnUpdate);

        Func<bool> canExecuteFunction = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), target, "Can" + methodName, true, false);

        if (canExecuteFunction == null)
        {
            if (action != null)
                return new RelayCommand(action, supportOnUpdate);
            else if (taskFunction != null)
                return new RelayCommand(taskFunction, supportOnUpdate);
        }
        else
          if (action != null)
            return new RelayCommand(action, canExecuteFunction, supportOnUpdate);
        else if (taskFunction != null)
            return new RelayCommand(taskFunction, canExecuteFunction, supportOnUpdate);

        return null;
    }

    private static RelayCommand GetCommand(object target, string methodName, bool supportOnUpdate)
    {
        var targetType = target.GetType();
      
        var methods = targetType.GetMethods();
        var properties = targetType.GetProperties();

        foreach (var method in methods)
        {
            if (methodName != method.Name)
                continue;

            //var foundProperty = properties.FirstOrDefault(x => x.Name == "Can" + method.Name);
            //return new RelayCommand(target, method, foundProperty, supportOnUpdate);
        }

        throw new InvalidOperationException("Target method not found '" + methodName + "'.");
    }
}