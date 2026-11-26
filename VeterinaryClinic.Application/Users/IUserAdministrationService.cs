using System;
using System.Collections.Generic;
using System.Text;
using VeterinaryClinic.Application.DTOs;

namespace VeterinaryClinic.Application.Users
{
    public interface IUserAdministrationService
    {
        Task<CreateDoctorResult> CreateDoctorAsync(
            CreateDoctorRequest request,
            CancellationToken cancellationToken = default);

        Task<UserStatusChangeResult> ActivateDoctorAsync(
    string userId,
    string token,
    string password,
    CancellationToken cancellationToken = default);

        Task<IReadOnlyList<UserSummaryDto>> GetUsersAsync(
        CancellationToken cancellationToken = default);
        Task<UserStatusChangeResult> SetUserActiveAsync(
            string userId,
            bool isActive,
            string actingAdminId,
            CancellationToken cancellationToken = default);
    }
}
