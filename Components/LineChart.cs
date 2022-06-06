using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MoneySaver.SPA.Models;

namespace MoneySaver.SPA.Components
{
    public partial class LineChart
    {
        [Inject]
        private IJSRuntime jsRuntimeService { get; set; }

        [Parameter]
        public LineChartDataModel Data { get; set; }

        [Parameter]
        public string ChartName { get; set; }

        [Parameter]
        public string ChartContainer { get; set; }

        public bool Update { get; set; } = false;

        protected override bool ShouldRender()
        {
            return this.Update;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await this.jsRuntimeService.InvokeVoidAsync(
                "lineChart.showChart", this.Data, this.ChartContainer);

            this.Update = false;
        }
    }
}
