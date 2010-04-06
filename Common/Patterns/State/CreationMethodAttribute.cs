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
