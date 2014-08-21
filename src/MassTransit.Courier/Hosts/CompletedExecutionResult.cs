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
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using InternalMessages;


    abstract class CompletedExecutionResult<TArguments> :
        ExecutionResult
        where TArguments : class
    {
        readonly Activity _activity;
        readonly IDictionary<string, object> _data;
        readonly TimeSpan _duration;
        readonly Execution<TArguments> _execution;
        readonly RoutingSlip _routingSlip;

        protected CompletedExecutionResult(Execution<TArguments> execution, Activity activity, RoutingSlip routingSlip)
            : this(execution, activity, routingSlip, RoutingSlipBuilder.NoArguments)
        {
        }

        protected CompletedExecutionResult(Execution<TArguments> execution, Activity activity, RoutingSlip routingSlip,
            IDictionary<string, object> data)
        {
            _execution = execution;
            _activity = activity;
            _routingSlip = routingSlip;
            _data = data;
            _duration = _execution.Elapsed;
        }

        protected IDictionary<string, object> Data
        {
            get { return _data; }
        }

        protected Execution<TArguments> Execution
        {
            get { return _execution; }
        }

        protected Activity Activity
        {
            get { return _activity; }
        }

        protected TimeSpan Duration
        {
            get { return _duration; }
        }

        public async Task Evaluate()
        {
            RoutingSlipBuilder builder = CreateRoutingSlipBuilder(_routingSlip);

            Build(builder);

            RoutingSlip routingSlip = builder.Build();

            IRoutingSlipEventPublisher publisher = new RoutingSlipEventPublisher(routingSlip);

            RoutingSlipActivityCompleted message = new RoutingSlipActivityCompletedMessage(_execution.Host, _execution.TrackingNumber,
                _execution.ActivityName, _execution.ActivityTrackingNumber, _execution.Timestamp, _duration, _data,
                routingSlip.Variables, _activity.Arguments);

            publisher.Publish(_execution.Bus, message);

            if (HasNextActivity(routingSlip))
            {
                IEndpoint endpoint = _execution.Bus.GetEndpoint(routingSlip.GetNextExecuteAddress());
                endpoint.Forward(_execution.ConsumeContext, routingSlip);
            }
            else
            {
                DateTime completedTimestamp = _execution.Timestamp + _duration;
                TimeSpan completedDuration = completedTimestamp - _routingSlip.CreateTimestamp;

                RoutingSlipCompleted completedEvent = new RoutingSlipCompletedMessage(_routingSlip.TrackingNumber, completedTimestamp,
                    completedDuration, routingSlip.Variables);
                publisher.Publish(_execution.Bus, completedEvent);
            }
        }

        static bool HasNextActivity(RoutingSlip routingSlip)
        {
            return routingSlip.Itinerary.Any();
        }

        protected virtual void Build(RoutingSlipBuilder builder)
        {
            builder.AddActivityLog(Execution.Host, Activity.Name, Execution.ActivityTrackingNumber, Execution.Timestamp, Duration);
        }

        protected virtual RoutingSlipBuilder CreateRoutingSlipBuilder(RoutingSlip routingSlip)
        {
            return new RoutingSlipBuilder(routingSlip, (IEnumerable<Activity> x) => x.Skip(1));
        }
    }
}