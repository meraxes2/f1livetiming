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

using System;

namespace Common.Patterns.State
{
    /// <summary>
    /// Defines the creation behaviour of the state machine.
    /// </summary>
    public enum CreationMethod
    {
        /// <summary>
        /// A new instance should be created for every state change to this state.
        /// </summary>
        Dynamic = 0,

        /// <summary>
        /// Akin to a singleton, only one instance will ever be needed per application. This
        /// is possibly more efficient for states which never store private information. This
        /// pattern is NOT thread safe because it relies on BasicSingleton pattern.
        /// </summary>
        OnePerApplication = 1,

        /// <summary>
        /// Similar to OnePerApplication, but if you have more than one state machine this can
        /// be used to ensure only one instance per state machine is available by using some 
        /// local storage in the FSM. This pattern is thread safe.
        /// </summary>
        OnePerStateMachine = 2
    }

    /// <summary>
    /// Associate this attribute with each of your states to change the behaviour for constructing
    /// instances of your state. See CreationMethod.
    /// </summary>
    public class CreationMethodAttribute : Attribute
    {
        public CreationMethod CreationMethod { get; private set; }

        public CreationMethodAttribute( CreationMethod creationMethod )
        {
            CreationMethod = creationMethod;
        }
    }
}
