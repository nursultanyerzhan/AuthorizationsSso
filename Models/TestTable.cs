using System;
using System.Collections.Generic;

namespace AuthorizationsSso.Models;

public partial class TestTable
{
    public Guid Id { get; set; }

    public string? Message { get; set; }

    public DateTime? CreatedDate { get; set; }
}
