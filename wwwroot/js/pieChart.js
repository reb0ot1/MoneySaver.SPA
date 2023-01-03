(function () {
    window.pieChart = {
        showChart: function (data, containerName) {
            Highcharts.chart(containerName, {
                chart: {
                    type: 'pie'
                },
                title: {
                    text: ''
                },
                subtitle: {
                    text: ''
                },

                accessibility: {
                    point: {
                        valueSuffix: '%'
                    }
                },

                plotOptions: {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: {
                            enabled: false
                        },
                        showInLegend: false
                    }
                },

                tooltip: {
                    pointFormat: 'Spent amount:{point.amount:.1f} <br/><b>{point.percentage:.1f}%</b>'
                },

                series: [
                    {
                        colorByPoint: true,
                        data: data
                    }
                ]
            });
        }
    }
})();