using Microsoft.AspNetCore.Components;
using MoneySaver.SPA.Models;
using MoneySaver.SPA.Services;

namespace MoneySaver.SPA.Components
{
    public partial class CategoryDialog
    {
        private bool forUpdate = false;
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
            if (!this.forUpdate)
            {
                //TODO: Add new child/parent category
                await this.CategoryService.AddCategory(this.Category);
            }
            else
            {
                //TODO: Update child/parent category
                await this.CategoryService.UpdateCategory(this.Category);
            }

            await CloseEventCallback.InvokeAsync(true);
            StateHasChanged();
        }

        public void Show(TransactionCategory category = null)
        {
            if (category != null)
            {
                this.Category = category;
                this.originalCategoryName = category.Name;
                this.forUpdate = true;
            }
            else 
            {
                this.Category = new TransactionCategory();
                this.forUpdate = false;
            }

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
