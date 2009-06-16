/*
 *  f1livetiming - Part of the Live Timing Library for .NET
 *  Copyright (C) 2009 Liam Lowey
 *  
 *      http://livetiming.turnitin.co.uk/
 *
 *  Licensed under the Apache License, Version 2.0 (the "License"); 
 *  you may not use this file except in compliance with the License. 
 *  You may obtain a copy of the License at 
 *  
 *      http://www.apache.org/licenses/LICENSE-2.0 
 *  
 *  Unless required by applicable law or agreed to in writing, software 
 *  distributed under the License is distributed on an "AS IS" BASIS, 
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
 *  See the License for the specific language governing permissions and 
 *  limitations under the License. 
 */

namespace Common.Patterns.Singleton
{
    /// <summary>
    /// This pattern follows a very simple non locking singleton.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BasicSingleton<T> where T : new()
    {
        private static readonly T _instance = new T();

        public static T Instance
        {
            get
            {
                return _instance;
            }
        }

        protected BasicSingleton()
        {
        }
    }
}
