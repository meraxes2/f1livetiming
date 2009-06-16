/*
 *  f1livetiming - Part of the Live Timing Library for .NET
 *  Copyright (C) 2009 Liam Lowey
 *  
 *      http://livetiming.turnitin.co.uk/
 *
 *  Licensed under the Apache License, Version 2.0 (the "License"); 
 *  you may not use this file except in compliance with the License. 
 *  You may obtain a copy of the License at 
 *  
 *      http://www.apache.org/licenses/LICENSE-2.0 
 *  
 *  Unless required by applicable law or agreed to in writing, software 
 *  distributed under the License is distributed on an "AS IS" BASIS, 
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
 *  See the License for the specific language governing permissions and 
 *  limitations under the License. 
 */

namespace Common.Patterns.State
{
    public class StateMachine<TContext> : IStateMachine<TContext> where TContext : IContext, new()
    {
        protected IState<TContext> CurrentState { get; private set; }
        protected TContext Context { get; private set; }

        protected StateMachine()
        {
            Context = new TContext();
        }

        protected void InitialState<TInitialState>() where TInitialState : IState<TContext>, new()
        {
            CurrentState = new TInitialState();
            CurrentState.Entry(this, Context);
        }

        #region IStateMachine Members

        public void ChangeTo<TNewState>() where TNewState : IState<TContext>, new()
        {
            CurrentState.Exit();
            CurrentState.Dispose();

            CurrentState = new TNewState();
            CurrentState.Entry(this, Context);
        }


        public void Dispose()
        {
            CurrentState.Exit();
            CurrentState.Dispose();
            Context.Dispose();

            CurrentState = default(IState<TContext>);
            Context = default(TContext);
        }

        #endregion
    }
}