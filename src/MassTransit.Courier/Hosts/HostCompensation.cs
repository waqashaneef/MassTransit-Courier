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
    using System.Diagnostics;
    using System.Linq;
    using Contracts;


    public class HostCompensation<TLog> :
        Compensation<TLog>
        where TLog : class
    {
        readonly ActivityLog _activityLog;
        readonly CompensateLog _compensateLog;
        readonly IConsumeContext<RoutingSlip> _context;
        readonly TLog _data;
        readonly Host _host;
        readonly SanitizedRoutingSlip _routingSlip;
        readonly Stopwatch _timer;
        readonly DateTime _startTimestamp;

        public HostCompensation(Host host, IConsumeContext<RoutingSlip> context)
        {
            _host = host;
            _context = context;

            _timer = Stopwatch.StartNew();
            _startTimestamp = DateTime.UtcNow;

            _routingSlip = new SanitizedRoutingSlip(context);
            if (_routingSlip.CompensateLogs.Count == 0)
                throw new ArgumentException("The routingSlip must contain at least one activity log");

            _compensateLog = _routingSlip.CompensateLogs.Last();

            _activityLog = _routingSlip.ActivityLogs.SingleOrDefault(x => x.ActivityTrackingNumber == _compensateLog.ActivityTrackingNumber);
            if (_activityLog == null)
            {
                throw new RoutingSlipException("The compensation log did not have a matching activity log entry: "
                                               + _compensateLog.ActivityTrackingNumber);
            }

            _data = _routingSlip.GetCompensateLogData<TLog>();
        }

        TLog Compensation<TLog>.Log
        {
            get { return _data; }
        }

        Guid Compensation<TLog>.TrackingNumber
        {
            get { return _routingSlip.TrackingNumber; }
        }

        IServiceBus Compensation<TLog>.Bus
        {
            get { return _context.Bus; }
        }

        public Host Host
        {
            get { return _host; }
        }

        public DateTime StartTimestamp
        {
            get { return _startTimestamp; }
        }

        public TimeSpan ElapsedTime
        {
            get { return _timer.Elapsed; }
        }

        public IConsumeContext ConsumeContext
        {
            get { return _context; }
        }

        public string ActivityName
        {
            get { return _activityLog.Name; }
        }

        public Guid ActivityTrackingNumber
        {
            get { return _activityLog.ActivityTrackingNumber; }
        }

        CompensationResult Compensation<TLog>.Compensated()
        {
            return new CompensatedCompensationResult<TLog>(this, _compensateLog, _routingSlip);
        }

        CompensationResult Compensation<TLog>.Compensated(object values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            return new CompensatedWithVariablesCompensationResult<TLog>(this, _compensateLog, _routingSlip,
                RoutingSlipBuilder.GetObjectAsDictionary(values));
        }

        CompensationResult Compensation<TLog>.Compensated(IDictionary<string, object> variables)
        {
            if (variables == null)
                throw new ArgumentNullException("variables");

            return new CompensatedWithVariablesCompensationResult<TLog>(this, _compensateLog, _routingSlip, variables);
        }

        CompensationResult Compensation<TLog>.Failed()
        {
            var exception = new RoutingSlipException("The routing slip compensation failed");

            return new FailedCompensationResult<TLog>(this, _compensateLog, _routingSlip, exception);
        }

        CompensationResult Compensation<TLog>.Failed(Exception exception)
        {
            return new FailedCompensationResult<TLog>(this, _compensateLog, _routingSlip, exception);
        }
    }
}