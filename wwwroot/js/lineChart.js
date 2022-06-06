(function () {
    window.lineChart = {
        showChart: function (data, containerName) {
            Highcharts.chart(containerName, {
                chart: {
                    type: 'line'
                },
                title: {
                    text: ''
                },
                subtitle: {
                    text: ''
                },
                xAxis: {
                    categories: data.categories
                },
                yAxis: {
                    title: {
                        text: ''
                    }
                },
                tooltip: {
                    pointFormat: '<b>Spent amount</b>:{point.y:,.2f}'
                },
                //plotOptions: {
                //    line: {
                //        dataLabels: {
                //            enabled: true
                //        },
                //        enableMouseTracking: true
                //    }
                //},
                series: data.series
            });
        }
    }
})();