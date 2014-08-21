// Copyright 2007-2013 Chris Patterson
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
    using System.Threading.Tasks;
    using Contracts;
    using InternalMessages;
    using Logging;


    public class ExecuteActivityHost<TActivity, TArguments> :
        Consumes<RoutingSlip>.Context
        where TActivity : ExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly ExecuteActivityFactory<TArguments> _activityFactory;
        readonly Uri _compensateAddress;
        readonly ILog _log = Logger.Get<ExecuteActivityHost<TActivity, TArguments>>();
        readonly HostImpl _host;

        public ExecuteActivityHost(Uri compensateAddress, ExecuteActivityFactory<TArguments> activityFactory)
        {
            if (compensateAddress == null)
                throw new ArgumentNullException("compensateAddress");
            if (activityFactory == null)
                throw new ArgumentNullException("activityFactory");

            _compensateAddress = compensateAddress;
            _activityFactory = activityFactory;
            _host = new HostImpl(null);
        }

        public ExecuteActivityHost(ExecuteActivityFactory<TArguments> activityFactory)
        {
            if (activityFactory == null)
                throw new ArgumentNullException("activityFactory");

            _activityFactory = activityFactory;
            _host = new HostImpl(null);
        }

        void Consumes<IConsumeContext<RoutingSlip>>.All.Consume(IConsumeContext<RoutingSlip> context)
        {
            var host = new ExecuteHost(_host, context.InputAddress);

            Execution<TArguments> execution = new HostExecution<TArguments>(host, _compensateAddress, context);

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Host: {0} Executing: {1}", execution.Bus.Endpoint.Address, execution.TrackingNumber);

            try
            {
                // TODO async MT support
                ExecutionResult result = ExecuteActivity(execution).Result;

                result.Evaluate().Wait();
            }
            catch (Exception ex)
            {
                _log.Error("The activity threw an unexpected exception", ex);
            }
        }

        async Task<ExecutionResult> ExecuteActivity(Execution<TArguments> execution)
        {
            try
            {
                return await _activityFactory.ExecuteActivity(execution);
            }
            catch (Exception ex)
            {
                return execution.Faulted(ex);
            }
        }


        class ExecuteHost :
            Host
        {
            public Uri Address { get; private set; }
            readonly Host _host;

            public string CourierVersion
            {
                get { return _host.CourierVersion; }
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

            public ExecuteHost(Host host, Uri address)
            {
                Address = address;
                _host = host;
            }
        }
    }
}