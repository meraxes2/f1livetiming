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

namespace F1.Data.Encryption
{
    /// <summary>
    /// Provide a Stream type which decorates other stream types with the
    /// functionality for decrypting the data. This is used so that knowledge
    /// of encryption is not required outside of this domain.
    /// </summary>
    public class DecryptStreamDecorator : Stream
    {
        private readonly Stream _source;
        private readonly DataDecryptor _decryptor;

        public DecryptStreamDecorator(Stream source, DataDecryptor decryptor)
        {
            _source = source;
            _decryptor = decryptor;
        }

        
        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = _source.Read(buffer, offset, count);

            if (bytesRead > 0 )
            {
                if (_decryptor != null)
                {
                    _decryptor.DecryptData(buffer, offset, bytesRead);
                }
                else
                {
                    throw new PacketGarbageException(bytesRead);
                }
            }

            return bytesRead;
        }
        
        public override bool CanRead
        {
            get { return _source.CanRead; }
        }

        public override long Length
        {
            get { return _source.Length; }
        }
        
        #region Not Implemented

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }        

        public override long Position
        {
            get { return _source.Position; }
            set { throw new NotImplementedException("Set Position property not supported"); }
        }

        public override void Flush()
        {
            throw new NotImplementedException("Flush method not supported");
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException("Seek method not supported");
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException("SetLength method not supported");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException("Write method not supported");
        }

        #endregion
    }
}