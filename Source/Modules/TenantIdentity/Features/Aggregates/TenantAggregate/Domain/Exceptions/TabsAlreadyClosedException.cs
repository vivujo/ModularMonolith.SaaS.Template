﻿using Shared.Features.Domain.Exceptions;

namespace Modules.TenantIdentity.Features.Aggregates.TenantAggregate.Domain.Exceptions
{
    public class TabsAlreadyClosedException : DomainException
    {
        public TabsAlreadyClosedException(string message) : base(message)
        {
        }
    }
}