﻿using Shared.Features.Domain;

namespace Modules.TenantIdentity.Features.Domain.TenantAggregate
{
    public class TenantSettings : Entity
    {
        public string IconURI { get; set; }
    }
}
