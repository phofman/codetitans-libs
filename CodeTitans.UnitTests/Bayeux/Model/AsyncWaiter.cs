#region License
/*
    Copyright (c) 2010, Paweł Hofman (CodeTitans)
    All Rights Reserved.

    Licensed under the Apache License version 2.0.
    For more information please visit:

    http://codetitans.codeplex.com/license
        or
    http://www.apache.org/licenses/


    For latest source code, documentation, samples
    and more information please visit:

    http://codetitans.codeplex.com/
*/
#endregion

using System;
using System.Threading;
using System.Windows.Forms;
using CodeTitans.Bayeux;

namespace CodeTitans.UnitTests.Bayeux.Model
{
    public class AsyncWaiter
    {
        private volatile int condition;
        private WaiterResults result = WaiterResults.Unknown;

        public void Reset()
        {
            result = WaiterResults.Unknown;
        }

        /// <summary>
        /// Wait for given number of seconds.
        /// </summary>
        public WaiterResults Wait(int seconds)
        {
            DateTime endTime = DateTime.Now.AddSeconds(seconds);

            while (DateTime.Now <= endTime)
            {
                Application.DoEvents();

                if (result != WaiterResults.Unknown)
                    break;

                Thread.Sleep(50);
            }

            if (result == WaiterResults.Unknown)
                return WaiterResults.Timeouted;

            return result;
        }

        public void Sleep(double miliseconds)
        {
            DateTime endTime = DateTime.Now.AddMilliseconds(miliseconds);

            while (DateTime.Now <= endTime)
            {
                Application.DoEvents();

                Thread.Sleep(10);
            }
        }

        public WaiterResults Wait(int seconds, IHttpDataSource source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            source.DataReceived += source_DataReceived;
            source.DataReceiveFailed += source_DataReceiveFailed;

            return Wait(seconds);
        }

        void source_DataReceiveFailed(object sender, HttpDataSourceEventArgs e)
        {
            e.DataSource.DataReceiveFailed -= source_DataReceiveFailed;
            Signal(false);
        }

        void source_DataReceived(object sender, HttpDataSourceEventArgs e)
        {
            e.DataSource.DataReceived -= source_DataReceived;
            Signal(true);
        }

        /// <summary>
        /// Sets the result of asynchronous operation.
        /// It will stop waiting in the main thread and verify the test result.
        /// </summary>
        public void Signal(bool success)
        {
            lock (this)
            {
                condition++;
                result = success ? WaiterResults.Success : WaiterResults.Failed;
            }
        }
    }
}
