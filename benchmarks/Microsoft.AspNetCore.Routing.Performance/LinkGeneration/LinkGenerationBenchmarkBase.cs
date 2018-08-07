// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.AspNetCore.Routing.Tree;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;

namespace Microsoft.AspNetCore.Routing.LinkGeneration
{
    public abstract class LinkGenerationBenchmarkBase
    {
        protected IServiceProvider CreateServices(params Endpoint[] endpoints)
        {
            var services = new ServiceCollection();
            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            services.AddLogging();
            services.AddOptions();
            services.AddRouting();

            services.TryAddEnumerable(
                ServiceDescriptor.Singleton<EndpointDataSource>(new DefaultEndpointDataSource(endpoints)));

            return services.BuildServiceProvider();
        }

        protected void AssertUrl(string expectedUrl, string actualUrl)
        {
            AssertUrl(expectedUrl, actualUrl, StringComparison.Ordinal);
        }

        protected void AssertUrl(string expectedUrl, string actualUrl, StringComparison stringComparison)
        {
            if (!string.Equals(expectedUrl, actualUrl, stringComparison))
            {
                throw new InvalidOperationException($"Expected: {expectedUrl}, Actual: {actualUrl}");
            }
        }

        protected MatcherEndpoint CreateEndpoint(
            string template,
            object defaults = null,
            object constraints = null,
            object requiredValues = null,
            int order = 0,
            string displayName = null,
            string routeName = null,
            params object[] metadata)
        {
            var d = new List<object>(metadata ?? Array.Empty<object>());
            d.Add(new RouteValuesAddressMetadata(routeName, new RouteValueDictionary(requiredValues)));

            return new MatcherEndpoint(
                MatcherEndpoint.EmptyInvoker,
                RoutePatternFactory.Parse(template, defaults, constraints),
                order,
                new EndpointMetadataCollection(d),
                displayName);
        }

        protected RequestContext CreateCurrentRequestContext(object ambientValues = null)
        {
            var ambientVals = ambientValues == null
                ? new RouteValueDictionary() : new RouteValueDictionary(ambientValues);

            var context = new DefaultHttpContext();
            context.Features.Set<IEndpointFeature>(new EndpointFeature
            {
                Values = ambientVals
            });

            return new RequestContext
            {
                HttpContext = context,
                AmbientValues = ambientVals
            };
        }

        protected void CreateOutboundRouteEntry(TreeRouteBuilder treeRouteBuilder, MatcherEndpoint endpoint)
        {
            var routeValuesAddressMetadata = endpoint.Metadata.GetMetadata<IRouteValuesAddressMetadata>();
            var requiredValues = routeValuesAddressMetadata?.RequiredValues ?? new RouteValueDictionary();

            AddOutboundRouteEntry(
                treeRouteBuilder,
                endpoint.RoutePattern.RawText,
                defaults: endpoint.RoutePattern.Defaults,
                requiredValues: requiredValues);
        }

        protected MatcherEndpoint[] GetGithubEndpoints()
        {
            var controllerCount = 0;
            var templatesVisited = new Dictionary<string, (int, int)>(StringComparer.OrdinalIgnoreCase);

            var endpoints = new MatcherEndpoint[243];
            endpoints[0] = CreateGithubEndpoint("/emojis", "GET");
            endpoints[1] = CreateGithubEndpoint("/events", "GET");
            endpoints[2] = CreateGithubEndpoint("/feeds", "GET");
            endpoints[3] = CreateGithubEndpoint("/gists", "GET");
            endpoints[4] = CreateGithubEndpoint("/gists", "POST");
            endpoints[5] = CreateGithubEndpoint("/issues", "GET");
            endpoints[6] = CreateGithubEndpoint("/markdown", "POST");
            endpoints[7] = CreateGithubEndpoint("/meta", "GET");
            endpoints[8] = CreateGithubEndpoint("/notifications", "GET");
            endpoints[9] = CreateGithubEndpoint("/notifications", "PUT");
            endpoints[10] = CreateGithubEndpoint("/rate_limit", "GET");
            endpoints[11] = CreateGithubEndpoint("/repositories", "GET");
            endpoints[12] = CreateGithubEndpoint("/user", "GET");
            endpoints[13] = CreateGithubEndpoint("/user", "PATCH");
            endpoints[14] = CreateGithubEndpoint("/users", "GET");
            endpoints[15] = CreateGithubEndpoint("/gists/public", "GET");
            endpoints[16] = CreateGithubEndpoint("/gists/starred", "GET");
            endpoints[17] = CreateGithubEndpoint("/gitignore/templates", "GET");
            endpoints[18] = CreateGithubEndpoint("/markdown/raw", "POST");
            endpoints[19] = CreateGithubEndpoint("/search/code", "GET");
            endpoints[20] = CreateGithubEndpoint("/search/issues", "GET");
            endpoints[21] = CreateGithubEndpoint("/search/repositories", "GET");
            endpoints[22] = CreateGithubEndpoint("/search/users", "GET");
            endpoints[23] = CreateGithubEndpoint("/user/emails", "GET");
            endpoints[24] = CreateGithubEndpoint("/user/emails", "DELETE");
            endpoints[25] = CreateGithubEndpoint("/user/emails", "POST");
            endpoints[26] = CreateGithubEndpoint("/user/followers", "GET");
            endpoints[27] = CreateGithubEndpoint("/user/following", "GET");
            endpoints[28] = CreateGithubEndpoint("/user/issues", "GET");
            endpoints[29] = CreateGithubEndpoint("/user/keys", "POST");
            endpoints[30] = CreateGithubEndpoint("/user/keys", "GET");
            endpoints[31] = CreateGithubEndpoint("/user/orgs", "GET");
            endpoints[32] = CreateGithubEndpoint("/user/repos", "GET");
            endpoints[33] = CreateGithubEndpoint("/user/repos", "POST");
            endpoints[34] = CreateGithubEndpoint("/user/starred", "GET");
            endpoints[35] = CreateGithubEndpoint("/user/subscriptions", "GET");
            endpoints[36] = CreateGithubEndpoint("/user/teams", "GET");
            endpoints[37] = CreateGithubEndpoint("/legacy/repos/search/{keyword}", "GET");
            endpoints[38] = CreateGithubEndpoint("/legacy/user/email/{email}", "GET");
            endpoints[39] = CreateGithubEndpoint("/legacy/user/search/{keyword}", "GET");
            endpoints[40] = CreateGithubEndpoint("/legacy/issues/search/{owner}/{repository}/{state}/{keyword}", "GET");
            endpoints[41] = CreateGithubEndpoint("/gitignore/templates/{language}", "GET");
            endpoints[42] = CreateGithubEndpoint("/notifications/threads/{id}", "GET");
            endpoints[43] = CreateGithubEndpoint("/notifications/threads/{id}", "PATCH");
            endpoints[44] = CreateGithubEndpoint("/user/following/{username}", "DELETE");
            endpoints[45] = CreateGithubEndpoint("/user/following/{username}", "GET");
            endpoints[46] = CreateGithubEndpoint("/user/following/{username}", "PUT");
            endpoints[47] = CreateGithubEndpoint("/user/keys/{keyId}", "DELETE");
            endpoints[48] = CreateGithubEndpoint("/user/keys/{keyId}", "GET");
            endpoints[49] = CreateGithubEndpoint("/notifications/threads/{id}/subscription", "DELETE");
            endpoints[50] = CreateGithubEndpoint("/notifications/threads/{id}/subscription", "GET");
            endpoints[51] = CreateGithubEndpoint("/notifications/threads/{id}/subscription", "PUT");
            endpoints[52] = CreateGithubEndpoint("/user/starred/{owner}/{repo}", "PUT");
            endpoints[53] = CreateGithubEndpoint("/user/starred/{owner}/{repo}", "GET");
            endpoints[54] = CreateGithubEndpoint("/user/starred/{owner}/{repo}", "DELETE");
            endpoints[55] = CreateGithubEndpoint("/user/subscriptions/{owner}/{repo}", "PUT");
            endpoints[56] = CreateGithubEndpoint("/user/subscriptions/{owner}/{repo}", "GET");
            endpoints[57] = CreateGithubEndpoint("/user/subscriptions/{owner}/{repo}", "DELETE");
            endpoints[58] = CreateGithubEndpoint("/gists/{id}", "GET");
            endpoints[59] = CreateGithubEndpoint("/gists/{id}", "PATCH");
            endpoints[60] = CreateGithubEndpoint("/gists/{id}", "DELETE");
            endpoints[61] = CreateGithubEndpoint("/orgs/{org}", "PATCH");
            endpoints[62] = CreateGithubEndpoint("/orgs/{org}", "GET");
            endpoints[63] = CreateGithubEndpoint("/teams/{teamId}", "PATCH");
            endpoints[64] = CreateGithubEndpoint("/teams/{teamId}", "GET");
            endpoints[65] = CreateGithubEndpoint("/teams/{teamId}", "DELETE");
            endpoints[66] = CreateGithubEndpoint("/users/{username}", "GET");
            endpoints[67] = CreateGithubEndpoint("/gists/{id}/comments", "GET");
            endpoints[68] = CreateGithubEndpoint("/gists/{id}/comments", "POST");
            endpoints[69] = CreateGithubEndpoint("/gists/{id}/forks", "POST");
            endpoints[70] = CreateGithubEndpoint("/gists/{id}/star", "DELETE");
            endpoints[71] = CreateGithubEndpoint("/gists/{id}/star", "GET");
            endpoints[72] = CreateGithubEndpoint("/gists/{id}/star", "PUT");
            endpoints[73] = CreateGithubEndpoint("/orgs/{org}/events", "GET");
            endpoints[74] = CreateGithubEndpoint("/orgs/{org}/issues", "GET");
            endpoints[75] = CreateGithubEndpoint("/orgs/{org}/members", "GET");
            endpoints[76] = CreateGithubEndpoint("/orgs/{org}/public_members", "GET");
            endpoints[77] = CreateGithubEndpoint("/orgs/{org}/repos", "GET");
            endpoints[78] = CreateGithubEndpoint("/orgs/{org}/repos", "POST");
            endpoints[79] = CreateGithubEndpoint("/orgs/{org}/teams", "POST");
            endpoints[80] = CreateGithubEndpoint("/orgs/{org}/teams", "GET");
            endpoints[81] = CreateGithubEndpoint("/teams/{teamId}/members", "GET");
            endpoints[82] = CreateGithubEndpoint("/teams/{teamId}/repos", "GET");
            endpoints[83] = CreateGithubEndpoint("/users/{username}/events", "GET");
            endpoints[84] = CreateGithubEndpoint("/users/{username}/followers", "GET");
            endpoints[85] = CreateGithubEndpoint("/users/{username}/gists", "GET");
            endpoints[86] = CreateGithubEndpoint("/users/{username}/keys", "GET");
            endpoints[87] = CreateGithubEndpoint("/users/{username}/orgs", "GET");
            endpoints[88] = CreateGithubEndpoint("/users/{username}/received_events", "GET");
            endpoints[89] = CreateGithubEndpoint("/users/{username}/repos", "GET");
            endpoints[90] = CreateGithubEndpoint("/users/{username}/starred", "GET");
            endpoints[91] = CreateGithubEndpoint("/users/{username}/subscriptions", "GET");
            endpoints[92] = CreateGithubEndpoint("/users/{username}/received_events/public", "GET");
            endpoints[93] = CreateGithubEndpoint("/users/{username}/events/orgs/{org}", "GET");
            endpoints[94] = CreateGithubEndpoint("/gists/{id}/comments/{commentId}", "DELETE");
            endpoints[95] = CreateGithubEndpoint("/gists/{id}/comments/{commentId}", "GET");
            endpoints[96] = CreateGithubEndpoint("/gists/{id}/comments/{commentId}", "PATCH");
            endpoints[97] = CreateGithubEndpoint("/orgs/{org}/members/{username}", "DELETE");
            endpoints[98] = CreateGithubEndpoint("/orgs/{org}/members/{username}", "GET");
            endpoints[99] = CreateGithubEndpoint("/orgs/{org}/public_members/{username}", "PUT");
            endpoints[100] = CreateGithubEndpoint("/orgs/{org}/public_members/{username}", "GET");
            endpoints[101] = CreateGithubEndpoint("/orgs/{org}/public_members/{username}", "DELETE");
            endpoints[102] = CreateGithubEndpoint("/teams/{teamId}/members/{username}", "GET");
            endpoints[103] = CreateGithubEndpoint("/teams/{teamId}/members/{username}", "PUT");
            endpoints[104] = CreateGithubEndpoint("/teams/{teamId}/members/{username}", "DELETE");
            endpoints[105] = CreateGithubEndpoint("/teams/{teamId}/memberships/{username}", "DELETE");
            endpoints[106] = CreateGithubEndpoint("/teams/{teamId}/memberships/{username}", "PUT");
            endpoints[107] = CreateGithubEndpoint("/teams/{teamId}/memberships/{username}", "GET");
            endpoints[108] = CreateGithubEndpoint("/users/{username}/following/{targetUser}", "GET");
            endpoints[109] = CreateGithubEndpoint("/teams/{teamId}/repos/{org}/{repo}", "PUT");
            endpoints[110] = CreateGithubEndpoint("/teams/{teamId}/repos/{owner}/{repo}", "DELETE");
            endpoints[111] = CreateGithubEndpoint("/teams/{teamId}/repos/{owner}/{repo}", "GET");
            endpoints[112] = CreateGithubEndpoint("/repos/{owner}/{repo}", "PATCH");
            endpoints[113] = CreateGithubEndpoint("/repos/{owner}/{repo}", "DELETE");
            endpoints[114] = CreateGithubEndpoint("/repos/{owner}/{repo}", "GET");
            endpoints[115] = CreateGithubEndpoint("/networks/{owner}/{repo}/events", "GET");
            endpoints[116] = CreateGithubEndpoint("/repos/{owner}/{repo}/assignees", "GET");
            endpoints[117] = CreateGithubEndpoint("/repos/{owner}/{repo}/branches", "GET");
            endpoints[118] = CreateGithubEndpoint("/repos/{owner}/{repo}/collaborators", "GET");
            endpoints[119] = CreateGithubEndpoint("/repos/{owner}/{repo}/comments", "GET");
            endpoints[120] = CreateGithubEndpoint("/repos/{owner}/{repo}/commits", "GET");
            endpoints[121] = CreateGithubEndpoint("/repos/{owner}/{repo}/contributors", "GET");
            endpoints[122] = CreateGithubEndpoint("/repos/{owner}/{repo}/deployments", "GET");
            endpoints[123] = CreateGithubEndpoint("/repos/{owner}/{repo}/deployments", "POST");
            endpoints[124] = CreateGithubEndpoint("/repos/{owner}/{repo}/downloads", "GET");
            endpoints[125] = CreateGithubEndpoint("/repos/{owner}/{repo}/events", "GET");
            endpoints[126] = CreateGithubEndpoint("/repos/{owner}/{repo}/forks", "GET");
            endpoints[127] = CreateGithubEndpoint("/repos/{owner}/{repo}/forks", "POST");
            endpoints[128] = CreateGithubEndpoint("/repos/{owner}/{repo}/hooks", "POST");
            endpoints[129] = CreateGithubEndpoint("/repos/{owner}/{repo}/hooks", "GET");
            endpoints[130] = CreateGithubEndpoint("/repos/{owner}/{repo}/issues", "GET");
            endpoints[131] = CreateGithubEndpoint("/repos/{owner}/{repo}/issues", "POST");
            endpoints[132] = CreateGithubEndpoint("/repos/{owner}/{repo}/keys", "GET");
            endpoints[133] = CreateGithubEndpoint("/repos/{owner}/{repo}/keys", "POST");
            endpoints[134] = CreateGithubEndpoint("/repos/{owner}/{repo}/labels", "GET");
            endpoints[135] = CreateGithubEndpoint("/repos/{owner}/{repo}/labels", "POST");
            endpoints[136] = CreateGithubEndpoint("/repos/{owner}/{repo}/languages", "GET");
            endpoints[137] = CreateGithubEndpoint("/repos/{owner}/{repo}/merges", "POST");
            endpoints[138] = CreateGithubEndpoint("/repos/{owner}/{repo}/milestones", "GET");
            endpoints[139] = CreateGithubEndpoint("/repos/{owner}/{repo}/milestones", "POST");
            endpoints[140] = CreateGithubEndpoint("/repos/{owner}/{repo}/notifications", "PUT");
            endpoints[141] = CreateGithubEndpoint("/repos/{owner}/{repo}/notifications", "GET");
            endpoints[142] = CreateGithubEndpoint("/repos/{owner}/{repo}/pulls", "POST");
            endpoints[143] = CreateGithubEndpoint("/repos/{owner}/{repo}/pulls", "GET");
            endpoints[144] = CreateGithubEndpoint("/repos/{owner}/{repo}/readme", "GET");
            endpoints[145] = CreateGithubEndpoint("/repos/{owner}/{repo}/releases", "POST");
            endpoints[146] = CreateGithubEndpoint("/repos/{owner}/{repo}/releases", "GET");
            endpoints[147] = CreateGithubEndpoint("/repos/{owner}/{repo}/stargazers", "GET");
            endpoints[148] = CreateGithubEndpoint("/repos/{owner}/{repo}/subscribers", "GET");
            endpoints[149] = CreateGithubEndpoint("/repos/{owner}/{repo}/subscription", "PUT");
            endpoints[150] = CreateGithubEndpoint("/repos/{owner}/{repo}/subscription", "GET");
            endpoints[151] = CreateGithubEndpoint("/repos/{owner}/{repo}/subscription", "DELETE");
            endpoints[152] = CreateGithubEndpoint("/repos/{owner}/{repo}/tags", "GET");
            endpoints[153] = CreateGithubEndpoint("/repos/{owner}/{repo}/teams", "GET");
            endpoints[154] = CreateGithubEndpoint("/repos/{owner}/{repo}/watchers", "GET");
            endpoints[155] = CreateGithubEndpoint("/repos/{owner}/{repo}/git/blobs", "POST");
            endpoints[156] = CreateGithubEndpoint("/repos/{owner}/{repo}/git/commits", "POST");
            endpoints[157] = CreateGithubEndpoint("/repos/{owner}/{repo}/git/refs", "GET");
            endpoints[158] = CreateGithubEndpoint("/repos/{owner}/{repo}/git/refs", "POST");
            endpoints[159] = CreateGithubEndpoint("/repos/{owner}/{repo}/git/tags", "POST");
            endpoints[160] = CreateGithubEndpoint("/repos/{owner}/{repo}/git/trees", "POST");
            endpoints[161] = CreateGithubEndpoint("/repos/{owner}/{repo}/issues/comments", "GET");
            endpoints[162] = CreateGithubEndpoint("/repos/{owner}/{repo}/issues/events", "GET");
            endpoints[163] = CreateGithubEndpoint("/repos/{owner}/{repo}/pulls/comments", "GET");
            endpoints[164] = CreateGithubEndpoint("/repos/{owner}/{repo}/stats/code_frequency", "GET");
            endpoints[165] = CreateGithubEndpoint("/repos/{owner}/{repo}/stats/commit_activity", "GET");
            endpoints[166] = CreateGithubEndpoint("/repos/{owner}/{repo}/stats/contributors", "GET");
            endpoints[167] = CreateGithubEndpoint("/repos/{owner}/{repo}/stats/participation", "GET");
            endpoints[168] = CreateGithubEndpoint("/repos/{owner}/{repo}/stats/punch_card", "GET");
            endpoints[169] = CreateGithubEndpoint("/repos/{owner}/{repo}/git/blobs/{shaCode}", "GET");
            endpoints[170] = CreateGithubEndpoint("/repos/{owner}/{repo}/git/commits/{shaCode}", "GET");
            endpoints[171] = CreateGithubEndpoint("/repos/{owner}/{repo}/git/refs/{ref}", "DELETE");
            endpoints[172] = CreateGithubEndpoint("/repos/{owner}/{repo}/git/refs/{ref}", "PATCH");
            endpoints[173] = CreateGithubEndpoint("/repos/{owner}/{repo}/git/refs/{ref}", "GET");
            endpoints[174] = CreateGithubEndpoint("/repos/{owner}/{repo}/git/tags/{shaCode}", "GET");
            endpoints[175] = CreateGithubEndpoint("/repos/{owner}/{repo}/git/trees/{shaCode}", "GET");
            endpoints[176] = CreateGithubEndpoint("/repos/{owner}/{repo}/issues/comments/{commentId}", "GET");
            endpoints[177] = CreateGithubEndpoint("/repos/{owner}/{repo}/issues/comments/{commentId}", "PATCH");
            endpoints[178] = CreateGithubEndpoint("/repos/{owner}/{repo}/issues/comments/{commentId}", "DELETE");
            endpoints[179] = CreateGithubEndpoint("/repos/{owner}/{repo}/issues/events/{eventId}", "GET");
            endpoints[180] = CreateGithubEndpoint("/repos/{owner}/{repo}/pulls/comments/{commentId}", "PATCH");
            endpoints[181] = CreateGithubEndpoint("/repos/{owner}/{repo}/pulls/comments/{commentId}", "GET");
            endpoints[182] = CreateGithubEndpoint("/repos/{owner}/{repo}/pulls/comments/{commentId}", "DELETE");
            endpoints[183] = CreateGithubEndpoint("/repos/{owner}/{repo}/releases/assets/{id}", "PATCH");
            endpoints[184] = CreateGithubEndpoint("/repos/{owner}/{repo}/releases/assets/{id}", "DELETE");
            endpoints[185] = CreateGithubEndpoint("/repos/{owner}/{repo}/releases/assets/{id}", "GET");
            endpoints[186] = CreateGithubEndpoint("/repos/{owner}/{repo}/assignees/{assignee}", "GET");
            endpoints[187] = CreateGithubEndpoint("/repos/{owner}/{repo}/branches/{branch}", "GET");
            endpoints[188] = CreateGithubEndpoint("/repos/{owner}/{repo}/collaborators/{user}", "PUT");
            endpoints[189] = CreateGithubEndpoint("/repos/{owner}/{repo}/collaborators/{user}", "DELETE");
            endpoints[190] = CreateGithubEndpoint("/repos/{owner}/{repo}/collaborators/{user}", "GET");
            endpoints[191] = CreateGithubEndpoint("/repos/{owner}/{repo}/comments/{commentId}", "DELETE");
            endpoints[192] = CreateGithubEndpoint("/repos/{owner}/{repo}/comments/{commentId}", "GET");
            endpoints[193] = CreateGithubEndpoint("/repos/{owner}/{repo}/comments/{commentId}", "PATCH");
            endpoints[194] = CreateGithubEndpoint("/repos/{owner}/{repo}/commits/{shaCode}", "GET");
            endpoints[195] = CreateGithubEndpoint("/repos/{owner}/{repo}/contents/{path}", "GET");
            endpoints[196] = CreateGithubEndpoint("/repos/{owner}/{repo}/contents/{path}", "DELETE");
            endpoints[197] = CreateGithubEndpoint("/repos/{owner}/{repo}/contents/{path}", "PUT");
            endpoints[198] = CreateGithubEndpoint("/repos/{owner}/{repo}/downloads/{downloadId}", "GET");
            endpoints[199] = CreateGithubEndpoint("/repos/{owner}/{repo}/downloads/{downloadId}", "DELETE");
            endpoints[200] = CreateGithubEndpoint("/repos/{owner}/{repo}/hooks/{hookId}", "DELETE");
            endpoints[201] = CreateGithubEndpoint("/repos/{owner}/{repo}/hooks/{hookId}", "GET");
            endpoints[202] = CreateGithubEndpoint("/repos/{owner}/{repo}/hooks/{hookId}", "PATCH");
            endpoints[203] = CreateGithubEndpoint("/repos/{owner}/{repo}/issues/{number}", "GET");
            endpoints[204] = CreateGithubEndpoint("/repos/{owner}/{repo}/issues/{number}", "PATCH");
            endpoints[205] = CreateGithubEndpoint("/repos/{owner}/{repo}/keys/{keyId}", "GET");
            endpoints[206] = CreateGithubEndpoint("/repos/{owner}/{repo}/keys/{keyId}", "DELETE");
            endpoints[207] = CreateGithubEndpoint("/repos/{owner}/{repo}/labels/{name}", "GET");
            endpoints[208] = CreateGithubEndpoint("/repos/{owner}/{repo}/labels/{name}", "PATCH");
            endpoints[209] = CreateGithubEndpoint("/repos/{owner}/{repo}/labels/{name}", "DELETE");
            endpoints[210] = CreateGithubEndpoint("/repos/{owner}/{repo}/milestones/{number}", "PATCH");
            endpoints[211] = CreateGithubEndpoint("/repos/{owner}/{repo}/milestones/{number}", "GET");
            endpoints[212] = CreateGithubEndpoint("/repos/{owner}/{repo}/milestones/{number}", "DELETE");
            endpoints[213] = CreateGithubEndpoint("/repos/{owner}/{repo}/pulls/{number}", "GET");
            endpoints[214] = CreateGithubEndpoint("/repos/{owner}/{repo}/pulls/{number}", "PATCH");
            endpoints[215] = CreateGithubEndpoint("/repos/{owner}/{repo}/releases/{id}", "PATCH");
            endpoints[216] = CreateGithubEndpoint("/repos/{owner}/{repo}/releases/{id}", "GET");
            endpoints[217] = CreateGithubEndpoint("/repos/{owner}/{repo}/releases/{id}", "DELETE");
            endpoints[218] = CreateGithubEndpoint("/repos/{owner}/{repo}/statuses/{ref}", "GET");
            endpoints[219] = CreateGithubEndpoint("/repos/{owner}/{repo}/statuses/{ref}", "POST");
            endpoints[220] = CreateGithubEndpoint("/repos/{owner}/{repo}/commits/{ref}/status", "GET");
            endpoints[221] = CreateGithubEndpoint("/repos/{owner}/{repo}/commits/{shaCode}/comments", "GET");
            endpoints[222] = CreateGithubEndpoint("/repos/{owner}/{repo}/commits/{shaCode}/comments", "POST");
            endpoints[223] = CreateGithubEndpoint("/repos/{owner}/{repo}/deployments/{id}/statuses", "GET");
            endpoints[224] = CreateGithubEndpoint("/repos/{owner}/{repo}/deployments/{id}/statuses", "POST");
            endpoints[225] = CreateGithubEndpoint("/repos/{owner}/{repo}/hooks/{hookId}/tests", "POST");
            endpoints[226] = CreateGithubEndpoint("/repos/{owner}/{repo}/issues/{number}/comments", "POST");
            endpoints[227] = CreateGithubEndpoint("/repos/{owner}/{repo}/issues/{number}/comments", "GET");
            endpoints[228] = CreateGithubEndpoint("/repos/{owner}/{repo}/issues/{number}/events", "GET");
            endpoints[229] = CreateGithubEndpoint("/repos/{owner}/{repo}/issues/{number}/labels", "POST");
            endpoints[230] = CreateGithubEndpoint("/repos/{owner}/{repo}/issues/{number}/labels", "GET");
            endpoints[231] = CreateGithubEndpoint("/repos/{owner}/{repo}/issues/{number}/labels", "PUT");
            endpoints[232] = CreateGithubEndpoint("/repos/{owner}/{repo}/issues/{number}/labels", "DELETE");
            endpoints[233] = CreateGithubEndpoint("/repos/{owner}/{repo}/milestones/{number}/labels", "GET");
            endpoints[234] = CreateGithubEndpoint("/repos/{owner}/{repo}/pulls/{number}/comments", "GET");
            endpoints[235] = CreateGithubEndpoint("/repos/{owner}/{repo}/pulls/{number}/comments", "POST");
            endpoints[236] = CreateGithubEndpoint("/repos/{owner}/{repo}/pulls/{number}/commits", "GET");
            endpoints[237] = CreateGithubEndpoint("/repos/{owner}/{repo}/pulls/{number}/files", "GET");
            endpoints[238] = CreateGithubEndpoint("/repos/{owner}/{repo}/pulls/{number}/merge", "PUT");
            endpoints[239] = CreateGithubEndpoint("/repos/{owner}/{repo}/pulls/{number}/merge", "GET");
            endpoints[240] = CreateGithubEndpoint("/repos/{owner}/{repo}/releases/{id}/assets", "GET");
            endpoints[241] = CreateGithubEndpoint("/repos/{owner}/{repo}/issues/{number}/labels/{name}", "DELETE");
            endpoints[242] = CreateGithubEndpoint("/repos/{owner}/{repo}/{archive_format}/{path}", "GET");

            return endpoints;

            MatcherEndpoint CreateGithubEndpoint(string template, string httpMethod)
            {
                // In attribute routing, same tempalte is used for all actions within that controller. The following
                // simulates that where we only incremenet the controller count when a new endpoint for a new template
                // is being created.
                if (!templatesVisited.TryGetValue(template, out var visitedTemplateInfo))
                {
                    controllerCount++;
                    visitedTemplateInfo = (controllerCount, 0);
                    templatesVisited[template] = visitedTemplateInfo;
                }

                // Increment the action count within a controller template
                visitedTemplateInfo.Item2++;

                var requiredValues = new
                {
                    area = (string)null,
                    controller = $"Controller{visitedTemplateInfo.Item1}",
                    action = $"Action{visitedTemplateInfo.Item2}",
                    page = (string)null
                };
                var defaults = new
                {
                    area = (string)null,
                    controller = $"Controller{visitedTemplateInfo.Item1}",
                    action = $"Action{visitedTemplateInfo.Item2}",
                    page = (string)null
                };
                var metadata = new List<object>();
                metadata.Add(new HttpMethodMetadata(new string[] { httpMethod }));

                return CreateEndpoint(
                    template,
                    defaults: defaults,
                    requiredValues: requiredValues,
                    metadata: metadata,
                    routeName: $"Controller{visitedTemplateInfo.Item1}");
            }
        }

        private void AddOutboundRouteEntry(
            TreeRouteBuilder treeRouteBuilder,
            string template,
            object defaults = null,
            object constraints = null,
            object requiredValues = null,
            int order = 0,
            string routeName = null)
        {
            treeRouteBuilder.MapOutbound(
                NullRouter.Instance,
                new RouteTemplate(RoutePatternFactory.Parse(template, defaults: defaults, constraints: constraints)),
                requiredLinkValues: new RouteValueDictionary(requiredValues),
                routeName,
                order);
        }
    }
}
