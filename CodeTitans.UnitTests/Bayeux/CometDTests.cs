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
using System.Collections.Generic;
using System.Text;
using CodeTitans.Bayeux;
using CodeTitans.JSon;
using CodeTitans.UnitTests.Bayeux.Model;
using System.Threading;
#if NUNIT
using NUnit.Framework;
using TestClassAttribute=NUnit.Framework.TestFixtureAttribute;
using TestMethodAttribute=NUnit.Framework.TestAttribute;
using TestInitializeAttribute=NUnit.Framework.SetUpAttribute;
using TestCleanupAttribute=NUnit.Framework.TearDownAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace CodeTitans.UnitTests.Bayeux
{
    /// <summary>
    /// Class testing general communication using Bayeux client with a CometD server.
    /// Server application must be installed separatelly and can be downloaded from http://www.cometd.org (http://cometd.org/documentation/installation).
    /// </summary>
    [TestClass]
    public class CometDTests
    {
        private BayeuxConnection connection;
        private AsyncWaiter waiter;

        [TestInitialize]
        public void TestInit()
        {
            connection = new BayeuxConnection("http://192.168.2.179:8080/cometd/");
            connection.DataReceived += LogDataReceived;
            connection.DataFailed += LogDataFailed;
            connection.ConnectionFailed += LogConnectionFailed;
            connection.Timeout = 2000;

            waiter = new AsyncWaiter();
            waiter.Reset();
        }

        [TestCleanup]
        public void TestCleanUp()
        {
            connection.Cancel();
        }

        void LogDataReceived(object sender, BayeuxConnectionEventArgs e)
        {
            IJSonWriter output = new JSonWriter(true);
            output.Write(e.Message);

            Console.WriteLine("Response: {0}", output);
            Console.WriteLine();
        }

        void LogDataFailed(object sender, BayeuxConnectionEventArgs e)
        {
            Console.WriteLine("HTTP status: {0} ({1})", e.StatusCode, e.StatusDescription);
            Console.WriteLine("Exception: {0}", e.Exception);
        }

        void LogConnectionFailed(object sender, BayeuxConnectionEventArgs e)
        {
            Console.WriteLine("HTTP status: {0} ({1})", e.StatusCode, e.StatusDescription);
            Console.WriteLine("Exception: {0}", e.Exception);
        }


        void LogChatEventReceived(object sender, BayeuxConnectionEventArgs e)
        {
            string channel = e.Message["channel"].StringValue;

            if (channel == "/chat/demo" && e.Message.Contains("data"))
            {
                Console.WriteLine("Chat message: {0} said: {1}", e.Message["data"]["user"].StringValue, e.Message["data"]["chat"].StringValue);
            }

            if (channel == "/chat/members" && e.Message.Contains("data"))
            {
                Console.WriteLine("Chat members:");
                foreach (var member in e.Message["data"].ArrayItems)
                {
                    Console.WriteLine(" - {0}", member.StringValue);
                }
            }
        }

        [TestMethod]
        [Ignore]
        public void HandshakeWithServer()
        {
            connection.DataReceived += DataReceived;
            connection.ConnectionFailed += ConnectionFailed;

            connection.Handshake();

            Assert.AreEqual(WaiterResults.Success, waiter.Wait(15));
        }

        void ConnectionFailed(object sender, BayeuxConnectionEventArgs e)
        {
            waiter.Signal(false);
        }

        void DataReceived(object sender, BayeuxConnectionEventArgs e)
        {
            waiter.Signal(true);
        }

        [TestMethod]
        [Ignore]
        public void ConnectToServer()
        {
            connection.EventReceived += LogChatEventReceived;

            // connect for the first time:
            connection.HandshakeSync();
            Assert.AreEqual(BayeuxConnectionState.Connected, connection.State, "Invalid state of the connection!");
            Assert.IsNotNull(connection.ClientID, "Not received ClientID!");

            var clientID = connection.ClientID;
            connection.ConnectSync();
            Assert.AreEqual(BayeuxConnectionState.Connected, connection.State, "Invalid state of the connection!");
            Assert.IsNotNull(connection.ClientID);

            connection.DisconnectSync();
            Assert.AreEqual(BayeuxConnectionState.Disconnected, connection.State, "Invalid state after disconnection!");
            Assert.IsNull(connection.ClientID);

            // reconnect:
            connection.HandshakeSync();
            Assert.AreEqual(BayeuxConnectionState.Connected, connection.State, "Invalid state of the connection!");
            Assert.IsNotNull(connection.ClientID, "Not received ClientID!");

            connection.ConnectSync();
            Assert.AreEqual(BayeuxConnectionState.Connected, connection.State, "Invalid state of the connection!");
            Assert.IsNotNull(connection.ClientID);

            Assert.AreNotEqual(clientID, connection.ClientID, "ClientID before reconnection should be different than the current one!");

            // subscribe to chat:
            connection.SubscribeSync("/chat/demo");
            Assert.IsTrue(connection.Subscribed("/chat/demo"), "Connection should be subscribed");

            connection.SubscribeSync("/chat/members");
            Assert.IsTrue(connection.Subscribed("/chat/members"), "Connection should be subscribed");

            // join the chat room:
            connection.PublishSync("/service/members", new BayeuxEventData("user", "Test-User", "room", "/chat/demo"));
            connection.PublishSync("/chat/demo", new BayeuxEventData("user", "Test-User", "membership", "join", "chat", "Test-User has joinded!"));

            // wait for some time:
            Thread.Sleep(5000);

            // ussubscribe from chat:
            connection.PublishSync("/chat/demo", new BayeuxEventData("user", "Test-User", "membership", "leave", "chat", "Test-User is leaving!"));
            //connection.UnsubscribeSync("/chat/demo");
            connection.DisconnectSync();
        }
    }
}
