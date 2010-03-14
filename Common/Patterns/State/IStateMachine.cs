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

using System;

namespace Common.Patterns.State
{
    /// <summary>
    /// Defines the principal interface of the StateMachine pattern.
    /// </summary>
    /// <typeparam name="TContext">The concrete IContext type this state machine will use.</typeparam>
    /// <typeparam name="TContract">The contract / interface to define the additional behaviours of this State Machine.</typeparam>
    public interface IStateMachine<TContext> : IDisposable where TContext : IContext
    {
        void ChangeTo<TNewState>() where TNewState : IState<TContext>, new();

    }
}
