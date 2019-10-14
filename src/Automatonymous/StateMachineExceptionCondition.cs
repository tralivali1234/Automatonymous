// Copyright 2011-2019 Chris Patterson, Dru Sellers
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
// Copyright 2011-2019 Chris Patterson, Dru Sellers
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


    /// <summary>
    /// Filters activities based on the conditional statement
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TData"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate bool StateMachineExceptionCondition<in TInstance, in TData, in TException>(BehaviorExceptionContext<TInstance, TData, TException> context)
        where TException : Exception;


    /// <summary>
    /// Filters activities based on the conditional statement
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate bool StateMachineExceptionCondition<in TInstance, in TException>(BehaviorExceptionContext<TInstance, TException> context)
        where TException : Exception;
}