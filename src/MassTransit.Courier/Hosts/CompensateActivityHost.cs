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
    using Contracts;
    using InternalMessages;
    using Logging;


    public class CompensateActivityHost<TActivity, TLog> :
        Consumes<RoutingSlip>.Context
        where TActivity : CompensateActivity<TLog>
        where TLog : class
    {
        readonly CompensateActivityFactory<TLog> _activityFactory;
        readonly Host _host;
        readonly ILog _log = Logger.Get<CompensateActivityHost<TActivity, TLog>>();

        public CompensateActivityHost(CompensateActivityFactory<TLog> activityFactory)
        {
            _activityFactory = activityFactory;
            _host = new HostImpl(null);
        }

        void Consumes<IConsumeContext<RoutingSlip>>.All.Consume(IConsumeContext<RoutingSlip> context)
        {
            var host = new CompensateHost(_host, context.InputAddress);

            Compensation<TLog> compensation = new HostCompensation<TLog>(host, context);

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Host: {0} Compensating: {1}", host.Address, compensation.TrackingNumber);

            CompensationResult result;
            try
            {
                result = _activityFactory.CompensateActivity(compensation);
            }
            catch (Exception ex)
            {
                result = compensation.Failed(ex);
            }

            result.Evaluate();
        }


        class CompensateHost :
            Host
        {
            readonly Host _host;

            public CompensateHost(Host host, Uri address)
            {
                Address = address;
                _host = host;
            }

            public Uri Address { get; private set; }

            public string RoutingSlipVersion
            {
                get { return _host.RoutingSlipVersion; }
            }

            public string OsVersion
            {
                get { return _host.OsVersion; }
            }

            public string MassTransitVersion
            {
                get { return _host.MassTransitVersion; }
            }

            public string FrameworkVersion
            {
                get { return _host.FrameworkVersion; }
            }

            public string AssemblyVersion
            {
                get { return _host.AssemblyVersion; }
            }

            public string Assembly
            {
                get { return _host.Assembly; }
            }

            public int ProcessId
            {
                get { return _host.ProcessId; }
            }

            public string MachineName
            {
                get { return _host.MachineName; }
            }

            public string ProcessName
            {
                get { return _host.ProcessName; }
            }
        }
    }
}