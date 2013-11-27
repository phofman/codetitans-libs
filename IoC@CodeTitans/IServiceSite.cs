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

namespace CodeTitans.Services
{
    /// <summary>
    /// Automated service provider set.
    /// </summary>
    public interface IServiceSite
    {
        /// <summary>
        /// Sets the service provider.
        /// </summary>
        void SetSite(IServiceProviderEx provider);

        /// <summary>
        /// Since it is impossible to pass paramters do the class constructor
        /// created by Activator on Compact Framework capable devices, then this function
        /// is used in its place.
        /// It will be called just after object allocation and only on Windows Mobile.
        /// </summary>
        void SetSiteArguments(params object[] initArgs);
    }
}