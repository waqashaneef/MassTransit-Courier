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
namespace MassTransit.Courier.Serialization
{
    using MassTransit.Serialization;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Default conversion of properties using standard serialization approach
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DefaultJsonTypeConverter<T> :
        JsonTypeConverter<T>
        where T : class
    {
        public T Convert(JToken token)
        {
            if (token.Type == JTokenType.Null)
                token = new JObject();

            using (var jsonReader = new JTokenReader(token))
            {
                return (T)JsonMessageSerializer.Deserializer.Deserialize(jsonReader, typeof(T));
            }
        }
    }
}