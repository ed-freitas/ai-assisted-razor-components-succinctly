(function () {
    const charts = new Map();
    const palette = ["#0d6efd", "#198754", "#ffc107", "#dc3545", "#6f42c1", "#20c997"];

    window.smartExpenseCharts = {
        renderExpenseCategoryChart: function (canvas, labels, values) {
            if (!canvas || !window.Chart) {
                return;
            }

            const chartKey = canvas.id || "expense-category-chart";
            const existingChart = charts.get(chartKey);
            if (existingChart) {
                existingChart.destroy();
            }

            const chart = new Chart(canvas, {
                type: "doughnut",
                data: {
                    labels: labels,
                    datasets: [{
                        data: values,
                        backgroundColor: labels.map((_, index) => palette[index % palette.length]),
                        borderColor: "#ffffff",
                        borderWidth: 2
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            position: "bottom"
                        },
                        tooltip: {
                            callbacks: {
                                label: function (context) {
                                    const label = context.label || "";
                                    const value = Number(context.raw || 0);
                                    return `${label}: ${value.toLocaleString(undefined, {
                                        style: "currency",
                                        currency: "USD"
                                    })}`;
                                }
                            }
                        }
                    }
                }
            });

            charts.set(chartKey, chart);
        }
    };
})();
