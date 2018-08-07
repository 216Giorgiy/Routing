﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Routing.Tree;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Routing.LinkGeneration
{
    public class SingleRouteWithNoParametersBenchmark : LinkGenerationBenchmarkBase
    {
        private TreeRouter _treeRouter;
        private LinkGenerator _linkGenerator;
        private RequestContext _requestContext;

        [GlobalSetup]
        public void Setup()
        {
            var template = "Products/Details";
            var defaults = new { controller = "Products", action = "Details" };
            var requiredValues = new { controller = "Products", action = "Details" };

            // Endpoint routing related
            var endpoint = CreateEndpoint(template, defaults, requiredValues: requiredValues);
            var services = CreateServices(endpoint);
            _linkGenerator = services.GetRequiredService<LinkGenerator>();

            // Attribute routing related
            var treeRouteBuilder = services.GetRequiredService<TreeRouteBuilder>();
            CreateOutboundRouteEntry(treeRouteBuilder, endpoint);
            _treeRouter = treeRouteBuilder.Build();

            _requestContext = CreateCurrentRequestContext();
        }

        [Benchmark(Baseline = true)]
        public void TreeRouter()
        {
            var virtualPathData = _treeRouter.GetVirtualPath(new VirtualPathContext(
                _requestContext.HttpContext,
                ambientValues: _requestContext.AmbientValues,
                values: new RouteValueDictionary(
                    new
                    {
                        controller = "Products",
                        action = "Details",
                    })));

            AssertUrl("/Products/Details", virtualPathData?.VirtualPath);
        }

        [Benchmark]
        public void EndpointRouting()
        {
            var actualUrl = _linkGenerator.GetLink(
                _requestContext.HttpContext,
                values: new RouteValueDictionary(
                    new
                    {
                        controller = "Products",
                        action = "Details",
                    }));

            AssertUrl("/Products/Details", actualUrl);
        }
    }
}
