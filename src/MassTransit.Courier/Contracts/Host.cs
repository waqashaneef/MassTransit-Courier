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
    /// The host where the activity, compensation, or exception occurred while processing
    /// a routing slip
    /// </summary>
    public interface Host
    {
        /// <summary>
        /// The machine name (or role instance name) of the local machine
        /// </summary>
        string MachineName { get; }

        /// <summary>
        /// The process name hosting the routing slip activity
        /// </summary>
        string ProcessName { get; }

        /// <summary>
        /// The processId of the hosting process
        /// </summary>
        int ProcessId { get; }

        /// <summary>
        /// The assembly containing the activity
        /// </summary>
        string Assembly { get; }

        /// <summary>
        /// The assembly version of the assembly containing the activity
        /// </summary>
        string AssemblyVersion { get; }

        /// <summary>
        /// The .NET framework version
        /// </summary>
        string FrameworkVersion { get; }

        /// <summary>
        /// The version of MassTransit used by the process
        /// </summary>
        string MassTransitVersion { get; }

        /// <summary>
        /// The operating system version hosting the application
        /// </summary>
        string OsVersion { get; }

        /// <summary>
        /// The input address of the queue for the execution or compensation of the activity
        /// </summary>
        Uri Address { get; }

        /// <summary>
        /// The MassTransit courier version used to process the routing slipS
        /// </summary>
        string CourierVersion { get; }
    }
}