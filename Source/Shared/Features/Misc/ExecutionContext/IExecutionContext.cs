﻿using Microsoft.Extensions.Hosting;
using Shared.Kernel.DomainKernel;

namespace Shared.Features.Misc.ExecutionContext
{
    public interface IExecutionContext
    {
        Guid UserId { get; }

        Guid TenantId { get; }

        SubscriptionPlanType TenantPlan { get; }

        TenantRole TenantRole { get; }

        public Uri BaseURI { get; }

        bool AuthenticatedRequest { get; }

        IHostEnvironment HostingEnvironment { get; }
    }
}
