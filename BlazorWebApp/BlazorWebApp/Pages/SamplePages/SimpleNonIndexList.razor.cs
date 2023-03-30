using PlaylistManagementSystem.ViewModels;

namespace BlazorWebApp.Pages.SamplePages
{
    public partial class SimpleNonIndexList
    {
        #region Fields
        private List<EmployeeView> employees { get; set; } = new();
        private string employeeName { get; set; }
        #endregion

        private async Task AddToEmployeeList()
        {
            int maxId = employees.Count == 0
                    ? 1
                    : employees.Max(x => x.EmployeeId) + 1
                ;            employees.Add(new EmployeeView()
            {
                EmployeeId = maxId, Name = employeeName
            });
            await InvokeAsync(StateHasChanged);
        }

        private void RemoveEmployee(int employeeId)
        {
            var selectEmployee = employees.FirstOrDefault(x => x.EmployeeId == employeeId);
            if (selectEmployee != null)
            {
                employees.Remove(selectEmployee);
            }
        }

    }
}
