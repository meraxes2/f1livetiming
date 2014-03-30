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
using System.IO;
using F1.Exceptions;
using F1.Runtime;
#if WINDOWS_PHONE
using System.Windows;
#endif
#if WINRT
using Windows.UI.Xaml;
using Windows.ApplicationModel;
using Windows.Storage;
#endif

namespace F1.Simulator
{
    /// <summary>
    /// Reads keyframes from a directory path
    /// </summary>
    public class KeyFrame : IKeyFrame
    {
        private readonly string _rootDirectory;
        private int _lastKeyFrame = 1;

        public KeyFrame(string rootDirectory)
        {
            _rootDirectory = rootDirectory;
        }

        public Stream GetKeyFrame()
        {
            return GetKeyFrame(_lastKeyFrame);
        }

        public Stream GetKeyFrame(int frameNumber)
        {
#if WINDOWS_PHONE || WINRT
            String path = String.Format("{0}/keyframe_{1}.bin", _rootDirectory, frameNumber.ToString("d5"));            
#else
            String path = String.Format("{0}\\keyframe_{1}.bin", _rootDirectory, frameNumber.ToString("d5"));
#endif

            Stream ret = GetKeyFrame(path);
            _lastKeyFrame = frameNumber;

            return ret;
        }

        private static Stream GetKeyFrame(string path)
        {
            try
            {
#if WINRT
                Package package = Package.Current;
                StorageFolder folder = package.InstalledLocation;

                foreach(string name in path.Split('/'))
                {
                    if(name.Contains("."))
                    {
                        StorageFile file = folder.GetFileAsync(name).AsTask().Result;
                        var f = file.OpenReadAsync().AsTask().Result;
                        return f.AsStreamForRead(); 
                    }
                    else
                    {
                        folder = folder.GetFolderAsync(name).AsTask().Result; 
                    }                    
                }

                throw new Exception("Could not open file");
#elif WINDOWS_PHONE
                return Application.GetResourceStream(new Uri(path, UriKind.Relative)).Stream;
#else
                return File.OpenRead(path);
#endif
            }
            catch (Exception e)
            {
                throw new KeyFrameException("Could not open file: " + path, e);
            }
        }
    }
}
