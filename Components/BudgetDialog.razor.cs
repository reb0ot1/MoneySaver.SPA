using Microsoft.AspNetCore.Components;
using MoneySaver.SPA.Models;
using MoneySaver.SPA.Models.Enums;
using MoneySaver.SPA.Services;

namespace MoneySaver.SPA.Components
{
    public partial class BudgetDialog
    {
        private CommandType CommandType { get; set; }

        public BudgetModel BudgetModel { get; set; }

        [Inject]
        public IBudgetService BudgetService { get; set; }

        [Parameter]
        public EventCallback<bool> CloseEventCallback { get; set; }

        public bool ShowDialog { get; set; }

        protected async Task HandleValidSubmit()
        {
            ShowDialog = false;
            this.UpdateBudgetDates();
            if (this.CommandType == CommandType.Add)
            {
                var result = await this.BudgetService.CreateBudgetAsync(this.BudgetModel);
            }

            if (this.CommandType == CommandType.Update)
            {
                var result = await this.BudgetService.UpdateBudgetAsync(this.BudgetModel);
            }

            await CloseEventCallback.InvokeAsync(true);
            StateHasChanged();
        }

        public void Show(CommandType commandType, BudgetModel budget = null)
        {
            this.CommandType = commandType;
            if (commandType == CommandType.Add)
            {
                this.BudgetModel = new BudgetModel
                {

                    StartDate = DateTime.UtcNow.Date,
                    EndDate = DateTime.UtcNow
                };
            }

            if (commandType == CommandType.Update)
            {
                this.BudgetModel = new BudgetModel
                {
                    Id = budget.Id,
                    BudgetType = budget.BudgetType,
                    EndDate = budget.EndDate,
                    StartDate = budget.StartDate,
                    Name = budget.Name,
                    IsInUse = budget.IsInUse
                };
            }

            this.ShowDialog = true;
            StateHasChanged();
        }

        public void Close()
        {
            this.ShowDialog = false;
        }

        private void UpdateBudgetDates()
        {
            //TODO: Think a way to optimeze it
            var startDate = new DateTime(this.BudgetModel.StartDate.Year, this.BudgetModel.StartDate.Month, this.BudgetModel.StartDate.Day, 0, 0, 0);
            var endDate = new DateTime(this.BudgetModel.EndDate.Year, this.BudgetModel.EndDate.Month, this.BudgetModel.EndDate.Day, 23, 59, 59, 999);

            this.BudgetModel.StartDate = startDate;
            this.BudgetModel.EndDate = endDate;
        }
    }
}
