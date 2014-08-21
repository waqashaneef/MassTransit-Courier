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
namespace MassTransit.Courier.MongoDbIntegration.Documents
{
    using System;
    using Contracts;


    public class HostDocument
    {
        public HostDocument(Host host)
        {
            MachineName = host.MachineName;
            ProcessName = host.ProcessName;
            ProcessId = host.ProcessId;
            Assembly = host.Assembly;
            AssemblyVersion = host.AssemblyVersion;
            FrameworkVersion = host.FrameworkVersion;
            MassTransitVersion = host.MassTransitVersion;
            OsVersion = host.OsVersion;
            Address = host.Address;
            RoutingSlipVersion = host.CourierVersion;
        }

        public string MachineName { get; set; }
        public string ProcessName { get; set; }
        public int ProcessId { get; set; }
        public string Assembly { get; set; }
        public string AssemblyVersion { get; set; }
        public string FrameworkVersion { get; set; }
        public string MassTransitVersion { get; set; }
        public string OsVersion { get; set; }
        public Uri Address { get; set; }
        public string RoutingSlipVersion { get; set; }
    }
}