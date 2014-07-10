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
namespace MassTransit.Courier
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;
    using InternalMessages;
    using Magnum.Reflection;


    /// <summary>
    /// A RoutingSlipBuilder is used to create a routing slip with proper validation that the resulting RoutingSlip
    /// is valid.
    /// </summary>
    public class RoutingSlipBuilder :
        ItineraryBuilder
    {
        public static readonly IDictionary<string, object> NoArguments = new Dictionary<string, object>();

        readonly IList<ActivityException> _activityExceptions;
        readonly IList<ActivityLog> _activityLogs;
        readonly IList<CompensateLog> _compensateLogs;
        readonly DateTime _createTimestamp;
        readonly IList<Activity> _itinerary;
        readonly List<Activity> _sourceItinerary;
        readonly Guid _trackingNumber;
        readonly IDictionary<string, object> _variables;
        IList<Subscription> _subscriptions;

        public RoutingSlipBuilder(Guid trackingNumber)
        {
            _trackingNumber = trackingNumber;
            _createTimestamp = DateTime.UtcNow;

            _itinerary = new List<Activity>();
            _sourceItinerary = new List<Activity>();
            _activityLogs = new List<ActivityLog>();
            _activityExceptions = new List<ActivityException>();
            _compensateLogs = new List<CompensateLog>();
            _variables = new Dictionary<string, object>();
            _subscriptions = new List<Subscription>();
        }

        public RoutingSlipBuilder(RoutingSlip routingSlip, Func<IEnumerable<Activity>, IEnumerable<Activity>> activitySelector)
        {
            _trackingNumber = routingSlip.TrackingNumber;
            _createTimestamp = routingSlip.CreateTimestamp;
            _itinerary = new List<Activity>(activitySelector(routingSlip.Itinerary));
            _activityLogs = new List<ActivityLog>(routingSlip.ActivityLogs);
            _compensateLogs = new List<CompensateLog>(routingSlip.CompensateLogs);
            _activityExceptions = new List<ActivityException>(routingSlip.ActivityExceptions);
            _variables = new Dictionary<string, object>(routingSlip.Variables);
            _subscriptions = new List<Subscription>(routingSlip.Subscriptions);

            _sourceItinerary = new List<Activity>();
        }

        public RoutingSlipBuilder(RoutingSlip routingSlip,
            Func<IEnumerable<CompensateLog>, IEnumerable<CompensateLog>> compensateLogSelector)
        {
            _trackingNumber = routingSlip.TrackingNumber;
            _createTimestamp = routingSlip.CreateTimestamp;
            _itinerary = new List<Activity>(routingSlip.Itinerary);
            _activityLogs = new List<ActivityLog>(routingSlip.ActivityLogs);
            _compensateLogs = new List<CompensateLog>(compensateLogSelector(routingSlip.CompensateLogs));
            _activityExceptions = new List<ActivityException>(routingSlip.ActivityExceptions);
            _variables = new Dictionary<string, object>(routingSlip.Variables);
            _subscriptions = new List<Subscription>(routingSlip.Subscriptions);

            _sourceItinerary = new List<Activity>();
        }

        /// <summary>
        /// The tracking number of the routing slip
        /// </summary>
        public Guid TrackingNumber
        {
            get { return _trackingNumber; }
        }

        /// <summary>
        /// Adds an activity to the routing slip without specifying any arguments
        /// </summary>
        /// <param name="name">The activity name</param>
        /// <param name="executeAddress">The execution address of the activity</param>
        public void AddActivity(string name, Uri executeAddress)
        {
            Activity activity = new ActivityImpl(name, executeAddress, NoArguments);
            _itinerary.Add(activity);
        }

        /// <summary>
        /// Adds an activity to the routing slip specifying activity arguments as an anonymous object
        /// </summary>
        /// <param name="name">The activity name</param>
        /// <param name="executeAddress">The execution address of the activity</param>
        /// <param name="arguments">An anonymous object of properties matching the argument names of the activity</param>
        public void AddActivity(string name, Uri executeAddress, object arguments)
        {
            IDictionary<string, object> argumentsDictionary = GetObjectAsDictionary(arguments);

            AddActivity(name, executeAddress, argumentsDictionary);
        }

        /// <summary>
        /// Adds an activity to the routing slip specifying activity arguments a dictionary
        /// </summary>
        /// <param name="name">The activity name</param>
        /// <param name="executeAddress">The execution address of the activity</param>
        /// <param name="arguments">A dictionary of name/values matching the activity argument properties</param>
        public void AddActivity(string name, Uri executeAddress, IDictionary<string, object> arguments)
        {
            Activity activity = new ActivityImpl(name, executeAddress, arguments);
            _itinerary.Add(activity);
        }

        /// <summary>
        /// Add a string value to the routing slip
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddVariable(string key, string value)
        {
            _variables.Add(key, value);
        }

        /// <summary>
        /// Add an object variable to the routing slip
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddVariable(string key, object value)
        {
            _variables.Add(key, value);
        }

        /// <summary>
        /// Sets the value of any existing variables to the value in the anonymous object,
        /// as well as adding any additional variables that did not exist previously.
        /// 
        /// For example, SetVariables(new { IntValue = 27, StringValue = "Hello, World." });
        /// </summary>
        /// <param name="values"></param>
        public void SetVariables(object values)
        {
            IDictionary<string, object> dictionary = GetObjectAsDictionary(values);

            SetVariablesFromDictionary(dictionary);
        }


        public void SetVariables(IEnumerable<KeyValuePair<string, object>> values)
        {
            SetVariablesFromDictionary(values);
        }

        public int AddSourceItinerary()
        {
            int count = _sourceItinerary.Count;

            foreach (Activity activity in _sourceItinerary)
                _itinerary.Add(activity);
            _sourceItinerary.Clear();

            return count;
        }

        /// <summary>
        /// Builds the routing slip using the current state of the builder
        /// </summary>
        /// <returns>The RoutingSlip</returns>
        public RoutingSlip Build()
        {
            return new RoutingSlipImpl(_trackingNumber, _createTimestamp, _itinerary, _activityLogs, _compensateLogs, _activityExceptions,
                _variables, _subscriptions);
        }

        /// <summary>
        /// Specify the source itinerary for this routing slip builder
        /// </summary>
        /// <param name="sourceItinerary"></param>
        public void SetSourceItinerary(IEnumerable<Activity> sourceItinerary)
        {
            _sourceItinerary.Clear();
            _sourceItinerary.AddRange(sourceItinerary);
        }

        public void AddActivityLog(Host host, string name, Guid activityTrackingNumber, DateTime timestamp, TimeSpan duration)
        {
            _activityLogs.Add(new ActivityLogImpl(host, activityTrackingNumber, name, timestamp, duration));
        }

        public void AddCompensateLog(Guid activityTrackingNumber, Uri compensateAddress, object logObject)
        {
            IDictionary<string, object> data = GetObjectAsDictionary(logObject);

            _compensateLogs.Add(new CompensateLogImpl(activityTrackingNumber, compensateAddress, data));
        }

        public void AddCompensateLog(Guid activityTrackingNumber, Uri compensateAddress, IDictionary<string, object> data)
        {
            _compensateLogs.Add(new CompensateLogImpl(activityTrackingNumber, compensateAddress, data));
        }

        /// <summary>
        /// Adds an activity exception to the routing slip
        /// </summary>
        /// <param name="host"></param>
        /// <param name="name">The name of the faulted activity</param>
        /// <param name="activityTrackingNumber">The activity tracking number</param>
        /// <param name="timestamp">The timestamp of the exception</param>
        /// <param name="duration"></param>
        /// <param name="exception">The exception thrown by the activity</param>
        public void AddActivityException(Host host, string name, Guid activityTrackingNumber, DateTime timestamp, TimeSpan duration,
            Exception exception)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (exception == null)
                throw new ArgumentNullException("exception");

            var exceptionInfo = new ExceptionInfoImpl(exception);

            ActivityException activityException = new ActivityExceptionImpl(name, host, activityTrackingNumber, timestamp, duration,
                exceptionInfo);
            _activityExceptions.Add(activityException);
        }

        /// <summary>
        /// Adds an activity exception to the routing slip
        /// </summary>
        /// <param name="host"></param>
        /// <param name="name">The name of the faulted activity</param>
        /// <param name="activityTrackingNumber">The activity tracking number</param>
        /// <param name="timestamp">The timestamp of the exception</param>
        /// <param name="duration"></param>
        /// <param name="exceptionInfo"></param>
        public void AddActivityException(Host host, string name, Guid activityTrackingNumber, DateTime timestamp, TimeSpan duration,
            ExceptionInfo exceptionInfo)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (exceptionInfo == null)
                throw new ArgumentNullException("exceptionInfo");

            ActivityException activityException = new ActivityExceptionImpl(name, host, activityTrackingNumber, timestamp, duration,
                exceptionInfo);
            _activityExceptions.Add(activityException);
        }

        public void AddActivityException(ActivityException activityException)
        {
            if (activityException == null)
                throw new ArgumentNullException("activityException");

            _activityExceptions.Add(activityException);
        }


        void SetVariablesFromDictionary(IEnumerable<KeyValuePair<string, object>> logValues)
        {
            foreach (var logValue in logValues)
            {
                if (logValue.Value == null
                    || (logValue.Value is string && string.IsNullOrEmpty((string)logValue.Value)))
                    _variables.Remove(logValue.Key);
                else
                    _variables[logValue.Key] = logValue.Value;
            }
        }


        public static IDictionary<string, object> GetObjectAsDictionary(object values)
        {
            if (values == null)
                return new Dictionary<string, object>();

            IDictionary<string, object> dictionary = Statics.Converter.Convert(values);

            return dictionary.ToDictionary(x => x.Key, x => x.Value);
        }


        /// <summary>
        /// Add an explicit subscription to the routing slip events
        /// </summary>
        /// <param name="address">The destination address where the events are sent</param>
        /// <param name="events">The events to include in the subscription</param>
        public void AddSubscription(Uri address, RoutingSlipEvents events)
        {
            _subscriptions.Add(new SubscriptionImpl(address, events, RoutingSlipEventContents.All));
        }


        static class Statics
        {
            internal static readonly AnonymousObjectDictionaryConverter Converter =
                new AnonymousObjectDictionaryConverter();

            /// <summary>
            /// Forces lazy load of all static fields in a thread-safe way.
            /// The static initializer will not be executed until a property or method in that class
            /// has been executed for the first time.
            /// </summary>
            static Statics()
            {
            }
        }
    }
}