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


    [Flags]
    public enum RoutingSlipEvents
    {
        All = 0,
        Completed = 0x0001,
        Faulted = 0x0002,
        CompensationFailed = 0x0004,
        ActivityCompleted = 0x0010,
        ActivityFaulted = 0x0020,
        ActivityCompensated = 0x0040,
        ActivityCompensationFailed = 0x0080,
    }
}