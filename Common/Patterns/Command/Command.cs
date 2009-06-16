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

using System;

namespace Common.Patterns.Command
{
    using Impl;    

    namespace Impl
    {
        public delegate void CommandProc();
        public delegate void CommandProc<T1>(T1 a1);
        public delegate void CommandProc<T1, T2>(T1 a1, T2 a2);
        public delegate void CommandProc<T1, T2, T3>(T1 a1, T2 a2, T3 a3);
        public delegate void CommandProc<T1, T2, T3, T4>(T1 a1, T2 a2, T3 a3, T4 a4);
        public delegate void CommandProc<T1, T2, T3, T4, T5>(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5);
        public delegate void CommandProc<T1, T2, T3, T4, T5, T6>(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6);
        public delegate void CommandProc<T1, T2, T3, T4, T5, T6, T7>(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7);
        public delegate void CommandProc<T1, T2, T3, T4, T5, T6, T7, T8>(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8);
    }


    public interface ICommand : IDisposable
    {
        void Execute();
    }


    public static class CommandFactory
    {
        public static ICommand MakeCommand(CommandProc proc)
        {
            return new Command0(proc);
        }


        public static ICommand MakeCommand<T1>(CommandProc<T1> proc, T1 arg1)
        {
            return new Command1<T1>(proc, arg1);
        }


        public static ICommand MakeCommand<T1, T2>(CommandProc<T1, T2> proc, T1 arg1, T2 arg2)
        {
            return new Command2<T1, T2>(proc, arg1, arg2);
        }


        public static ICommand MakeCommand<T1, T2, T3>(CommandProc<T1, T2, T3> proc, T1 arg1, T2 arg2, T3 arg3)
        {
            return new Command3<T1, T2, T3>(proc, arg1, arg2, arg3);
        }


        public static ICommand MakeCommand<T1, T2, T3, T4>(CommandProc<T1, T2, T3, T4> proc, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return new Command4<T1, T2, T3, T4>(proc, arg1, arg2, arg3, arg4);
        }


        public static ICommand MakeCommand<T1, T2, T3, T4, T5>(CommandProc<T1, T2, T3, T4, T5> proc, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return new Command5<T1, T2, T3, T4, T5>(proc, arg1, arg2, arg3, arg4, arg5);
        }


        public static ICommand MakeCommand<T1, T2, T3, T4, T5, T6>(CommandProc<T1, T2, T3, T4, T5, T6> proc, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            return new Command6<T1, T2, T3, T4, T5, T6>(proc, arg1, arg2, arg3, arg4, arg5, arg6);
        }


        public static ICommand MakeCommand<T1, T2, T3, T4, T5, T6, T7>(CommandProc<T1, T2, T3, T4, T5, T6, T7> proc, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            return new Command7<T1, T2, T3, T4, T5, T6, T7>(proc, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
    }

    namespace Impl
    {
        internal class Command0 : ICommand
        {
            private readonly CommandProc _proc;

            public Command0(CommandProc proc)
            {
                _proc = proc;
            }

            public void Execute()
            {
                _proc.Invoke();
            }

            public void Dispose()
            {
            }
        }


        internal class Command1<T1> : ICommand
        {
            private readonly CommandProc<T1> _proc;
            private readonly T1 _arg1;

            public Command1(CommandProc<T1> proc, T1 arg1)
            {
                _proc = proc;
                _arg1 = arg1;
            }

            public void Execute()
            {
                _proc.Invoke(_arg1);
            }

            public void Dispose()
            {
            }
        }


        internal class Command2<T1, T2> : ICommand
        {
            private readonly CommandProc<T1, T2> _proc;
            private readonly T1 _arg1;
            private readonly T2 _arg2;

            public Command2(CommandProc<T1, T2> proc, T1 arg1, T2 arg2)
            {
                _proc = proc;
                _arg1 = arg1;
                _arg2 = arg2;
            }

            public void Execute()
            {
                _proc.Invoke(_arg1, _arg2);
            }

            public void Dispose()
            {
            }
        }


        internal class Command3<T1, T2, T3> : ICommand
        {
            private readonly CommandProc<T1, T2, T3> _proc;
            private readonly T1 _arg1;
            private readonly T2 _arg2;
            private readonly T3 _arg3;

            public Command3(CommandProc<T1, T2, T3> proc, T1 arg1, T2 arg2, T3 arg3)
            {
                _proc = proc;
                _arg1 = arg1;
                _arg2 = arg2;
                _arg3 = arg3;
            }

            public void Execute()
            {
                _proc.Invoke(_arg1, _arg2, _arg3);
            }

            public void Dispose()
            {
            }
        }


        internal class Command4<T1, T2, T3, T4> : ICommand
        {
            private readonly CommandProc<T1, T2, T3, T4> _proc;
            private readonly T1 _arg1;
            private readonly T2 _arg2;
            private readonly T3 _arg3;
            private readonly T4 _arg4;

            public Command4(CommandProc<T1, T2, T3, T4> proc, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            {
                _proc = proc;
                _arg1 = arg1;
                _arg2 = arg2;
                _arg3 = arg3;
                _arg4 = arg4;
            }

            public void Execute()
            {
                _proc.Invoke(_arg1, _arg2, _arg3, _arg4);
            }

            public void Dispose()
            {
            }
        }


        internal class Command5<T1, T2, T3, T4, T5> : ICommand
        {
            private readonly CommandProc<T1, T2, T3, T4, T5> _proc;
            private readonly T1 _arg1;
            private readonly T2 _arg2;
            private readonly T3 _arg3;
            private readonly T4 _arg4;
            private readonly T5 _arg5;

            public Command5(CommandProc<T1, T2, T3, T4, T5> proc, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
            {
                _proc = proc;
                _arg1 = arg1;
                _arg2 = arg2;
                _arg3 = arg3;
                _arg4 = arg4;
                _arg5 = arg5;
            }
            
            public void Execute()
            {
                _proc.Invoke(_arg1, _arg2, _arg3, _arg4, _arg5);
            }

            public void Dispose()
            {
            }
        }



        internal class Command6<T1, T2, T3, T4, T5, T6> : ICommand
        {
            private readonly CommandProc<T1, T2, T3, T4, T5, T6> _proc;
            private readonly T1 _arg1;
            private readonly T2 _arg2;
            private readonly T3 _arg3;
            private readonly T4 _arg4;
            private readonly T5 _arg5;
            private readonly T6 _arg6;

            public Command6(CommandProc<T1, T2, T3, T4, T5, T6> proc, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
            {
                _proc = proc;
                _arg1 = arg1;
                _arg2 = arg2;
                _arg3 = arg3;
                _arg4 = arg4;
                _arg5 = arg5;
                _arg6 = arg6;
            }


            public void Execute()
            {
                _proc.Invoke(_arg1, _arg2, _arg3, _arg4, _arg5, _arg6);
            }

            public void Dispose()
            {
            }
        }


        internal class Command7<T1, T2, T3, T4, T5, T6, T7> : ICommand
        {
            private readonly CommandProc<T1, T2, T3, T4, T5, T6, T7> _proc;
            private readonly T1 _arg1;
            private readonly T2 _arg2;
            private readonly T3 _arg3;
            private readonly T4 _arg4;
            private readonly T5 _arg5;
            private readonly T6 _arg6;
            private readonly T7 _arg7;

            public Command7(CommandProc<T1, T2, T3, T4, T5, T6, T7> proc, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
            {
                _proc = proc;
                _arg1 = arg1;
                _arg2 = arg2;
                _arg3 = arg3;
                _arg4 = arg4;
                _arg5 = arg5;
                _arg6 = arg6;
                _arg7 = arg7;
            }
            
            public void Execute()
            {
                _proc.Invoke(_arg1, _arg2, _arg3, _arg4, _arg5, _arg6, _arg7);
            }

            public void Dispose()
            {
                
            }
        }
    }
}
