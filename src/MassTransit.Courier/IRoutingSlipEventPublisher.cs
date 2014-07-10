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
    using Contracts;


    public interface IRoutingSlipEventPublisher
    {
        void Publish(IServiceBus bus, RoutingSlipCompleted message);
        void Publish(IServiceBus bus, RoutingSlipFaulted message);
        void Publish(IServiceBus bus, RoutingSlipCompensationFailed message);
        void Publish(IServiceBus bus, RoutingSlipActivityCompleted message);
        void Publish(IServiceBus bus, RoutingSlipActivityFaulted message);
        void Publish(IServiceBus bus, RoutingSlipActivityCompensated message);
        void Publish(IServiceBus bus, RoutingSlipActivityCompensationFailed message);
    }
}