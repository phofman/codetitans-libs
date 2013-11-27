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

namespace CodeTitans.NUnitTests
{

    /// <summary>
    /// Extension class for NUnit compatibility with Microsoft Test Framework. 
    /// </summary>
    public static class AssertExt
    {
        public static void IsInstanceOf<T>(object o, string message)
        {
#if NUNIT
            NUnit.Framework.Assert.IsInstanceOf<T>(o, message);
#else
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(o, typeof(T), message);
#endif
        }
    }

#if NUNIT

    /// <summary>
    /// Class to add attribute that should be ignored in NUnit.
    /// </summary>
    public class DeploymentItemAttribute : System.Attribute
    {
        public DeploymentItemAttribute(string path)
        {
        }
    }
#endif

}

