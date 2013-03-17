using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Threading;

namespace Common
{
    public class HttpWebAdaptor
    {
        private HttpWebRequest _request;
        private AutoResetEvent _requestDone = new AutoResetEvent(false);

        public HttpWebAdaptor(HttpWebRequest request)
        {
            _request = request;
        }

        public HttpWebRequest Request
        {
            get
            {
                return _request;
            }
        }

        public Stream GetRequestStream()
        {
            try
            {
                IAsyncResult asyncRes = _request.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), null);
                _requestDone.WaitOne();

                return _request.EndGetRequestStream(asyncRes);
            }
            catch (System.Exception)
            {
            }

            return null;

        }

        private void GetRequestStreamCallback(IAsyncResult asyncRes)
        {
            _requestDone.Set();
        }

        public WebResponse GetResponse()
        {
            IAsyncResult asyncRes;

            try
            {
                asyncRes = _request.BeginGetResponse(new AsyncCallback(GetResponseCallback), null);
                _requestDone.WaitOne();

                return _request.EndGetResponse(asyncRes);
            }
            catch (System.Exception)
            {
            }

            return null;
        }

        private void GetResponseCallback(IAsyncResult asyncRes)
        {
            _requestDone.Set();
        }
    }
}
