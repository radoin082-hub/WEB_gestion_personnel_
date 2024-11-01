using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domains.Model.Employee;
using Infrastructures.Storages.EmployeStorages;
using Services.Interfaces;

namespace Services
{
	public class EmployeService : IEmployeService
	{
		private readonly EmployeStorage _employeStorage;

		public EmployeService(EmployeStorage employeStorage)
		{
			_employeStorage = employeStorage;
		}

		public async Task<List<Employe>> GetEmployeesAsync()
		{
			return await _employeStorage.GetAll();
		}

		public async Task<Employe?> GetEmployeeByIdAsync(int id)
		{
			return await _employeStorage.GetById(id);
		}

		public async Task AddEmployeAsync(Employe employee)
		{
			await _employeStorage.Add(employee);
		}

		public async Task UpdateEmployeAsync(Employe employee)
		{
			await _employeStorage.Update(employee);
		}

		public async Task DeleteEmployeAsync(int id)
		{
			await _employeStorage.Delete(id);
		}

		public async Task<int> GetTotaleNumberOfEmployeAsync()
		{
			return await _employeStorage.GetTotalNumberOfEmployees();
		}

		public async Task<decimal> GetTotaleSalaryForMonthAsync(DateTime month)
		{
			return await _employeStorage.GetTotalSalaryForMonth(month);
		}

		public async Task<List<Employe>> GetEmployeByFunctionIdAsync(int fonctionId)
		{
			return await _employeStorage.GetEmployeesByFunctionId(fonctionId);
		}

		public async Task<int?> GetEmployeIdByNameAsync(string nom, string prenom, string nomFonction)
		{
			return await _employeStorage.GetEmployeeIdByName(nom, prenom, nomFonction);
		}
	}
}
