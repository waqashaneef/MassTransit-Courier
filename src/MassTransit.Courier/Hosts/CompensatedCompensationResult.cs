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
    using System.Threading.Tasks;
    using Contracts;
    using Extensions;
    using InternalMessages;


    class CompensatedCompensationResult<TLog> :
        CompensationResult
        where TLog : class
    {
        readonly CompensateLog _compensateLog;
        readonly Compensation<TLog> _compensation;
        readonly TimeSpan _duration;
        readonly RoutingSlip _routingSlip;

        public CompensatedCompensationResult(Compensation<TLog> compensation, CompensateLog compensateLog, RoutingSlip routingSlip)
        {
            _compensation = compensation;
            _compensateLog = compensateLog;
            _routingSlip = routingSlip;
            _duration = _compensation.ElapsedTime;
        }

        public async Task Evaluate()
        {
            RoutingSlipBuilder builder = CreateRoutingSlipBuilder(_routingSlip);

            Build(builder);

            RoutingSlip routingSlip = builder.Build();

            IRoutingSlipEventPublisher publisher = new RoutingSlipEventPublisher(routingSlip);

            RoutingSlipActivityCompensated activityCompensated = new RoutingSlipActivityCompensatedMessage(_compensation.Host,
                _compensation.TrackingNumber, _compensation.ActivityName, _compensation.ActivityTrackingNumber, _compensation.StartTimestamp,
                _duration, _compensateLog.Data, _routingSlip.Variables);
            publisher.Publish(_compensation.Bus, activityCompensated);

            if (HasMoreCompensations(routingSlip))
            {

                IEndpoint endpoint = _compensation.Bus.GetEndpoint(routingSlip.GetNextCompensateAddress());
                endpoint.Forward(_compensation.ConsumeContext, routingSlip);
            }
            else
            {
                DateTime faultedTimestamp = _compensation.StartTimestamp + _duration;
                TimeSpan faultedDuration = faultedTimestamp - _routingSlip.CreateTimestamp;

                RoutingSlipFaulted routingSlipFaulted = new RoutingSlipFaultedMessage(_compensation.TrackingNumber, faultedTimestamp,
                    faultedDuration, _routingSlip.ActivityExceptions, _routingSlip.Variables);
                publisher.Publish(_compensation.Bus, routingSlipFaulted);
            }
        }

        bool HasMoreCompensations(RoutingSlip routingSlip)
        {
            return routingSlip.CompensateLogs != null && routingSlip.CompensateLogs.Count > 0;
        }

        protected virtual void Build(RoutingSlipBuilder builder)
        {
        }

        protected virtual RoutingSlipBuilder CreateRoutingSlipBuilder(RoutingSlip routingSlip)
        {
            return new RoutingSlipBuilder(routingSlip, (IEnumerable<CompensateLog> x) => x.SkipLast());
        }
    }
}