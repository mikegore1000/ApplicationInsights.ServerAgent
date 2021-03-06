﻿using System;
using System.Collections.Generic;
using Xunit;

namespace ApplicationInsights.ServerAgent.Tests
{
    public class ServerAgentTests
    {
        [Fact]
        public void when_starting_all_event_log_pollers_are_started()
        {
            var pollers = new List<FakeEventLogPoller> { new FakeEventLogPoller(), new FakeEventLogPoller() };

            var sut = new ServerAgent(pollers);
            sut.Start();

            foreach (var p in pollers)
            {
                Assert.True(p.Started, "the event log poller should have been started");
            }
        }

        [Fact]
        public void when_starting_an_already_started_agent_nothing_happens()
        {
            var pollers = new List<FakeEventLogPoller> { new FakeEventLogPoller(), new FakeEventLogPoller() };

            var sut = new ServerAgent(pollers);
            sut.Start();
            sut.Start();
        }

        class FakeEventLogPoller : IEventLogPoller
        {
            public void Start()
            {
                if(Started)
                    throw new Exception("Already started");

                Started = true;
            }

            public bool Started { get; private set; }
        }
    }
}
