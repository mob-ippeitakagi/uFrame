using System;
using System.Collections.Generic;
using System.Linq;
using uFrame.MVVM;
using UniRx;
using UnityEngine;

namespace Invert.StateMachine
{
	public class StateMachine : P<State>
	{
		// State系
		List<State> states;
		public List<State> States
		{
			get { return this.states; }
			set { this.states = value; }
		}
		public virtual State StartState
		{
			get { return this.States.FirstOrDefault(); }
		}
		public State LastState
		{
			get { return this.LastValue as State; }
		}
		public State CurrentState
		{
			get { return this.Value; }
			set { this.Value = value; }
		}

		// StateTransition系
		List<StateTransition> transitions;
		public virtual List<StateTransition> Transitions
		{
			get { return this.transitions ?? (this.transitions = new List<StateTransition>()); }
			set { this.transitions = value; }
		}
		public StateTransition LastTransition { get; set; }

		// 何に使うんだろう？uFrameの処理かな？
		public string Identifier { get; private set; }

		public StateMachine()
		{
		}
		// public StateMachine(ViewModel owner, string propertyName) : base(owner, propertyName)
		public StateMachine(string propertyName)
		{
			this.Compose();
		}
		void Compose()
		{
			this.states = new List<State>();
			this.states.Clear();
			this.Compose(this.states);
			this.CurrentState = this.StartState;
		}
		public virtual void Compose(List<State> states)
		{
			this.Transitions.Clear();
		}

		protected override void OnPropertyChanged(string value)
		{
			if (this.LastValue != null)
				this.LastState.OnExit(this.CurrentState);

			base.OnPropertyChanged(value);

			if (this.Value != null)
				this.Value.OnEnter(this.LastValue as State);
		}

		public void Transition(string name)
		{
			StateTransition transition = null;
			foreach (var t in this.Transitions)
			{
				if (t.From == CurrentState && t.Name == name)
				{
					transition = t;
					break;
				}
			}
			if (transition != null)
			{
				this.Transition(transition);
			}
		}
		public void Transition(StateTransition transition)
		{
			if (transition == null) return;
			if (transition.From == CurrentState)
			{
				this.LastTransition = transition;
				this.CurrentState = transition.To;
			}
		}

		public void SetState<TState>() where TState : State
		{
			var state = this.States.OfType<TState>().FirstOrDefault();
			CurrentState = state;
		}
		public void SetState(string stateName)
		{
			var state = this.States.FirstOrDefault(p => p.Name == stateName);
			if (state != null)
			{
				CurrentState = state;
			}
		}
	}


	public class StateMachineTrigger : IObserver<Unit>, IObserver<bool>
	{
		List<Func<bool>> computers;
		public List<Func<bool>> Computers
		{
			get { return this.computers ?? (this.computers = new List<Func<bool>>()); }
			set { this.computers = value; }
		}

		public StateMachine StateMachine { get; set; }
		public string Name { get; set; }

		public StateMachineTrigger(StateMachine stateMachine, string name)
		{
			this.StateMachine = stateMachine;
			this.Name = name;
		}

		public void AddComputer(P<bool> computed)
		{
			computed.Subscribe(this);
			this.Computers.Add(computed.Computer);
		}

		public void OnCompleted()
		{
		}
		public void OnError(Exception error)
		{
		}
		public void OnNext(bool value)
		{
			if (value)
				this.StateMachine.CurrentState.Trigger(this);
		}
		public void OnNext(Unit value)
		{
			this.StateMachine.CurrentState.Trigger(this);
		}
	}
}

