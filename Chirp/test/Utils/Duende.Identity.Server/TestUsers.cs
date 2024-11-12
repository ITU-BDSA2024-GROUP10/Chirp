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
        }
    };
}