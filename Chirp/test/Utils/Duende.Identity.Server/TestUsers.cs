// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using IdentityModel;
using System.Security.Claims;
using Duende.IdentityServer.Test;

namespace Duende.Identity.Server;

public static class TestUsers
{
    public static List<TestUser> Users => new()
    {
        new TestUser
        {
            SubjectId = "1",
            Username = "alice",
            Password = "password",

            Claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Name, "Alice Smith"),
                new Claim(JwtClaimTypes.Email, "Alice@exampel.com"),
                new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
            }
        },
        new TestUser
        {
            SubjectId = "2",
            Username = "TestWithAllInfo",
            Password = "password",

            Claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Name, "Mr. test with all info"),
                new Claim(JwtClaimTypes.Email, "Test@AllInfo.com"),
                new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
            }
        },
        new TestUser
        {
            SubjectId = "3",
            Username = "TestWithEmail",
            Password = "password",

            Claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Email, "Test@WithEmail.com"),
                new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
            }
        },
        new TestUser
        {
            SubjectId = "4",
            Username = "TestWithUserName",
            Password = "password",

            Claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Name, "mr. Test with username"),
                new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
            }
        },
        new TestUser
        {
            SubjectId = "5",
            Username = "TestWithNoInfo",
            Password = "password"
        },
        new TestUser
        {
            SubjectId = "6",
            Username = "Mr. Demo",
            Password = "password",

            Claims = new List<Claim>()
            {
                new Claim(JwtClaimTypes.Name, "Mr. Demo"),
                new Claim(JwtClaimTypes.Email, "MrDemo@Demo.com"),
                new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean)
            }
        }
    };
}