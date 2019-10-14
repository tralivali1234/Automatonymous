// Copyright 2011-2014 Chris Patterson, Dru Sellers
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
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public interface InstanceLift<out T>
        where T : StateMachine
    {
        Task Raise(Event @event, CancellationToken cancellationToken = default);

        Task Raise<TData>(Event<TData> @event, TData data, CancellationToken cancellationToken = default);

        Task Raise(Func<T, Event> eventSelector, CancellationToken cancellationToken = default);

        Task Raise<TData>(Func<T, Event<TData>> eventSelector, TData data, CancellationToken cancellationToken = default);
    }
}