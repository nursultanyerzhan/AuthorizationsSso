using System;
using System.Collections.Generic;

namespace AuthorizationsSso.Models;

public partial class Authorization
{
    public Guid Id { get; set; }

    public DateTime CreatedDate { get; set; }

    public string Iin { get; set; } = null!;

    public string? AccessToken { get; set; }

    public string? AccessCode { get; set; }

    public string? RefreshToken { get; set; }
}
