using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MoneySaver.SPA.Models;

namespace MoneySaver.SPA.Components
{
    public partial class PieChart
    {
        [Inject]
        private IJSRuntime jsRuntimeService { get; set; }

        [Parameter]
        public DataItem[] Data { get; set; }

        [Parameter]
        public string ChartName { get; set; }

        [Parameter]
        public string ChartContainer { get; set; }

        public bool Update { get; set; } = false;

        [Parameter]
        public double Total { get; set; }

        protected override bool ShouldRender()
        {
            return this.Update;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await this.jsRuntimeService.InvokeVoidAsync(
                "pieChart.showChart", this.Data, this.ChartContainer);

            this.Update = false;
            
        }
    }
}
