// Copyright 2011-2016 Chris Patterson, Dru Sellers
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
namespace Automatonymous.Binders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class TriggerEventActivityBinder<TInstance> :
        EventActivityBinder<TInstance>
        where TInstance : class
    {
        readonly ActivityBinder<TInstance>[] _activities;
        readonly Event _event;
        readonly StateMachineEventFilter<TInstance> _filter;
        readonly StateMachine<TInstance> _machine;

        public TriggerEventActivityBinder(StateMachine<TInstance> machine, Event @event, params ActivityBinder<TInstance>[] activities)
        {
            _event = @event;
            _machine = machine;
            _activities = activities ?? new ActivityBinder<TInstance>[0];
        }

        public TriggerEventActivityBinder(StateMachine<TInstance> machine, Event @event, StateMachineEventFilter<TInstance> filter,
            params ActivityBinder<TInstance>[] activities)
        {
            _event = @event;
            _filter = filter;
            _machine = machine;
            _activities = activities ?? new ActivityBinder<TInstance>[0];
        }

        TriggerEventActivityBinder(StateMachine<TInstance> machine, Event @event, StateMachineEventFilter<TInstance> filter,
            ActivityBinder<TInstance>[] activities,
            params ActivityBinder<TInstance>[] appendActivity)
        {
            _event = @event;
            _filter = filter;
            _machine = machine;

            _activities = new ActivityBinder<TInstance>[activities.Length + appendActivity.Length];
            Array.Copy(activities, 0, _activities, 0, activities.Length);
            Array.Copy(appendActivity, 0, _activities, activities.Length, appendActivity.Length);
        }

        Event EventActivityBinder<TInstance>.Event => _event;

        EventActivityBinder<TInstance> EventActivityBinder<TInstance>.Add(Activity<TInstance> activity)
        {
            ActivityBinder<TInstance> activityBinder = new ExecuteActivityBinder<TInstance>(_event, activity);

            return new TriggerEventActivityBinder<TInstance>(_machine, _event, _filter, _activities, activityBinder);
        }

        EventActivityBinder<TInstance> EventActivityBinder<TInstance>.Catch<T>(
            Func<ExceptionActivityBinder<TInstance, T>, ExceptionActivityBinder<TInstance, T>> activityCallback)
        {
            ExceptionActivityBinder<TInstance, T> binder = new CatchExceptionActivityBinder<TInstance, T>(_machine, _event);

            binder = activityCallback(binder);

            ActivityBinder<TInstance> activityBinder = new CatchActivityBinder<TInstance, T>(_event, binder);

            return new TriggerEventActivityBinder<TInstance>(_machine, _event, _filter, _activities, activityBinder);
        }

        EventActivityBinder<TInstance> EventActivityBinder<TInstance>.If(StateMachineCondition<TInstance> condition,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            return IfElse(condition, activityCallback, _ => _);
        }

        EventActivityBinder<TInstance> EventActivityBinder<TInstance>.IfAsync(StateMachineAsyncCondition<TInstance> condition,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            return IfElseAsync(condition, activityCallback, _ => _);
        }

        public EventActivityBinder<TInstance> IfElse(StateMachineCondition<TInstance> condition,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> thenActivityCallback,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> elseActivityCallback)
        {
            var thenBinder = GetBinder(thenActivityCallback);
            var elseBinder = GetBinder(elseActivityCallback);

            var conditionBinder = new ConditionalActivityBinder<TInstance>(_event, condition, thenBinder, elseBinder);

            return new TriggerEventActivityBinder<TInstance>(_machine, _event, _filter, _activities, conditionBinder);
        }

        public EventActivityBinder<TInstance> IfElseAsync(StateMachineAsyncCondition<TInstance> condition,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> thenActivityCallback,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> elseActivityCallback)
        {
            var thenBinder = GetBinder(thenActivityCallback);
            var elseBinder = GetBinder(elseActivityCallback);

            var conditionBinder = new ConditionalActivityBinder<TInstance>(_event, condition, thenBinder, elseBinder);

            return new TriggerEventActivityBinder<TInstance>(_machine, _event, _filter, _activities, conditionBinder);
        }

        private EventActivityBinder<TInstance> GetBinder(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            EventActivityBinder<TInstance> binder = new TriggerEventActivityBinder<TInstance>(_machine, _event);
            return activityCallback(binder);
        }

        StateMachine<TInstance> EventActivityBinder<TInstance>.StateMachine => _machine;

        public IEnumerable<ActivityBinder<TInstance>> GetStateActivityBinders()
        {
            if (_filter != null)
                return Enumerable.Repeat(CreateConditionalActivityBinder(), 1);

            return _activities;
        }

        ActivityBinder<TInstance> CreateConditionalActivityBinder()
        {
            EventActivityBinder<TInstance> thenBinder = new TriggerEventActivityBinder<TInstance>(_machine, _event, _activities);
            EventActivityBinder<TInstance> elseBinder = new TriggerEventActivityBinder<TInstance>(_machine, _event);

            var conditionBinder = new ConditionalActivityBinder<TInstance>(_event, context => _filter(context), thenBinder, elseBinder);

            return conditionBinder;
        }
    }
}