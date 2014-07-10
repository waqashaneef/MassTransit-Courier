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
namespace MassTransit.Courier.Contracts
{
    using System;


    /// <summary>
    /// A routing slip subscription defines a specific endpoint where routing
    /// slip events should be sent (not published). If specified, events are not published.
    /// </summary>
    public interface Subscription
    {
        /// <summary>
        /// The address where events should be sent
        /// </summary>
        Uri Address { get; }

        /// <summary>
        /// The events that are subscribed
        /// </summary>
        RoutingSlipEvents Events { get; }
    }


    public interface SubscriptionEvent
    {
        string Name { get; }

        EventContents Include { get; }
    }


    [Flags]
    public enum EventContents
    {
        Variables = 0x0001,
        Arguments = 0x0002,
        Data = 0x0004,
        Encrypted = 0x0008,
        All = Variables | Arguments | Data,
    }

    [Flags]
    public enum RoutingSlipEvents
    {
        Completed = 0x0001,
        Faulted = 0x0002,
        CompensationFailed = 0x0004,
        ActivityCompleted = 0x0010,
        ActivityFaulted = 0x0020,
        ActivityCompensationFailed = 0x0040,
    }
}