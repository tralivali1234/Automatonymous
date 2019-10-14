// Copyright 2011-2015 Chris Patterson, Dru Sellers
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Automatonymous
{
    using System.Threading;
    using System.Threading.Tasks;
    using Activities;
    using Behaviors;
    using Contexts;


    public static class StateMachineExtensions
    {
        /// <summary>
        ///     Transition a state machine instance to a specific state, producing any events related
        ///     to the transaction such as leaving the previous state and entering the target state
        /// </summary>
        /// <typeparam name="TInstance">The state instance type</typeparam>
        /// <param name="machine">The state machine</param>
        /// <param name="instance">The state instance</param>
        /// <param name="state">The target state</param>
        /// <param name="cancellationToken"></param>
        public static Task TransitionToState<TInstance>(this StateMachine<TInstance> machine, TInstance instance, State state,
            CancellationToken cancellationToken = default)
            where TInstance : class
        {
            StateAccessor<TInstance> accessor = machine.Accessor;
            State<TInstance> toState = machine.GetState(state.Name);

            Activity<TInstance> activity = new TransitionActivity<TInstance>(toState, accessor);
            Behavior<TInstance> behavior = new LastBehavior<TInstance>(activity);

            var eventContext = new StateMachineEventContext<TInstance>(machine, instance, toState.Enter, cancellationToken);

            BehaviorContext<TInstance> behaviorContext = new EventBehaviorContext<TInstance>(eventContext);

            return behavior.Execute(behaviorContext);
        }
    }
}