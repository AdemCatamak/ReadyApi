using System;
using Microsoft.AspNetCore.Authentication;

namespace ReadyApi.Core.BasicAuth
{
    public static class BasicAuthenticationAppBuilderExtensions
    {
        public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder)
            => builder.AddBasic(BasicAuthDefaults.AuthenticationScheme);

        public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder, string authenticationScheme)
            => builder.AddBasic(authenticationScheme, configureOptions: null);

        public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder, Action<BasicAuthOptions> configureOptions)
            => builder.AddBasic(BasicAuthDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddBasic(
            this AuthenticationBuilder builder,
            string authenticationScheme,
            Action<BasicAuthOptions> configureOptions)
        {
            return builder.AddScheme<BasicAuthOptions, BasicAuthHandler>(authenticationScheme, configureOptions);
        }
    }
}