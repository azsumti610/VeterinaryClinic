using System;
using System.Collections.Generic;
using System.Text;

namespace VeterinaryClinic.Application.Services
{
    public sealed class GreetingService : IGreetingService
    {
        public string GetMessage() => "Приложението работи.";
    }
}
