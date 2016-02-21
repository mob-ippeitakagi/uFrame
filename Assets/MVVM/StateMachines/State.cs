using System;
using System.Collections.Generic;
using UniRx;

namespace Invert.StateMachine
{
	public abstract class State : IObserver<StateMachineTrigger>
	{
		Dictionary<StateMachineTrigger, StateTransition> triggers;
		public Dictionary<StateMachineTrigger, StateTransition> Triggers
		{
			get { return this.triggers ?? (this.triggers = new Dictionary<StateMachineTrigger, StateTransition>()); }
			set { this.triggers = value; }
		}

		public StateMachine StateMachine { get; set; }
		public abstract string Name { get; }

		public void Transition(StateTransition transition)
		{
			this.StateMachine.Transition(transition);
		}

		public virtual void OnEnter(State previousState)
		{
			if (previousState != null && previousState != this)
				foreach (var trigger in this.Triggers)
					foreach (var computer in trigger.Key.Computers)
						trigger.Key.OnNext(computer());
		}
		public virtual void OnExit(State nextState)
		{
		}
		public void OnEntering(State currentState)
		{
		}

		public virtual void AddTrigger(StateMachineTrigger trigger, StateTransition transition)
		{
			this.Triggers.Add(trigger, transition);
		}

		public void Trigger(StateMachineTrigger transition)
		{
			this.OnNext(transition);
		}

		public void OnCompleted()
		{
		}
		public void OnError(Exception error)
		{
		}
		public void OnNext(StateMachineTrigger value)
		{
			if (this.Triggers.ContainsKey(value))
			{
				Transition(Triggers[value]);
			}
		}

		public override string ToString()
		{
			return Name ?? this.GetType().Name;
		}
	}
}
