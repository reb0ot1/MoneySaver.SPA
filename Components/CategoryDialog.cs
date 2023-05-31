using Microsoft.AspNetCore.Components;
using MoneySaver.SPA.Models;
using MoneySaver.SPA.Models.Enums;
using MoneySaver.SPA.Services;

namespace MoneySaver.SPA.Components
{
    public partial class CategoryDialog
    {
        private CommandType? command = null;
        private string originalCategoryName = null;

        [Inject]
        public ICategoryService CategoryService { get; set; }

        public TransactionCategory Category { get; set; }

        public TransactionCategory ParentCategory { get; set; }

        [Parameter]
        public EventCallback<bool> CloseEventCallback { get; set; }

        public bool ShowDialog { get; set; }

        protected async Task HandleValidSubmit()
        {
            ShowDialog = false;
            switch (this.command)
            {
                case CommandType.Add:
                    await this.CategoryService.AddCategory(this.Category);
                    break;
                case CommandType.Update:
                    await this.CategoryService.UpdateCategory(this.Category);
                    break;
                default:
                    return;
            }

            await CloseEventCallback.InvokeAsync(true);
            StateHasChanged();
        }

        public void Show(CommandType command, TransactionCategory category = null)
        {
            if (category != null)
            {
                this.Category = category;
                this.originalCategoryName = category.Name;
            }
            else 
            {
                this.Category = new TransactionCategory();
            }

            this.command = command;

            this.ShowDialog = true;
            StateHasChanged();
        }

        public void Close()
        {
            this.ShowDialog = false;
            this.Category.Name = this.originalCategoryName??string.Empty;
            StateHasChanged();
        }
    }
}
