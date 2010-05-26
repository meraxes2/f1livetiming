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

using System.Collections.Generic;
using Common.Patterns.Singleton;

namespace Common.Patterns.State
{
    public class StateMachine<TContext, TContract> : IStateMachine<TContext> where TContext : IContext, new()
    {
        private Dictionary<string, IState<TContext>> _localStateCache = new Dictionary<string, IState<TContext>>();

        protected IState<TContext> CurrentState { get; private set; }

        protected TContext Context { get; private set; }

        protected TContract TypedState
        {
            get
            {
                return (TContract)CurrentState;
            }
        }

        protected StateMachine()
        {
            Context = new TContext();
        }

        protected void InitialState<TInitialState>() where TInitialState : IState<TContext>, new()
        {
            CurrentState = CreateState<TInitialState>();
            CurrentState.StateMachine = this;
            CurrentState.Context = Context;            
            CurrentState.Entry();
        }

        #region IStateMachine Members

        public void ChangeTo<TNewState>() where TNewState : IState<TContext>, new()
        {
            CurrentState.Exit();
            CurrentState.Dispose();

            CurrentState = CreateState<TNewState>();
            CurrentState.StateMachine = this;
            CurrentState.Context = Context;
            CurrentState.Entry();
        }


        public void Dispose()
        {
            CurrentState.Exit();
            CurrentState.Dispose();
            Context.Dispose();

            CurrentState = default(IState<TContext>);
            Context = default(TContext);

            _localStateCache = null;
        }

        #endregion

        #region State Factory Method

        private TNewState CreateState<TNewState>() where TNewState : IState<TContext>, new()
        {
            object [] attrs = typeof (TNewState).GetCustomAttributes(typeof (CreationMethodAttribute), false);

            if (attrs != null && attrs.Length > 0)
            {
                foreach (object nextAttr in attrs)
                {
                    // process the first creation method attribute we find as there should only be one
                    if( nextAttr is CreationMethodAttribute )
                    {
                        CreationMethodAttribute creationMethod = (CreationMethodAttribute)nextAttr;

                        switch (creationMethod.CreationMethod)
                        {
                            case CreationMethod.Dynamic:
                                return new TNewState();
                            case CreationMethod.OnePerApplication:
                                return BasicSingleton<TNewState>.Instance;
                            case CreationMethod.OnePerStateMachine:
                                string key = typeof (TNewState).FullName;
                                if (_localStateCache.ContainsKey(key))
                                {
                                    return (TNewState) _localStateCache[key];
                                }
                                TNewState ret = new TNewState();
                                _localStateCache[key] = ret;
                                return ret;
                        }
                    }
                }
            }
            
            // default, i.e. no attributes
            return new TNewState();
        }

        #endregion
    }
}