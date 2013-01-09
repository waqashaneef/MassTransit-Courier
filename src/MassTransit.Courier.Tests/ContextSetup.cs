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
namespace MassTransit.Courier.Tests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Log4NetIntegration.Logging;
    using Logging;
    using NUnit.Framework;
    using log4net;
    using log4net.Config;


    [SetUpFixture]
    public class ContextSetup
    {
        [SetUp]
        public void Before_any()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;

            string file = Path.Combine(path, "masstransit.courier.tests.log4net.xml");

            XmlConfigurator.Configure(new FileInfo(file));

            Trace.WriteLine("Loading Log4net: " + file);

            Logger.UseLogger(new Log4NetLogger());
        }

        [TearDown]
        public void After_all()
        {
            LogManager.Shutdown();
        }
    }
}