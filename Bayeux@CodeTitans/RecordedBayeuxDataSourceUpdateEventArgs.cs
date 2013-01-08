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
using CodeTitans.JSon;

#if !CODETITANS_LIB_CORE
namespace CodeTitans.Bayeux
#else
namespace CodeTitans.Core.Net
#endif
{
    /// <summary>
    /// Arguments passed along with all update events notified from RecordedBayeuxDataSource.
    /// </summary>
    public sealed class RecordedBayeuxDataSourceUpdateEventArgs : EventArgs
    {
        internal RecordedBayeuxDataSourceUpdateEventArgs(RecordedBayeuxDataSource dataSource, IJSonObject request, IJSonMutableObject response)
        {
            if (dataSource == null)
                throw new ArgumentNullException("dataSource");

            DataSource = dataSource;
            Request = request;
            Response = response;
        }

        #region Properties

        /// <summary>
        /// Gets the reference to original data-source issueing the notification.
        /// </summary>
        public RecordedBayeuxDataSource DataSource
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the bayeux request presented as JSON object.
        /// </summary>
        public IJSonObject Request
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the recorded bayeux response presented as JSON object.
        /// It was already selected from the source and can only be updated here.
        /// </summary>
        public IJSonMutableObject Response
        {
            get;
            private set;
        }

        #endregion
    }
}
