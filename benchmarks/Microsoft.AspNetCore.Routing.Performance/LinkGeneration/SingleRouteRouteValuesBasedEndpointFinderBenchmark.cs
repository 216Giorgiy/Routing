// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Routing.LinkGeneration
{
    public class SingleRouteRouteValuesBasedEndpointFinderBenchmark : LinkGenerationBenchmarkBase
    {
        private IEndpointFinder<RouteValuesAddress> _finder;
        private TestEndpointFinder _baseFinder;
        private RequestContext _requestContext;

        [GlobalSetup]
        public void Setup()
        {
            var template = "Products/Details";
            var defaults = new { controller = "Products", action = "Details" };
            var requiredValues = new { controller = "Products", action = "Details" };

            var endpoint = CreateEndpoint(template, defaults, requiredValues: requiredValues, routeName: "ProductDetails");
            var services = CreateServices();
            _finder = services.GetRequiredService<IEndpointFinder<RouteValuesAddress>>();
            _baseFinder = new TestEndpointFinder(endpoint);

            _requestContext = CreateCurrentRequestContext();
        }

        [Benchmark(Baseline = true)]
        public void Baseline()
        {
            var actual = _baseFinder.FindEndpoints(address: 0);
            GC.KeepAlive(actual);
        }

        [Benchmark]
        public void RouteValues()
        {
            var actual = _finder.FindEndpoints(new RouteValuesAddress
            {
                AmbientValues = _requestContext.AmbientValues,
                ExplicitValues = new RouteValueDictionary(new { controller = "Products", action = "Details" }),
                RouteName = null
            });
            GC.KeepAlive(actual);
        }

        [Benchmark]
        public void RouteName()
        {
            var actual = _finder.FindEndpoints(new RouteValuesAddress
            {
                AmbientValues = _requestContext.AmbientValues,
                RouteName = "ProductDetails"
            });
            GC.KeepAlive(actual);
        }

        private class TestEndpointFinder : IEndpointFinder<int>
        {
            private readonly Endpoint _endpoint;

            public TestEndpointFinder(Endpoint endpoint)
            {
                _endpoint = endpoint;
            }

            public IEnumerable<Endpoint> FindEndpoints(int address)
            {
                return new[] { _endpoint };
            }
        }
    }
}
