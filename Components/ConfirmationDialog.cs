using Microsoft.AspNetCore.Components;

namespace MoneySaver.SPA.Components
{
    public partial class ConfirmationDialog
    {
        private string objectId;
        public bool ShowDialog { get; set; }

        [Parameter]
        public EventCallback<string> ProceedEventCallback { get; set; }

        [Parameter]
        public EventCallback<bool> CloseEventCallback { get; set; }

        public void Show(string id)
        {
            this.objectId = id;
            this.ShowDialog = true;
            StateHasChanged();
        }

        public async Task Proceed()
        {
            await ProceedEventCallback.InvokeAsync(this.objectId);
            this.ShowDialog = false;
            await CloseEventCallback.InvokeAsync(true);
        }

        public void Close()
        {
            this.ShowDialog = false;
            StateHasChanged();
        }
    }
}
