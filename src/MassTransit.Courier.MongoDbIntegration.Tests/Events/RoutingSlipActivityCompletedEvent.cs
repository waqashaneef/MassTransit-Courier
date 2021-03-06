﻿// Copyright 2007-2013 Chris Patterson
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
namespace MassTransit.Courier.MongoDbIntegration.Tests.Events
{
    using System;
    using System.Collections.Generic;
    using Contracts;


    class RoutingSlipActivityCompletedEvent :
        RoutingSlipActivityCompleted
    {
        public RoutingSlipActivityCompletedEvent(Guid trackingNumber, string activityName,
            Guid activityTrackingNumber, DateTime timestamp)
        {
            Timestamp = timestamp;
            TrackingNumber = trackingNumber;
            ActivityName = activityName;
            ActivityTrackingNumber = activityTrackingNumber;

            Variables = new Dictionary<string, object>
                {
                    {"Content", "Goodbye, cruel world."},
                };

            Results = new Dictionary<string, object>
                {
                    {"OriginalContent", "Hello, World!"}
                };

            Arguments = new Dictionary<string, object>();
        }

        public Guid TrackingNumber { get; private set; }
        public DateTime Timestamp { get; private set; }
        public Guid ActivityTrackingNumber { get; private set; }
        public string ActivityName { get; private set; }
        public IDictionary<string, object> Arguments { get; private set; }
        public IDictionary<string, object> Results { get; private set; }
        public IDictionary<string, object> Variables { get; private set; }
    }
}