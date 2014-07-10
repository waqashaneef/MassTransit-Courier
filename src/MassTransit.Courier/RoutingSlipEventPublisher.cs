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
namespace MassTransit.Courier
{
    using System.Linq;
    using Contracts;


    public class RoutingSlipEventPublisher :
        IRoutingSlipEventPublisher
    {
        readonly RoutingSlip _routingSlip;

        public RoutingSlipEventPublisher(RoutingSlip routingSlip)
        {
            _routingSlip = routingSlip;
        }

        public void Publish(IServiceBus bus, RoutingSlipCompleted message)
        {
            PublishEvent(bus, message, RoutingSlipEvents.Completed);
        }

        public void Publish(IServiceBus bus, RoutingSlipFaulted message)
        {
            PublishEvent(bus, message, RoutingSlipEvents.Faulted);
        }

        public void Publish(IServiceBus bus, RoutingSlipCompensationFailed message)
        {
            PublishEvent(bus, message, RoutingSlipEvents.CompensationFailed);
        }

        public void Publish(IServiceBus bus, RoutingSlipActivityCompleted message)
        {
            PublishEvent(bus, message, RoutingSlipEvents.ActivityCompleted);
        }

        public void Publish(IServiceBus bus, RoutingSlipActivityFaulted message)
        {
            PublishEvent(bus, message, RoutingSlipEvents.ActivityFaulted);
        }

        public void Publish(IServiceBus bus, RoutingSlipActivityCompensated message)
        {
            PublishEvent(bus, message, RoutingSlipEvents.ActivityCompensated);
        }

        public void Publish(IServiceBus bus, RoutingSlipActivityCompensationFailed message)
        {
            PublishEvent(bus, message, RoutingSlipEvents.ActivityCompensationFailed);
        }

        void PublishEvent<T>(IServiceBus bus, T message, RoutingSlipEvents eventFlag)
            where T : class
        {
            if (_routingSlip.Subscriptions.Any())
            {
                foreach (Subscription subscription in _routingSlip.Subscriptions)
                {
                    if (subscription.Events == RoutingSlipEvents.All || subscription.Events.HasFlag(eventFlag))
                    {
                        IEndpoint endpoint = bus.GetEndpoint(subscription.Address);
                        endpoint.Send(message);
                    }
                }
            }
            else
                bus.Publish(message);
        }
    }
}