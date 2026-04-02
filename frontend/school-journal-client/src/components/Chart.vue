<template>
  <div>
    <canvas ref="canvas"></canvas>
  </div>
</template>
<script>
    import { Chart, registerables } from 'chart.js';

    Chart.register(...registerables);

    export default {
    name: 'MyChart',

    props: {
        chartData: {
        type: Object,
        required: true,
        },
        options: {
        type: Object,
        default: () => ({}),
        },
    },

    data() {
        return {
        chartInstance: null
        }
    },

    mounted() {
        this.renderChart();
    },

    watch: {
        chartData: {
        deep: true,
        handler() {
            this.updateChart();
        }
        }
    },

    methods: {
        renderChart() {
        this.chartInstance = new Chart(this.$refs.canvas, {
            type: 'line',
            data: this.chartData,
            options: this.options,
        });
        },

        updateChart() {
        if (this.chartInstance) {
            this.chartInstance.destroy();
        }
        this.renderChart();
        }
    }
    };
</script>