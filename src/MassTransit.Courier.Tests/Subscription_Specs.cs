﻿// Copyright 2007-2014 Chris Patterson
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
    using Contracts;
    using Hosts;
    using MassTransit.Testing;
    using NUnit.Framework;
    using Testing;


    [TestFixture]
    public class Adding_a_subscription_to_the_routing_slip
    {
        [Test]
        public void Should_send_the_completed_event()
        {
            Assert.IsTrue(_test.Sent.Any<RoutingSlipCompleted>());
        }

        [Test]
        public void Should_not_publish_the_completed_event()
        {
            Assert.IsFalse(_test.Published.Any<RoutingSlipCompleted>());
        }

        [Test]
        public void Should_register_the_proper_consumer()
        {
            Assert.IsNotEmpty(_test.Scenario.InputBus.HasSubscription<RoutingSlip>(),
                "Consumer subscription not registered");
        }

        ConsumerTest<BusTestScenario, ExecuteActivityHost<TestActivity, TestArguments>> _test;

        [TestFixtureSetUp]
        public void Setup()
        {
            _test = TestFactory.ForConsumer<ExecuteActivityHost<TestActivity, TestArguments>>()
                .InSingleBusScenario()
                .New(x =>
                {
                    x.ConstructUsing(() =>
                    {
                        var compensateAddress = new Uri("loopback://localhost/mt_server");

                        return new ExecuteActivityHost<TestActivity, TestArguments>(compensateAddress,
                            new FactoryMethodExecuteActivityFactory<TestActivity, TestArguments>(_ => new TestActivity()));
                    });

                    var builder = new RoutingSlipBuilder(Guid.NewGuid());
                    builder.AddActivity("test", new Uri("loopback://localhost/mt_client"), new
                    {
                        Value = "Hello",
                    });
                    builder.AddSubscription(new Uri("loopback://localhost/mt_client"), RoutingSlipEvents.Completed | RoutingSlipEvents.Faulted);

                    var routingSlip = builder.Build();

                    string jsonString = routingSlip.ToJsonString();

                    Console.WriteLine(jsonString);

                    x.Send(routingSlip);
                });

            _test.Execute();
        }

        [TestFixtureTearDown]
        public void Teardown()
        {
            _test.Dispose();
        }


        [Test]
        public void Should_serialize_and_deserialize_properly()
        {
            var builder = new RoutingSlipBuilder(Guid.NewGuid());
            builder.AddActivity("a", new Uri("loopback://locahost/execute_a"));

            builder.AddSubscription(new Uri("loopback://localhost/events"), RoutingSlipEvents.Completed | RoutingSlipEvents.Faulted);

            RoutingSlip routingSlip = builder.Build();

            string jsonString = routingSlip.ToJsonString();

            RoutingSlip loaded = RoutingSlipExtensions.GetRoutingSlip(jsonString);

            Assert.AreEqual(RoutingSlipEvents.Completed | RoutingSlipEvents.Faulted, loaded.Subscriptions[0].Events);
        }
    }
}