using InsuranceCore.DTO;
using InsuranceCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InsuranceCore.Interfaces
{
    public interface IAumsService
    {
        Task<List<AuthResponse>> GetUserInFeature(string branchcode, string featureid);

        Task<LoginResponse> AuthenticateUser(string username, string password, string accessToken, CancellationToken cancellationToken);
    }
}
