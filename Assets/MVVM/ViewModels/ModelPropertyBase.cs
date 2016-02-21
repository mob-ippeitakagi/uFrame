using System;
using System.Collections.Generic;
using System.ComponentModel;
using UniRx;

namespace uFrame.MVVM
{
	public class P<T> : ISubject<T>, IObservableProperty, INotifyPropertyChanged
	{
		private object objectValue;
		private object lastValue;
		public object ObjectValue
		{
			get { return this.objectValue ?? default(T); }
			set
			{
				this.lastValue = this.objectValue;
				this.objectValue = value;
				OnPropertyChanged(PropertyName);
			}
		}
		public object LastValue
		{
			get { return this.lastValue; }
			set { this.lastValue = value; }
		}

		public IObservable<T> ChangedObservable
		{
			get { return this.Where(_ => this.ObjectValue != this.LastValue); }
		}

		public IObservable<Unit> AsUnit
		{
			get { return this.Select(_ => Unit.Default); }
		}

		public string PropertyName { get; set; }
		public Func<T> Computer { get; set; }

// #if !DLL
// 		public ViewModel Owner { get; set; }
// #endif

		public event PropertyChangedEventHandler PropertyChanged;

		public P()
		{
		}

		public P(T value)
		{
			this.objectValue = value;
		}

// #if !DLL
// 		public P(ViewModel owner, string propertyName)
// 		{
// 			Owner = owner;
// 			PropertyName = propertyName;
// 		}
// #endif




		public IDisposable SubscribeInternal(Action<object> propertyChanged)
		{
			return this.Subscribe(v => propertyChanged(v));
		}

		public IDisposable Subscribe(IObserver<object> observer)
		{
			PropertyChangedEventHandler evt = delegate { observer.OnNext(this.ObjectValue); };
			this.PropertyChanged += evt;
			var disposer = Disposable.Create(() => this.PropertyChanged -= evt);
			// if (Owner != null)
			// {
			// 	Owner.AddBinding(disposer);
			// }
			return disposer;
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = this.PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
			// if (Owner != null)
			// {
			// 	Owner.OnPropertyChanged(this, PropertyName);
			// }
		}

		public IDisposable ToComputed(Func<T> action, params IObservableProperty[] properties)
		{
			Computer = action;
			var disposables = new List<IDisposable>();
			foreach (var property in properties)
			{
				disposables.Add(property.SubscribeInternal(_ =>
				{
					OnNext(action());
				}));
			}

			//OnNext(action());

			return Disposable.Create(() =>
			{
				foreach (var d in disposables)
					d.Dispose();
			});
		}

		public Type ValueType
		{
			get { return typeof (T); }
		}

		public IDisposable Subscribe(IObserver<T> observer)
		{

			PropertyChangedEventHandler evt = delegate { observer.OnNext(Value); };
			PropertyChanged += evt;
			return Disposable.Create(() => PropertyChanged -= evt);
		}

		public T Value
		{
			get { return (T) ObjectValue; }
			set { ObjectValue = value; }
		}

		public void OnCompleted()
		{

		}

		public void OnError(Exception error)
		{

		}

		public void OnNext(T value)
		{
			ObjectValue = value;
		}
	}
}
