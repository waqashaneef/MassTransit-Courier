// Copyright 2007-2014 Chris Patterson
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
namespace MassTransit.Courier.Hosts
{
    using System;
    using System.Collections.Generic;
    using Contracts;


    class NextActivityWithVariablesExecutionResult<TArguments> :
        CompletedExecutionResult<TArguments>
        where TArguments : class
    {
        readonly IDictionary<string, object> _variables;

        public NextActivityWithVariablesExecutionResult(Execution<TArguments> execution, Activity activity, RoutingSlip routingSlip,
            IDictionary<string, object> variables)
            : base(execution, activity, routingSlip)
        {
            _variables = variables;
        }

        protected override void Build(RoutingSlipBuilder builder)
        {
            base.Build(builder);

            builder.SetVariables(_variables);
        }
    }


    class NextActivityWithVariablesExecutionResult<TArguments, TLog> :
        CompletedExecutionResult<TArguments>
        where TArguments : class
        where TLog : class
    {
        readonly Uri _compensationAddress;
        readonly IDictionary<string, object> _variables;

        public NextActivityWithVariablesExecutionResult(Execution<TArguments> execution, Activity activity, RoutingSlip routingSlip,
            Uri compensationAddress, TLog log, IDictionary<string, object> variables)
            : base(execution, activity, routingSlip, RoutingSlipBuilder.GetObjectAsDictionary(log))
        {
            _compensationAddress = compensationAddress;
            _variables = variables;
        }

        protected override void Build(RoutingSlipBuilder builder)
        {
            base.Build(builder);

            builder.AddCompensateLog(Execution.ActivityTrackingNumber, _compensationAddress, Data);
            builder.SetVariables(_variables);
        }
    }
}