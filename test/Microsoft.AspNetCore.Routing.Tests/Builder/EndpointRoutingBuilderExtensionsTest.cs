﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Microsoft.AspNetCore.Builder
{
    public class EndpointRoutingBuilderExtensionsTest
    {
        [Fact]
        public void UseEndpointRouting_ServicesNotRegistered_Throws()
        {
            // Arrange
            var app = new ApplicationBuilder(Mock.Of<IServiceProvider>());

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => app.UseEndpointRouting());

            // Assert
            Assert.Equal(
                "Unable to find the required services. " +
                "Please add all the required services by calling 'IServiceCollection.AddRouting' " +
                "inside the call to 'ConfigureServices(...)' in the application startup code.",
                ex.Message);
        }

        [Fact]
        public void UseEndpoint_ServicesNotRegistered_Throws()
        {
            // Arrange
            var app = new ApplicationBuilder(Mock.Of<IServiceProvider>());

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => app.UseEndpoint());

            // Assert
            Assert.Equal(
                "Unable to find the required services. " +
                "Please add all the required services by calling 'IServiceCollection.AddRouting' " +
                "inside the call to 'ConfigureServices(...)' in the application startup code.",
                ex.Message);
        }

        [Fact]
        public async Task UseEndpointRouting_ServicesRegistered_SetsFeature()
        {
            // Arrange
            var services = CreateServices();

            var app = new ApplicationBuilder(services);

            app.UseEndpointRouting();

            var appFunc = app.Build();
            var httpContext = new DefaultHttpContext();

            // Act
            await appFunc(httpContext);

            // Assert
            Assert.NotNull(httpContext.Features.Get<IEndpointFeature>());
        }

        [Fact]
        public void UseEndpoint_WithoutRoutingServicesRegistered_Throws()
        {
            // Arrange
            var services = CreateServices();

            var app = new ApplicationBuilder(services);

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => app.UseEndpoint());

            // Assert
            Assert.Equal(
                "EndpointRoutingMiddleware must be added to the request execution pipeline before EndpointMiddleware. " +
                "Please add EndpointRoutingMiddleware by calling 'IApplicationBuilder.UseEndpointRouting' " +
                "inside the call to 'Configure(...)' in the application startup code.",
                ex.Message);
        }

        [Fact]
        public async Task UseEndpoint_ServicesRegisteredAndEndpointRoutingRegistered_SetsFeature()
        {
            // Arrange
            var services = CreateServices();

            var app = new ApplicationBuilder(services);

            app.UseEndpointRouting();
            app.UseEndpoint();

            var appFunc = app.Build();
            var httpContext = new DefaultHttpContext();

            // Act
            await appFunc(httpContext);

            // Assert
            Assert.NotNull(httpContext.Features.Get<IEndpointFeature>());
        }

        private IServiceProvider CreateServices()
        {
            var services = new ServiceCollection();

            services.AddLogging();
            services.AddOptions();
            services.AddRouting();

            return services.BuildServiceProvider();
        }
    }
}
