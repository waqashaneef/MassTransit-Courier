// Copyright 2007-2013 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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


    public class ExecuteActivityHost<TController, TArguments> :
        Consumes<RoutingSlip>.Context
        where TController : ExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly Uri _compensateAddress;
        readonly Func<TArguments, TController> _controllerFactory;

        public ExecuteActivityHost(Uri compensateAddress, Func<TArguments, TController> controllerFactory)
        {
            if (compensateAddress == null)
                throw new ArgumentNullException("compensateAddress");
            if (controllerFactory == null)
                throw new ArgumentNullException("controllerFactory");

            _compensateAddress = compensateAddress;
            _controllerFactory = controllerFactory;
        }

        public void Consume(IConsumeContext<RoutingSlip> context)
        {
            var execution = new HostExecution<TArguments>(context, _compensateAddress);

            try
            {
                TController controller = _controllerFactory(execution.Arguments);

                ExecutionResult result = controller.Execute(execution);
            }
            catch (Exception ex)
            {
                execution.Faulted(ex);
            }
        }
    }
}