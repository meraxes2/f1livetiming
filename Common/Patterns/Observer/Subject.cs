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

using System.Collections.Generic;

namespace Common.Patterns.Observer
{
    public class Subject<TSubject, TNotify>
    {
        private readonly List<IObserver<TSubject, TNotify>> _observers = new List<IObserver<TSubject,TNotify>>();

        public void Attach( IObserver<TSubject,TNotify> observer )
        {
            _observers.Add( observer );
        }

        public void Detach( IObserver<TSubject,TNotify> observer )
        {
            _observers.Remove( observer );
        }

        public void Notify( TSubject subject, TNotify val )
        {
            _observers.ForEach( obs => obs.Update( subject, val ) );  
        }
    }
}