/*
 *  f1livetiming - Part of the Live Timing Library for .NET
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
    /// <summary>
    /// Provides a common implementation of all the basic features of the state pattern. Inheriting
    /// from this class is optional (but it's easier).
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public abstract class State<TContext> : IState<TContext> 
        where TContext : IContext, new()
    {
        /// <summary>
        /// A reference to the current state machine. This 
        /// value is initialised by the state machine immediately after
        /// construction and before Entry().
        /// </summary>
        public IStateMachine<TContext> StateMachine { get; set; }

        /// <summary>
        /// A reference to the current state machine's context. This 
        /// value is initialised by the state machine immediately after
        /// construction and before Entry().
        /// </summary>
        public TContext Context { get; set; }

        /// <summary>
        /// When a new state is made current, the entry method is called
        /// to initialise the implementation of the state. This method
        /// is guaranteed to be called after the Exit() of the previous
        /// state.
        /// </summary>
        public virtual void Entry()
        {
        }

        /// <summary>
        /// When a new state is 
        /// </summary>
        public virtual void Exit()
        {
        }
        
        /// <summary>
        /// See IDispose
        /// </summary>
        public virtual void Dispose()
        {
        }
    }
}
