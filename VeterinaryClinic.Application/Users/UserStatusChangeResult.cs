using System;
using System.Collections.Generic;
using System.Text;

namespace VeterinaryClinic.Application.Users;

public sealed class UserStatusChangeResult
{
    public bool Succeeded { get; init; }

    public string? Error { get; init; }
}
