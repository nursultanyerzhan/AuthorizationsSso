using System;
using System.Collections.Generic;

namespace AuthorizationsSso.Models;

public partial class AuthorizationAppClient
{
    public int Id { get; set; }

    public Guid ClientId { get; set; }

    public string Password { get; set; } = null!;

    public string SystemName { get; set; } = null!;

    public string RedirectUri { get; set; } = null!;
}
