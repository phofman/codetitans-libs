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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using CodeTitans.Bayeux;
using CodeTitans.JSon;
using CodeTitans.NUnitTests;
using CodeTitans.Services;
#if NUNIT
using NUnit.Framework;
using TestClassAttribute=NUnit.Framework.TestFixtureAttribute;
using TestMethodAttribute=NUnit.Framework.TestAttribute;
using TestInitializeAttribute=NUnit.Framework.SetUpAttribute;
using TestCleanupAttribute=NUnit.Framework.TearDownAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace CodeTitans.UnitTests.Core
{
    /// <summary>
    /// Summary description for ExceptionTests
    /// </summary>
    [TestClass]
    public class SerializationTests
    {
        #region Throw Exception Class

        public class Rethrower : MarshalByRefObject
        {
            public void Rethrow (Exception ex)
            {
                throw ex;
            }

            public void Rethrow<T>() where T : Exception, new()
            {
                throw new T();
            }

            public void Throw(string exceptionName)
            {
                Type t = Type.GetType(exceptionName);
                throw (Exception) Activator.CreateInstance(t);
            }
        }

        #endregion

        private AppDomain appDomain;
        private Rethrower thrower;

        [TestInitialize]
        public void TestInit()
        {
            appDomain = AppDomain.CreateDomain("Test App Domain", null, null);

            // load all required assemblies:
            var a1 = appDomain.Load(typeof (JSonReaderException).Assembly.GetName());
            var a2 = appDomain.Load(typeof (BayeuxException).Assembly.GetName());
            var a3 = appDomain.Load(typeof (ServiceCreationException).Assembly.GetName());

            Assert.IsNotNull(a1);
            Assert.IsNotNull(a2);
            Assert.IsNotNull(a3);

            thrower = (Rethrower) appDomain.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().GetName().CodeBase, typeof(Rethrower).FullName);
        }

        [TestCleanup]
        public void TestCleanUp()
        {
            AppDomain.Unload(appDomain);
        }

        private void ThrowExceptionFromAnotherDomain<T>() where T : Exception, new ()
        {
            thrower.Throw(typeof(T).AssemblyQualifiedName);
        }

        private void SerializeToStream<T>() where T : Exception, new ()
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream stream = new MemoryStream())
            {
                // serialize object:
                formatter.Serialize(stream, new T());

                // deserialize object and verify:
                stream.Position = 0;
                var obj = (T)formatter.Deserialize(stream);
                Assert.IsNotNull(obj);
                AssertExt.IsInstanceOf<T>(obj, "Should be a different type object");
            }
        }

        [TestMethod]
        public void VerifySecondaryDomain()
        {
            Assert.IsNotNull(appDomain);
            Assert.IsNotNull(thrower);
            Assert.AreNotEqual(AppDomain.CurrentDomain, appDomain);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowAndSerializeException()
        {
            thrower.Rethrow(new ArgumentNullException());
        }

        [TestMethod]
        [ExpectedException(typeof(BayeuxException))]
        public void ThrowAndSerializeBayeuxException()
        {
            SerializeToStream<BayeuxException>();
            ThrowExceptionFromAnotherDomain<BayeuxException>();
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceCreationException))]
        public void ThrowAndSerializeServiceCreationException()
        {
            SerializeToStream<ServiceCreationException>();
            ThrowExceptionFromAnotherDomain<ServiceCreationException>();
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceValidationException))]
        public void ThrowAndSerializeServiceValidationException()
        {
            SerializeToStream<ServiceValidationException>();
            ThrowExceptionFromAnotherDomain<ServiceValidationException>();
        }

        [TestMethod]
        [ExpectedException(typeof(JSonException))]
        public void ThrowAndSerializeJSonException()
        {
            SerializeToStream<JSonException>();
            ThrowExceptionFromAnotherDomain<JSonException>();
        }

        [TestMethod]
        [ExpectedException(typeof(JSonReaderException))]
        public void ThrowAndSerializeJSonReaderException()
        {
            SerializeToStream<JSonReaderException>();
            ThrowExceptionFromAnotherDomain<JSonReaderException>();
        }

        [TestMethod]
        [ExpectedException(typeof(JSonWriterException))]
        public void ThrowAndSerializeJSonWriterException()
        {
            SerializeToStream<JSonWriterException>();
            ThrowExceptionFromAnotherDomain<JSonWriterException>();
        }
    }
}
