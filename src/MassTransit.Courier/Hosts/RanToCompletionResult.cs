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
    using InternalMessages;


    class RanToCompletionResult :
        ExecutionResult
    {
        readonly string _activityName;
        readonly Guid _activityTrackingNumber;
        readonly IDictionary<string, object> _arguments;
        readonly IServiceBus _bus;
        readonly IDictionary<string, object> _data;
        readonly TimeSpan _duration;
        readonly RoutingSlip _routingSlip;
        readonly DateTime _timestamp;
        readonly Host _host;

        public RanToCompletionResult(IServiceBus bus, RoutingSlip routingSlip, Guid activityTrackingNumber,
            string activityName, DateTime timestamp, TimeSpan duration, IDictionary<string, object> data,
            IDictionary<string, object> arguments, Host host)
        {
            _timestamp = timestamp;
            _routingSlip = routingSlip;
            _activityName = activityName;
            _activityTrackingNumber = activityTrackingNumber;
            _data = data;
            _arguments = arguments;
            _host = host;
            _duration = duration;
            _bus = bus;
        }

        public DateTime Timestamp
        {
            get { return _timestamp; }
        }

        public void Evaluate()
        {
            RoutingSlipActivityCompleted activityEvent = new RoutingSlipActivityCompletedMessage(_host, _routingSlip.TrackingNumber, _activityName,
                _activityTrackingNumber, _timestamp, _duration, _data, _routingSlip.Variables, _arguments);
            _bus.Publish(activityEvent);

            DateTime completedTimestamp = _timestamp + _duration;
            TimeSpan completedDuration = completedTimestamp - _routingSlip.CreateTimestamp;

            RoutingSlipCompleted completedEvent = new RoutingSlipCompletedMessage(_routingSlip.TrackingNumber, completedTimestamp,
                completedDuration, _routingSlip.Variables);
            _bus.Publish(completedEvent);
        }
    }
}