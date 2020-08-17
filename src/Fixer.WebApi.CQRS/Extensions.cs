﻿using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Fixer.CQRS.Commands;
using Fixer.CQRS.Queries;
using Fixer.WebApi.CQRS.Builders;
using Fixer.WebApi.CQRS.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Fixer.WebApi.CQRS
{
    public static class Extensions
    {
        public static IApplicationBuilder UseDispatcherEndpoints(this IApplicationBuilder app,
            Action<IDispatcherEndpointsBuilder> builder)
        {
            var definitions = app.ApplicationServices.GetService<WebApiEndpointDefinitions>();

            return app.UseRouter(router =>
                builder(new DispatcherEndpointsBuilder(new EndpointsBuilder(router, definitions))));
        }

        public static IDispatcherEndpointsBuilder Dispatch(this IEndpointsBuilder endpoints,
            Func<IDispatcherEndpointsBuilder, IDispatcherEndpointsBuilder> builder)
            => builder(new DispatcherEndpointsBuilder(endpoints));

        public static IApplicationBuilder UsePublicContracts<T>(this IApplicationBuilder app,
            string endpoint = "/_contracts") => app.UsePublicContracts(endpoint, typeof(T));

        public static IApplicationBuilder UsePublicContracts(this IApplicationBuilder app,
            bool attributeRequired, string endpoint = "/_contracts")
            => app.UsePublicContracts(endpoint, null, attributeRequired);

        public static IApplicationBuilder UsePublicContracts(this IApplicationBuilder app,
            string endpoint = "/_contracts", Type attributeType = null, bool attributeRequired = true)
            => app.UseMiddleware<PublicContractsMiddleware>(string.IsNullOrWhiteSpace(endpoint) ? "/_contracts" :
                endpoint.StartsWith("/") ? endpoint : $"/{endpoint}", attributeType ?? typeof(PublicContractAttribute),
                attributeRequired);

        public static Task SendAsync<T>(this HttpContext context, T command) where T : class, Fixer.CQRS.Commands.ICommand
            => context.RequestServices.GetService<ICommandDispatcher>().SendAsync(command);

        public static Task<TResult> QueryAsync<TResult>(this HttpContext context, IQuery<TResult> query)
            => context.RequestServices.GetService<IQueryDispatcher>().QueryAsync(query);

        public static Task<TResult> QueryAsync<TQuery, TResult>(this HttpContext context, TQuery query)
            where TQuery : class, IQuery<TResult>
            => context.RequestServices.GetService<IQueryDispatcher>().QueryAsync<TQuery, TResult>(query);
    }
}
