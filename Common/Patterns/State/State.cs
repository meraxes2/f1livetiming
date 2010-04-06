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
