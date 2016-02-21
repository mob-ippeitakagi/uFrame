```cs
  // フィールド
  // プロパティ
  // コンストラクタ
  // メソッド
public class StateMachine : P<State>
  // フィールド
  List<State> _states;
  List<StateTransition> _transitions;
  // プロパティ
  public List<State> States
  public virtual State StartState
  public State LastState
  public State CurrentState
  public virtual List<StateTransition> Transitions
  public StateTransition LastTransition
  public string Identifier
  // コンストラクタ
  public StateMachine()
  public StateMachine(ViewModel owner, string propertyName) : base(owner, propertyName)
  // メソッド
  void Compose()
  protected override void OnPropertyChanged(string value)
  public virtual void Compose(List<State> states)
  public void Transition(string name)
  public void Transition(StateTransition transition)
  public void SetState<TState>() where TState : State
  public void SetState(string stateName)

public class P<T> : ISubject<T>, IObservableProperty, INotifyPropertyChanged
  // フィールド
  object _objectValue;
  object _lastValue;
  // プロパティ
  public IObservable<T> ChangedObservable
  public object LastValue
  public Func<T> Computer
  public T Value
  // コンストラクタ
  public P()
  public P(T value)
  public P(ViewModel owner, string propertyName)
  // メソッド
  public IDisposable Subscribe(IObserver<object> observer)
  protected virtual void OnPropertyChanged(string propertyName)
  public IDisposable ToComputed(Func<T> action, params IObservableProperty[] properties)
  // ISubject<T> -> IObserver<T>
  public void OnCompleted()
  public void OnError(Exception error)
  public void OnNext(T value)
  // ISubject<T> -> IObservable<T>
  public IDisposable Subscribe(IObserver<T> observer)
  // IObservableProperty
  public object ObjectValue
  public string PropertyName
  public ViewModel Owner
  public Type ValueType
  public IObservable<Unit> AsUnit
  public IDisposable SubscribeInternal(Action<object> propertyChanged)
  // INotifyPropertyChanged
  public event PropertyChangedEventHandler PropertyChanged;


public class StateMachineTrigger : IObserver<Unit>, IObserver<bool>
  // フィールド
  List<Func<bool>> _computers;
  // プロパティ
  public StateMachine StateMachine
  public string Name
  public List<Func<bool>> Computers
  // コンストラクタ
  public StateMachineTrigger(StateMachine stateMachine, string name)
  // メソッド
  public void AddComputer(P<bool> computed)
  // IObserver<T>
  public void OnCompleted()
  public void OnError(Exception error)
  public void OnNext(bool value)
  public void OnNext(Unit value)

public class StateTransition
  // フィールド
  public string Identifier;
  public State From;
  public State To;
  public string Name;
  // コンストラクタ
  public StateTransition(string name, State from, State to)

public abstract class State : IObserver<StateMachineTrigger>
  // フィールド
  Dictionary<StateMachineTrigger, StateTransition> _triggers;
  // プロパティ
  public abstract string Name
  public StateMachine StateMachine
  public Dictionary<StateMachineTrigger, StateTransition> Triggers
  // コンストラクタ
  // メソッド
  public void Transition(StateTransition transition)
  public override string ToString()
  public virtual void OnEnter(State previousState)
  public virtual void OnExit(State nextState)
  public void OnEntering(State currentState)
  public virtual void AddTrigger(StateMachineTrigger trigger,StateTransition transition)
  public void Trigger(StateMachineTrigger transition)
  // IObserver<T>
  public void OnCompleted()
  public void OnError(Exception error)
  public void OnNext(StateMachineTrigger value)

```
