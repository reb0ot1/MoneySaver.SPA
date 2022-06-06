using Microsoft.AspNetCore.Components;
using MoneySaver.SPA.Models;
using MoneySaver.SPA.Services;

namespace MoneySaver.SPA.Components
{
    public partial class CategoryDialog
    {
        private bool forUpdate = false;

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
            ResetDialog();
            if (category != null)
            {
                this.Category = category;
                if (category.TransactionCategoryId != null)
                {
                    this.forUpdate = true;
                }
            }

            this.ShowDialog = true;
            StateHasChanged();
        }

        public void Close()
        {
            this.ShowDialog = false;
            StateHasChanged();
        }

        private void ResetDialog()
        {
            this.Category = new TransactionCategory();
            this.forUpdate = false;
        }

    }
}
