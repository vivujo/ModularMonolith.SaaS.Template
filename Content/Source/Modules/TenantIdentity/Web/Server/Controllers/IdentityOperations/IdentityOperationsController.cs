﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Modules.TenantIdentity.DomainFeatures.Application.Queries;
using Modules.TenantIdentity.DomainFeatures.TenantAggregate.Domain;
using Modules.TenantIdentity.DomainFeatures.UserAggregate.Application.Commands;
using Modules.TenantIdentity.DomainFeatures.UserAggregate.Application.Queries;
using Modules.TenantIdentity.DomainFeatures.UserAggregate.Domain;
using Modules.TenantIdentity.Web.Shared.DTOs;
using Modules.TenantIdentity.Web.Shared.DTOs.IdentityOperations;
using Shared.Infrastructure.CQRS.Command;
using Shared.Infrastructure.CQRS.Query;
using Shared.Web.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Modules.TenantIdentity.Web.Server.Controllers.IdentityOperations
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class IdentityOperationsController : BaseController
    {
        private readonly SignInManager<User> signInManager;

        public IdentityOperationsController(SignInManager<User> signInManager, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<BFFUserInfoDTO> GetClaimsOfCurrentUser()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return BFFUserInfoDTO.Anonymous;
            }
            return new BFFUserInfoDTO()
            {
                Claims = User.Claims.Select(claim => new ClaimValueDTO { Type = claim.Type, Value = claim.Value }).ToList()
            };
        }

        [HttpGet("selectTenant/{TenantId}")]
        public async Task<ActionResult> SetTenantForCurrentUser(Guid tenantId, [FromQuery] string redirectUri)
        {
            var user = await queryDispatcher.DispatchAsync<GetUserById, User>(new GetUserById { });

            var tenantMembershipsOfUserQuery = new GetAllTenantMembershipsOfUser() { UserId = user.Id };
            var tenantMemberships = await queryDispatcher.DispatchAsync<GetAllTenantMembershipsOfUser, List<TenantMembership>>(tenantMembershipsOfUserQuery);

            if (tenantMemberships.Select(t => t.Tenant.Id).Contains(tenantId))
            {
                var setSelectedTenantForUser = new SetSelectedTenantForUser { };
                await commandDispatcher.DispatchAsync(setSelectedTenantForUser);
                await signInManager.RefreshSignInAsync(user);
            }
            else
            {
                throw new Exception();
            }

            return LocalRedirect(redirectUri ?? "/");
        }

        [HttpGet("Logout")]
        public async Task<ActionResult> LogoutCurrentUser([FromQuery] string redirectUri)
        {
            await signInManager.SignOutAsync();
            return LocalRedirect(redirectUri ?? "/");
        }
    }
}