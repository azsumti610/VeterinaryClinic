using System;
using System.Collections.Generic;
using System.Text;

namespace VeterinaryClinic.Application.Users
{
    public interface IUserAdministrationService
    {
        Task<CreateDoctorResult> CreateDoctorAsync(
            CreateDoctorRequest request,
            CancellationToken cancellationToken = default);

        Task<DoctorActivationResult> ActivateDoctorAsync(
    string userId,
    string token,
    string password,
    CancellationToken cancellationToken = default);
    }
}
