using GasStationBilling.Api.Models;

namespace GasStationBilling.Api.Services;

public interface IEmployeeService
{
    Task<List<EmployeeResponse>> GetAllAsync();
    Task<EmployeeResponse> CreateAsync(int requesterId, CreateEmployeeRequest request);
    Task<EmployeeResponse?> UpdatePermissionsAsync(int id, int requesterId, UpdateEmployeePermissionsRequest request);
}